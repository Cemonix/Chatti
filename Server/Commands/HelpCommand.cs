using System;
using Server.Interfaces;

namespace Server.Commands;

public class HelpCommand : ICommand
{
    public CommandName Name => CommandName.Help;
    public string Description => "Shows all available commands";
    public string Usage => "/help";
    private readonly Dictionary<CommandName, string> _commands = [];

    public HelpCommand(ICollection<ICommand> commands)
    {
        foreach (var command in commands)
        {
            _commands.Add(command.Name, command.Description);
        }
    }

    public string Execute(string[] parameters)
    {
        var helpText = "Available commands:\n";
        foreach (var cmd in _commands)
        {
            helpText += $"/{cmd.Key.ToString().ToLower()} - {cmd.Value}\n";
        }
        return helpText;
    }
}
