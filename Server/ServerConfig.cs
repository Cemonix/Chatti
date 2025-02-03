using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Server;

public class ServerConfig
{
    public int Port { get; set; }
    public int MaxClients { get; set; }

    public static ServerConfig GetServerConfig()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serverConfig = config.GetSection("ServerConfig").Get<ServerConfig>() ?? 
            throw new InvalidOperationException("Server configuration is missing");

        return serverConfig;
    }
}
