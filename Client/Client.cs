using System;
using System.Net.Sockets;
using Shared;

namespace Client;

public class Client(string host, int port)
{
    private readonly string _host = host;
    private readonly int _port = port;
    private readonly CancellationTokenSource _cts = new();
    private readonly string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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

                    if (MessageParser.IsFileTransfer(message))
                    {
                        string fileData = MessageParser.GetFileData(message);
                        if (fileData == string.Empty)
                        {
                            Console.WriteLine("Invalid file data received.");
                            continue;
                        }

                        string fileName = MessageParser.GetFileName(message);
                        if (fileName == string.Empty)
                        {
                            Console.WriteLine("Invalid file name received.");
                            continue;
                        }

                        string filePath = Path.Combine(homeDirectory, fileName);
                        await System.IO.File.WriteAllBytesAsync(filePath, Convert.FromBase64String(fileData));
                        Console.WriteLine($"File {fileName} received and saved to home directory.");
                        continue;
                    }

                    Console.WriteLine(message);
                }
            }
            catch (IOException ex)
            {
#if DEBUG
                Logger<Client>.LogInfo($"IO exception: {ex.Message}");
#endif
                Console.WriteLine("Lost connection to server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _cts.Cancel();
            }
        });

        while (!_cts.IsCancellationRequested)
        {
            var message = Console.ReadLine();
            if (message == null) break;

            if (MessageParser.IsFileTransferCommand(message))
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
