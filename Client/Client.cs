using System;
using System.Net.Sockets;

namespace Client;

public class Client(string host, int port)
{
    private readonly string _host = host;
    private readonly int _port = port;

    public async Task StartAsync()
    {
        var client = new TcpClient();
        await client.ConnectAsync(_host, _port);

        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        _ = Task.Run(async () =>
        {
            while (true)
            {
                var message = await reader.ReadLineAsync();
                if (message == null) break;
                Console.WriteLine(message);
            }
        });

        while (true)
        {
            var message = Console.ReadLine();
            if (message == null) break;
            await writer.WriteLineAsync(message);
        }
    }
}
