using System;
using Server.Interfaces;

namespace Server.Commands;

public class ExitCommand : ICommand
{
    public CommandName Name => CommandName.Exit;
    public string Description => "Exits the server";
    public string Usage => "/exit";

    public string? Execute(string[] parameters)
    {
        return "Exiting server...";
    }
}
