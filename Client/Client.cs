using System;
using System.Net.Sockets;

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
            var message = Console.ReadLine();
            if (message == null) break;
            await writer.WriteLineAsync(message);
        }
    }
}
