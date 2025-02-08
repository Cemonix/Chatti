namespace Shared;

public static class MessageParser
{
    public static string[] Parse(string message)
    {
        return message.Split(' ');
    }

    public static bool IsCommand(string command)
    {
        return command.StartsWith('/');
    }

    public static string[] GetCommandParameters(string command)
    {
        return IsCommand(command) ? [.. command.Split(' ').Skip(1)] : [];
    }

    public static string RemoveSender(string message)
    {
        return string.Join(' ', message.Split(": ").Skip(1));
    }

    public static string GetRecipient(string message)
    {
        return GetCommandParameters(message)[0];
    }

    public static string? GetFilePath(string message)
    {
        if (IsFileTransfer(message))
            return GetCommandParameters(message)[1];
        
        return null;
    }

    public static bool IsFileTransfer(string message)
    {
        if (message.StartsWith("/sendfile"))
        {
            string[] parameters = GetCommandParameters(message);
            if (parameters.Length < 2)
            {
                Console.WriteLine("Invalid number of parameters. Usage: /sendfile <username> <file path>");
                return false;
            }

            return true;
        }

        return false;
    }
}
