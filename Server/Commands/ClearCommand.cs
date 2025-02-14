using System;
using Server.Interfaces;
using Shared;

namespace Server.Commands;

public class ClearCommand : ICommand
{
    public CommandName Name => CommandName.Clear;
    public string Description => "Clears the chat console";
    public string Usage => "/clear";

    public string? Execute(string[] parameters)
    {
        try
        {
            Console.Clear();
        }
        catch (Exception ex)
        {
#if DEBUG
            Logger<ClearCommand>.LogDebug($"{ex.Message}");
#endif
            return "Failed to clear chat";
        }

        return null;
    }
}
