using System;

namespace Server
{   
    class Program
    {
        static async Task Main(string[] args)
        {   
            try
            {
                using var server = new Server(ServerConfig.GetServerConfig());
                await server.StartAsync();
            }
            catch (Exception ex)
            {
                Shared.Logger<Program>.LogError($"Critical error: {ex.Message}");
                Environment.Exit(1);
            }
            
            // TODO: File transfer
            // TODO: Group chat
            // TODO: Message end-to-end encryption
            // TODO: Voice chat
        }
    }
}