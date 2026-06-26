using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CyberSecurityAwarenessBot.Services
{
    public enum UserIntent
    {
        AddTask,
        ViewTasks,
        DeleteTask,
        CompleteTask,
        SetReminder,
        StartQuiz,
        ShowActivityLog,
        ShowHistory,
        Help,
        Exit,
        Unknown
    }

    public class NlpService
    {
        private readonly List<(UserIntent Intent, string[] Keywords)> _intentMap = new List<(UserIntent, string[])>
{
    (UserIntent.AddTask,         new string[] { "add task", "create task", "new task",
                                                "add a task", "remind me to", "set a task",
                                                "i need to" }),
    (UserIntent.SetReminder,     new string[] { "remind me in", "set reminder",
                                                "remind me on", "set a reminder",
                                                "remind me tomorrow", "reminder for" }),
    (UserIntent.ViewTasks,       new string[] { "view tasks", "show tasks", "my tasks",
                                                "list tasks", "what tasks", "all tasks",
                                                "show my tasks" }),
    (UserIntent.DeleteTask,      new string[] { "delete task", "remove task", "cancel task" }),
    (UserIntent.CompleteTask,    new string[] { "complete task", "mark done", "finished task",
                                                "task done", "mark complete", "mark as done" }),
    (UserIntent.StartQuiz,       new string[] { "start quiz", "take quiz", "quiz me",
                                                "play quiz", "test my knowledge",
                                                "cybersecurity quiz", "begin quiz" }),
    (UserIntent.ShowActivityLog, new string[] { "activity log", "show activity",
                                                "what have you done", "recent actions",
                                                "show log", "action log", "my log" }),
    (UserIntent.ShowHistory,     new string[] { "history", "my questions",
                                                "question history" }),
    (UserIntent.Help,            new string[] { "help", "what can you do",
                                                "topics", "commands" }),
    (UserIntent.Exit,            new string[] { "exit", "quit", "bye",
                                                "goodbye", "close" }),
};

        public UserIntent Detect(string input)
        {
            string lower = input.ToLower();
            foreach (var (intent, keywords) in _intentMap)
                foreach (var kw in keywords)
                    if (lower.Contains(kw))
                        return intent;
            return UserIntent.Unknown;
        }

        public int? ExtractDays(string input)
        {
            var match = Regex.Match(input, @"in\s+(\d+)\s+day", RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int days))
                return days;
            if (input.ToLower().Contains("tomorrow")) return 1;
            if (input.ToLower().Contains("next week")) return 7;
            return null;
        }

        public string ExtractTaskTitle(string input)
        {
            string[] prefixes =
            {
                "add task", "create task", "new task", "add a task",
                "remind me to", "set a task", "i need to",
                "add a reminder to", "add reminder to"
            };

            string lower = input.ToLower();
            foreach (var prefix in prefixes)
            {
                int idx = lower.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    string after = input.Substring(idx + prefix.Length).Trim(" -:".ToCharArray());
                    if (!string.IsNullOrWhiteSpace(after)) return after;
                }
            }
            return Regex.Replace(input,
                @"\b(please|can you|could you|i want to|i would like to)\b",
                "", RegexOptions.IgnoreCase).Trim();
        }
    }
}