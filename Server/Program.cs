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
                Logger<Program>.LogError($"Critical error: {ex.Message}");
                Environment.Exit(1);
            }
            
            // TODO: Client disconnection handling
            // TODO: Group chat
            // TODO: Message end-to-end encryption
            // TODO: File transfer
            // TODO: Voice chat
        }
    }
}