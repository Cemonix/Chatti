using System;
using Server.Interfaces;

namespace Server.Commands;

public class SendToCommand : ICommand
{
    public CommandName Name => CommandName.SendTo;
    public string Description => "Send a message to a specific user";
    public string Usage => "/sendto <username> <message>";

    public static string GetRecipient(string[] parameters) {
        if (parameters.Length < 2)
            return string.Empty;
        return parameters[0];
    }

    public string? Execute(string[] parameters)
    {
        if (parameters.Length < 2)
            return "Invalid number of parameters";

        return string.Join(' ', parameters[1..]);
    }
}
