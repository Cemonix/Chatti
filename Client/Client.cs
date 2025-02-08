using System;
using System.Net.Sockets;
using Shared;

namespace Client;

public class Client(string host, int port)
{
    private readonly string _host = host;
    private readonly int _port = port;
    private readonly CancellationTokenSource _cts = new();

    public async Task StartAsync()
    {
        var client = new TcpClient();
        await client.ConnectAsync(_host, _port);

        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        _ = Task.Run(async () =>
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    var message = await reader.ReadLineAsync();
                    if (message == null)
                    {
                        Console.WriteLine("Server disconnected.");
                        _cts.Cancel();
                        break;
                    }

                    // TODO: Handle file transfer
                    // - parse the message - figure out how to differentiate between a message and a file
                    // - save the file to disk
                    Console.WriteLine(message);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Lost connection to server.");
                _cts.Cancel();
            }
        });

        while (!_cts.IsCancellationRequested)
        {
            // TODO: Readline escapes special characters - need to write own readline
            var message = Console.ReadLine();
            if (message == null) break;

            if (MessageParser.IsFileTransfer(message))
            {
                var recipient = MessageParser.GetRecipient(message);
                var filePath = MessageParser.GetFilePath(message);
                if (recipient == null)
                {
                    Console.WriteLine("Invalid recipient.");
                    continue;
                }
                
                if (filePath == null)
                {
                    Console.WriteLine("Invalid file path.");
                    continue;
                }

                Shared.File? file = Shared.File.Load(recipient, filePath);
                if (file == null)
                {
                    Console.WriteLine("File not found.");
                    continue;
                }

                await writer.WriteLineAsync(file.ToString());
                continue;
            }
            
            await writer.WriteLineAsync(message);
        }
    }

}
