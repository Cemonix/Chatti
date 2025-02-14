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
        if (IsFileTransferCommand(message))
        {
            string path = GetCommandParameters(message)[1];
            return path.Trim('"');
        }
        
        return null;
    }

    public static bool IsFileTransferCommand(string message)
    {
        if (!message.StartsWith("/sendfile"))
            return false;

        string[] parameters = GetCommandParameters(message);
        if (parameters.Length < 2)
        {
            Console.WriteLine("Invalid number of parameters. Usage: /sendfile <username> <file path>");
            return false;
        }

        return true;
    }

    public static bool IsFileTransfer(string message)
    {
        return message.Contains("file=");
    }

    public static string GetFileName(string message)
    {
        if (IsFileTransfer(message))
        {
            message = RemoveSender(message);
            return message.Split(' ')[0];
        }
        
        return string.Empty;
    }

    public static string GetFileData(string message)
    {
        if (IsFileTransfer(message))
        {
            string? fileData = message.Split("file=").Skip(1).FirstOrDefault();
            return fileData ?? string.Empty;
        }
        
        return string.Empty;
   }
}
