using System.Collections.Generic;

using System;
using System.Collections.Generic;

namespace CyberSecurityAwarenessBot.Models
{
    public class UserProfile
    {
        public string Name { get; set; } = string.Empty;
        public List<string> QuestionHistory { get; set; } = new List<string>();
        public List<ActivityLogEntry> ActivityLog { get; set; } = new List<ActivityLogEntry>();

        public void AddToHistory(string question)
        {
            QuestionHistory.Add(question);
        }

        public void LogAction(string description)
        {
            ActivityLog.Add(new ActivityLogEntry
            {
                Description = description,
                Timestamp = DateTime.Now
            });
        }

        // Returns last N entries (default 10)
        public List<ActivityLogEntry> GetRecentLog(int count = 10)
        {
            int start = Math.Max(0, ActivityLog.Count - count);
            return ActivityLog.GetRange(start, ActivityLog.Count - start);
        }
    }
}