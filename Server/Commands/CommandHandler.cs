using System;
using Shared;
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

    public static CommandName GetCommandName(string command)
    {
        if (!MessageParser.IsCommand(command))
            return CommandName.Unknown;

        string[] commandContent = MessageParser.Parse(command);
        string commandName = RemoveCommandPrefix(commandContent[0]);
        return Enum.TryParse(commandName, true, out CommandName result) ? result : CommandName.Unknown;
    }

    public static string[] GetCommandParameters(string command)
    {
        var commandContent = MessageParser.Parse(command);
        return MessageParser.IsCommand(commandContent[0]) ? [.. commandContent.Skip(1)] : [];
    }

    public string? ExecuteCommand(CommandName commandName, string[] parameters)
    {   
        if (!_commands.TryGetValue(commandName, out var command))
            return $"Unknown command '{commandName}'. Type /help for available commands.";

        return command.Execute(parameters);
 
    }

    private static string RemoveCommandPrefix(string message)
    {
        return MessageParser.IsCommand(message) ? message[1..] : message;
    }
}
