using System;
using Server.Commands;
using Server.Interfaces;

namespace Server;

public class CommandHandler
{
    private readonly Dictionary<CommandName, ICommand> _commands = [];

    public void RegisterCommand(ICommand command)
    {
        _commands.Add(command.Name, command);
    }

    public ICollection<ICommand> GetCommands() => _commands.Values;

    public static bool IsCommand(string message)
    {
        var content = ParseMessage(message);
        return content.StartsWith('/');
    }

    public static CommandName GetCommandName(string message)
    {
        var content = ParseMessage(message);
        content = RemoveCommandPrefix(content);
        return Enum.TryParse(content.Split(' ')[0], true, out CommandName commandName) ? commandName : CommandName.Unknown;
    }

    public static string[] GetCommandParameters(string message)
    {
        var content = ParseMessage(message);
        return content.StartsWith('/') ? [.. content.Split(' ').Skip(1)] : [];
    }

    public string? ExecuteCommand(CommandName commandName, string[] parameters)
    {   
        if (!_commands.TryGetValue(commandName, out var command))
            return $"Unknown command '{commandName}'. Type /help for available commands.";

        return command.Execute(parameters);
 
    }

    private static string ParseMessage(string message)
    {
        string[] parts = message.Split(": ", 2);
        return parts.Length < 2 ? message : parts[1];
    }

    private static string RemoveCommandPrefix(string message)
    {
        return message.StartsWith('/') ? message[1..] : message;
    }
}
