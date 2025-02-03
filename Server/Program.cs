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
            
            // TODO: Figure out main loop for server with other threads
            // TODO: Client disconnection handling
            // TODO: Client to client communication
            //       - figure out how to specify the recipient of a message
            // TODO: Group chat
            // TODO: Message end-to-end encryption
            // TODO: File transfer
            // TODO: Voice chat
        }
    }
}