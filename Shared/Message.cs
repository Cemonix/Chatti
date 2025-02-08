using System;

namespace Shared;

public record Message(string Sender, string Recipient, string Content)
{
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] {Sender}: {Content}";
    }
}
