using System;
using Server.Interfaces;

namespace Server.Commands;

public class UsersCountCommand : ICommand
{
    public CommandName Name => CommandName.Users;
    public string Description => "Shows number of connected users";
    public string Usage => "/users";
    private readonly Func<int> _connectedClientsCount;

    public UsersCountCommand(Func<int> connectedClientsCount)
    {
        ArgumentNullException.ThrowIfNull(connectedClientsCount);
        _connectedClientsCount = connectedClientsCount;
    }

    public string Execute(string[] parameters)
    {
        return $"Connected users: {_connectedClientsCount?.Invoke()}";
    }
}
