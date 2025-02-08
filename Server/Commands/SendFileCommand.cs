using System;
using Server.Interfaces;

namespace Server.Commands;

public class SendFileCommand : ICommand
{
    public CommandName Name => CommandName.SendFile;
    public string Description => "Send a file to a specific user";
    public string Usage => "/sendfile <username> <file path>";

    public string? Execute(string[] parameters)
    {
        if (parameters.Length < 2)
            return $"Invalid number of parameters. Usage: {Usage}";

        return string.Join(' ', parameters[1..]);
    }
}
