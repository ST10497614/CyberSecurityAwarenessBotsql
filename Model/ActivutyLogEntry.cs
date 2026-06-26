using System;

namespace CyberSecurityAwarenessBot.Models
{
    public class ActivityLogEntry
    {
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString()
            => $"[{Timestamp:HH:mm:ss}] {Description}";
    }
}