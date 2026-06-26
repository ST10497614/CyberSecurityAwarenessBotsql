using System;
using System.Collections.Generic;

namespace CyberSecurityAwarenessBot.UI
{
    public static class ConsoleUI
    {
        private const string BotName = "CyberBot";

        public static void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
 ██████╗██╗   ██╗██████╗ ███████╗██████╗ ██████╗  ██████╗ ████████╗
██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔══██╗██╔═══██╗╚══██╔══╝
██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝██████╔╝██║   ██║   ██║   
██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗██╔══██╗██║   ██║   ██║   
╚██████╗   ██║   ██████╔╝███████╗██║  ██║██████╔╝╚██████╔╝   ██║   
 ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚═════╝  ╚═════╝    ╚═╝  ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          [ CYBERSECURITY AWARENESS BOT  v2.0 ]               ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void WriteBotMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{BotName}] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }

        public static void WriteUserPrompt(string name)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{name}] > ");
            Console.ResetColor();
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[!] {message}");
            Console.ResetColor();
        }

        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[✓] {message}");
            Console.ResetColor();
        }

        public static void WriteHistory(List<string> history)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n──────────────────────────────────");
            Console.WriteLine("         QUESTION HISTORY          ");
            Console.WriteLine("──────────────────────────────────");
            if (history.Count == 0)
                Console.WriteLine("  No questions asked yet.");
            else
                for (int i = 0; i < history.Count; i++)
                    Console.WriteLine($"  {i + 1}. {history[i]}");
            Console.WriteLine("──────────────────────────────────\n");
            Console.ResetColor();
        }

        public static void WriteHelp()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n──────────────────────────────────");
            Console.WriteLine("            HELP TOPICS            ");
            Console.WriteLine("──────────────────────────────────");
            Console.ResetColor();
            WriteBotMessage("Task Management:");
            WriteBotMessage("  - add task [title]");
            WriteBotMessage("  - show tasks / view tasks");
            WriteBotMessage("  - complete task #1");
            WriteBotMessage("  - delete task #1");
            WriteBotMessage("Quiz:");
            WriteBotMessage("  - start quiz");
            WriteBotMessage("Activity Log:");
            WriteBotMessage("  - show activity log");
            WriteBotMessage("Cybersecurity Topics:");
            WriteBotMessage("  - password safety, phishing, safe browsing");
            WriteBotMessage("  - ransomware, data protection, 2FA");
            WriteBotMessage("Other Commands:");
            WriteBotMessage("  - history | help | exit");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("──────────────────────────────────\n");
            Console.ResetColor();
        }
    }
}