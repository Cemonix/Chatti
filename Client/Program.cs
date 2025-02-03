using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client("127.0.0.1", 8080);
            client.StartAsync().Wait();
        }
    }
}

