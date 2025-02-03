using System;
using Server.Interfaces;

namespace Server.Commands;

public class UsersCommand(ConnectedClientsDelegate connectedClients) : ICommand
{
    public CommandName Name => CommandName.Users;
    public string Description => "Shows number of connected users";
    public string Usage => "/users";
    private readonly ConnectedClientsDelegate _connectedClients = connectedClients;

    public string Execute(string[] parameters)
    {
        return $"Connected users: {_connectedClients()}";
    }
}
