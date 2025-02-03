using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Commands;

namespace Server;

public delegate int ConnectedClientsDelegate();

public class Server : IDisposable
{
    private readonly ServerConfig _config;
    private readonly CommandHandler _commandHandler;
    private readonly Dictionary<string, ClientHandler> _clients = [];
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cts = new();
    // private readonly BlockingCollection<string> _messageQueue = [];
    private readonly BlockingCollection<Message> _messageQueue = [];

    public Server(ServerConfig config)
    {
        _config = config;
        _commandHandler = new CommandHandler();
        _listener = new TcpListener(IPAddress.Loopback, _config.Port);
        InitializeCommands();
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Logger<Server>.LogInfo($"Server started on port {_config.Port}");

        try 
        {
            // Read server console input
            var consoleTask = Task.Run(ServerConsoleInput);

            // Start a task to handle new clients
            var newClientTask = Task.Run(HandleNewClientAsync, _cts.Token);

            // Start a task to process messages from the queue
            var processMessagesTask = Task.Run(async () =>
            {
                foreach (var message in _messageQueue.GetConsumingEnumerable(_cts.Token))
                {
                    if (message.Recipient == "All")
                        await BroadcastMessageAsync(message);
                    else if (_clients.TryGetValue(message.Recipient, out var client)) {
                        await client.SendMessageAsync(message);
                    }
                }
            }, _cts.Token);

            // Wait for any task to complete (error) or cancellation
            await Task.WhenAny(consoleTask, newClientTask, processMessagesTask);
        }
        catch (Exception ex)
        {
            Logger<Server>.LogError($"Unexpected error: {ex.Message}");
        }
        finally
        {
            _cts.Cancel();
            Dispose();
        }
    }

    public ConnectedClientsDelegate ConnectedClients => () => _clients.Count;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _listener?.Stop();
            foreach (var client in _clients.Values)
            {
                _ = DisconnectClient(client);
            }
            Logger<Server>.LogInfo("Server stopped");
        }
    }

    private void InitializeCommands()
    {
        _commandHandler.RegisterCommand(new ClearCommand());
        _commandHandler.RegisterCommand(new UsersCommand(ConnectedClients));
        _commandHandler.RegisterCommand(new SendToCommand());
        _commandHandler.RegisterCommand(new ExitCommand());
        _commandHandler.RegisterCommand(new HelpCommand(_commandHandler.GetCommands()));
    }

    private async Task HandleNewClientAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            if (_clients.Count >= _config.MaxClients)
                Logger<Server>.LogWarning($"Connection rejected: Server is full");

            var client = await _listener.AcceptTcpClientAsync();
            
            var username = $"User{_clients.Count + 1}";
            ClientHandler clientHandler = new (client, username, _commandHandler, _messageQueue);
            _clients.Add(username, clientHandler);

            try
            {
                Logger<Server>.LogInfo($"Client connected: {username}. Total clients: {_clients.Count}");
                await BroadcastMessageAsync(new Message("Server", "All", $"{username} joined the chat", DateTime.Now));

                _ = clientHandler.HandleCommunicationAsync();
            }
            catch (Exception ex)
            {
                Logger<Server>.LogError($"Unexpected error: {ex.Message}");
                if (clientHandler != null)
                    await DisconnectClient(clientHandler);
            }
        }
    }

    private async Task BroadcastMessageAsync(Message message)
    {
        foreach (var client in _clients.Values)
        {
            await client.SendMessageAsync(message);
        };
    }

    private void ServerConsoleInput()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            var message = Console.ReadLine();
            if (message == null)
                break;
            
            if (CommandHandler.IsCommand(message))
            {
                var response = _commandHandler.ExecuteCommand(
                    CommandHandler.GetCommandName(message),
                    CommandHandler.GetCommandParameters(message)
                );
                if (response != null)
                    Console.WriteLine(response);
            }
            else
            {
                string serverMessage = $"Server: {message}";
                Console.WriteLine(serverMessage);
                _messageQueue.Add(new Message("Server", "All", message, DateTime.Now));
            }
        }
    }

    private async Task DisconnectClient(ClientHandler client)
    {
        if (_clients.Remove(client.Username)) {
            Logger<Server>.LogInfo($"{client.Username} disconnected. Total clients: {_clients.Count}");
            await BroadcastMessageAsync(new Message("Server", "All", $"{client.Username} left the chat", DateTime.Now));
            client.Dispose();
        }
    }
}