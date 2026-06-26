using CyberSecurityAwarenessBot.Models;
using CyberSecurityAwarenessBot.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityAwarenessBot.Services
{
    public class ChatbotService
    {
        private readonly Dictionary<string[], string> _responses = new Dictionary<string[], string>
{
    {
        new string[] { "how are you", "how r u", "how are u" },
        "I'm running smoothly and all security protocols are active! 😊"
    },
    {
        new string[] { "purpose", "what do you do", "why are you here", "what are you" },
        "My purpose is to educate you about cybersecurity threats and best practices."
    },
    {
        new string[] { "password", "passwords", "password safety" },
        "🔐 Password Safety:\n" +
        "  • Use 12+ characters with letters, numbers and symbols.\n" +
        "  • Never reuse passwords across different sites.\n" +
        "  • Use a password manager like Bitwarden or 1Password.\n" +
        "  • Avoid personal info like birthdays or names."
    },
    {
        new string[] { "phishing", "phishing email", "phishing attack" },
        "🎣 Phishing:\n" +
        "  • Attackers impersonate trusted sources to steal credentials.\n" +
        "  • Always verify the sender's email address.\n" +
        "  • Never click suspicious links — hover first to preview the URL.\n" +
        "  • Legitimate organisations never ask for your password via email."
    },
    {
        new string[] { "browse safely", "safe browsing", "browsing", "internet safety" },
        "🌐 Safe Browsing:\n" +
        "  • Look for HTTPS and a padlock in the address bar.\n" +
        "  • Avoid downloading files from untrusted websites.\n" +
        "  • Use a VPN on public Wi-Fi networks.\n" +
        "  • Keep your browser updated."
    },
    {
        new string[] { "ransomware", "ransom" },
        "🦠 Ransomware:\n" +
        "  • Malware that encrypts your files and demands payment.\n" +
        "  • Always keep offline or cloud backups.\n" +
        "  • Never open attachments from unknown senders.\n" +
        "  • Keep your OS and software up to date."
    },
    {
        new string[] { "protect my data", "data protection", "data privacy" },
        "🛡️ Data Protection:\n" +
        "  • Only share info on trusted HTTPS sites.\n" +
        "  • Review app permissions regularly.\n" +
        "  • Enable full-disk encryption on your devices.\n" +
        "  • Be cautious about what you share on social media."
    },
    {
        new string[] { "two-factor", "2fa", "two factor", "multi-factor", "mfa" },
        "🔑 Two-Factor Authentication (2FA):\n" +
        "  • Adds a second layer of security beyond your password.\n" +
        "  • Even if your password is stolen, attackers cannot get in.\n" +
        "  • Use an authenticator app over SMS when possible.\n" +
        "  • Enable 2FA on email and banking accounts."
    },
};
        public string GetBotResponse(string input)
        {
            string lower = input.ToLower();
            foreach (var entry in _responses)
                if (entry.Key.Any(k => lower.Contains(k)))
                    return entry.Value;

            return "I'm not sure about that. Type 'help' for a list of topics I can help with.";
        }

        // Console-mode chat loop (kept for backwards compatibility)
        public void StartChat(UserProfile user)
        {
            Console.WriteLine();
            while (true)
            {
                ConsoleUI.WriteUserPrompt(user.Name);
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) { ConsoleUI.WriteError("Please type something."); continue; }
                string t = input.Trim();
                if (t.Equals("exit", StringComparison.OrdinalIgnoreCase)) { ConsoleUI.WriteBotMessage($"Goodbye, {user.Name}! 👋"); break; }
                if (t.Equals("history", StringComparison.OrdinalIgnoreCase)) { ConsoleUI.WriteHistory(user.QuestionHistory); continue; }
                if (t.Equals("help", StringComparison.OrdinalIgnoreCase)) { ConsoleUI.WriteHelp(); continue; }
                user.AddToHistory(t);
                ConsoleUI.WriteBotMessage(GetBotResponse(t));
                Console.WriteLine();
            }
        }
    }
}