using System;
using Server.Interfaces;

namespace Server.Commands;

public class SendToCommand : ICommand
{
    public CommandName Name => CommandName.SendTo;
    public string Description => "Send a message to a specific user";
    public string Usage => "/sendto <username> <message>";

    public string? Execute(string[] parameters)
    {
        if (parameters.Length < 2)
            return $"Invalid number of parameters. Usage: {Usage}";

        return string.Join(' ', parameters[1..]);
    }
}
