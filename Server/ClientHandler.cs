using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Server.Commands;
using Shared;

namespace Server;

public class ClientHandler : IDisposable
{
    public string Username { get; }
    private readonly TcpClient _client;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly CommandHandler _commandHandler;
    private readonly CancellationTokenSource _cts = new();
    private readonly BlockingCollection<Message> _messageQueue;

    public ClientHandler(
        TcpClient client, string username, CommandHandler commandHandler, BlockingCollection<Message> messageQueue
    )
    {
        Username = username;
        _client = client;
        _commandHandler = commandHandler;
        _messageQueue = messageQueue;
        _reader = new StreamReader(client.GetStream());
        _writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
    }

    public async Task HandleCommunicationAsync()
    {
        try
        {
            await SendMessageAsync(
                new Message("Server", Username, "Welcome! Type /help for available commands.")
            );

            while (!_cts.Token.IsCancellationRequested)
            {
                var message = await _reader.ReadLineAsync();
                if (message == null) break;

                if (MessageParser.IsCommand(message))
                {
                    await HandleCommandAsync(message);
                }
                else
                    _messageQueue.Add(new Message(Username, "All", message));
            }
        }
        catch (IOException)
        {
            Logger<ClientHandler>.LogInfo($"Client {Username} disconnected unexpectedly");
        }
        catch (Exception ex)
        {
            Logger<ClientHandler>.LogError($"Error handling client {Username}: {ex.Message}");
        }
    }

    public async Task SendMessageAsync(Message message)
    {
        try
        {
            await _writer.WriteLineAsync(message.ToString());
        }
        catch (Exception ex)
        {
            Logger<ClientHandler>.LogError($"Error sending message to {Username}: {ex.Message}");
        }
    }

    public async Task HandleCommandAsync(string message)
    {
        try 
        {
            CommandName commandName = CommandHandler.GetCommandName(message);
            string[] parameters = CommandHandler.GetCommandParameters(message);
            var response = _commandHandler.ExecuteCommand(commandName, parameters);

            if (response != null) {
                string? recipient;
                switch (commandName)
                {
                    case CommandName.SendTo:
                        recipient = MessageParser.GetRecipient(message);
                        if (recipient == null)
                        {
                            await SendMessageAsync(new Message("Server", Username, response));
                            break;
                        }

                        _messageQueue.Add(new Message(Username, recipient, response));
                        break;
                    case CommandName.SendFile:
                        recipient = MessageParser.GetRecipient(message);
                        if (recipient == null)
                        {
                            await SendMessageAsync(new Message("Server", Username, response));
                            break;
                        }

                        string[] splitResponse = response.Split(' ');
                        string fileData = $"{splitResponse[0]} file={splitResponse[1]}";

                        _messageQueue.Add(new Message(Username, recipient, response));
                        break;
                    case CommandName.Exit:
                        await SendMessageAsync(new Message(Username, Username, response));
                        _messageQueue.Add(new Message(Username, "Server", "Disconnect"));
                        _cts.Cancel();
                        break;
                    default:
                        await SendMessageAsync(new Message("Server", Username, response));
                        break;
                }

            }
        }
        catch (Exception ex)
        {
            Logger<ClientHandler>.LogError($"Error handling command from {Username}: {ex.Message}");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _reader.Dispose();
            _writer.Dispose();
            _client.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
