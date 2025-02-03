using System;

namespace Server;

public class Message(string sender, string recipient, string content, DateTime timestamp)
{
    public string Sender { get; } = sender;
    public string Recipient { get; } = recipient;
    public string Content { get; } = content;
    public DateTime Timestamp { get; } = timestamp;
}
