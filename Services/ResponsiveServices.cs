using System;
using System.Collections.Generic;

namespace CyberSecurityAwarenessBot.Services
{
    // ── Requirement 8: OOP - each topic is a structured object ───
    public class CyberTopic
    {
        public string Name { get; set; }
        public string[] Keywords { get; set; }
        public string[] Responses { get; set; }
        public string[] MoreResponses { get; set; }

        public CyberTopic(string name, string[] keywords, string[] responses, string[] moreResponses)
        {
            Name = name;
            Keywords = keywords;
            Responses = responses;
            MoreResponses = moreResponses;
        }
    }

    public class ResponseService
    {
        // ── Requirement 8: Use arrays/lists for organised storage ─
        private readonly List<CyberTopic> _topics;
        private readonly Random _random = new Random();

        // ── Requirement 4 & 5: Track conversation context/memory ─
        private string _lastTopic = null;
        private string _favouriteTopic = null;

        public ResponseService()
        {
            // ── Requirement 2 & 3: Keywords + random responses ────
            _topics = new List<CyberTopic>
            {
                new CyberTopic(
                    "password",
                    new[] { "password", "passwords", "passphrase", "credential" },
                    new[]
                    {
                        "Use strong passwords with a mix of uppercase, lowercase, numbers, and symbols. Never reuse passwords across accounts.",
                        "A good password is at least 12 characters long. Consider using a passphrase like 'Purple!Monkey$Dishwasher9'.",
                        "Never share your password with anyone, including IT support. Legitimate services will never ask for it.",
                        "Use a password manager to store and generate strong, unique passwords for every account."
                    },
                    new[]
                    {
                        "Also enable two-factor authentication alongside your strong password for maximum security.",
                        "Change your passwords regularly, especially after a data breach. Check haveibeenpwned.com to see if your email was exposed.",
                        "Avoid using personal details like birthdays or pet names — these are the first things attackers guess."
                    }
                ),
                new CyberTopic(
                    "phishing",
                    new[] { "phishing", "phish", "fake email", "suspicious email", "email scam" },
                    new[]
                    {
                        "Phishing is when attackers pretend to be trusted organisations to steal your information. Always verify the sender's email address carefully.",
                        "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                        "Phishing emails often create a sense of urgency — 'Your account will be closed!' Stay calm and verify before clicking anything.",
                        "Legitimate banks and services will never ask for your password or OTP via email. If in doubt, contact the company directly."
                    },
                    new[]
                    {
                        "Always hover over links in emails before clicking to see the real destination URL.",
                        "Check for spelling mistakes and odd sender addresses — phishing emails often have subtle errors.",
                        "Report phishing emails to your IT department or email provider to help protect others."
                    }
                ),
                new CyberTopic(
                    "scam",
                    new[] { "scam", "scams", "fraud", "con", "trick", "deceive", "fake" },
                    new[]
                    {
                        "Online scams often create urgency or fear. Be cautious of messages asking for money, OTPs, passwords, or banking details.",
                        "If an offer sounds too good to be true, it almost certainly is. Verify before you trust.",
                        "Romance scams, lottery scams, and tech support scams are among the most common. Never send money to someone you haven't met.",
                        "Scammers exploit emotions — fear, greed, and loneliness. Take a breath and think critically before acting."
                    },
                    new[]
                    {
                        "Report scams to the South African Police Service (SAPS) or the South African Banking Risk Information Centre (SABRIC).",
                        "Warn friends and family about scams — sharing awareness is one of the best defences.",
                        "Never click links in unsolicited SMS messages. Go directly to the official website instead."
                    }
                ),
                new CyberTopic(
                    "privacy",
                    new[] { "privacy", "private", "personal data", "data protection", "personal information" },
                    new[]
                    {
                        "Protect your privacy by reviewing app permissions — only grant access to what is truly necessary.",
                        "Use a VPN on public Wi-Fi to encrypt your traffic and keep your browsing private.",
                        "Regularly review your social media privacy settings. Limit who can see your posts and personal details.",
                        "Be mindful of what you share online — even seemingly harmless details can be used by attackers to target you."
                    },
                    new[]
                    {
                        "Under POPIA (Protection of Personal Information Act), South African organisations must protect your data. You have the right to request what data they hold.",
                        "Delete accounts and apps you no longer use — they are potential data leak sources.",
                        "Use private/incognito browsing when using shared computers to avoid leaving traces."
                    }
                ),
                new CyberTopic(
                    "safe browsing",
                    new[] { "browsing", "browse", "safe browsing", "website", "internet", "online" },
                    new[]
                    {
                        "Only visit trusted websites. Always check for HTTPS in the address bar before entering personal information.",
                        "Keep your browser and its extensions updated to patch known security vulnerabilities.",
                        "Avoid clicking pop-up ads or downloading software from untrusted websites.",
                        "Use a reputable ad blocker to reduce exposure to malicious advertisements."
                    },
                    new[]
                    {
                        "Be cautious on public Wi-Fi — avoid accessing banking or sensitive accounts without a VPN.",
                        "Clear your browser cookies and cache regularly to remove tracking data.",
                        "Install browser extensions like HTTPS Everywhere to enforce secure connections automatically."
                    }
                ),
                new CyberTopic(
                    "malware",
                    new[] { "malware", "virus", "trojan", "spyware", "adware", "worm", "ransomware", "infected" },
                    new[]
                    {
                        "Malware is harmful software designed to damage or gain unauthorised access to your system. Install reputable antivirus software and keep it updated.",
                        "Never open email attachments from unknown senders — they are a common malware delivery method.",
                        "Ransomware encrypts your files and demands payment. Back up your data regularly to an offline or cloud location.",
                        "Download software only from official sources. Avoid cracked or pirated software — it often contains hidden malware."
                    },
                    new[]
                    {
                        "If you suspect malware, disconnect from the internet immediately and run a full antivirus scan.",
                        "Keep your operating system updated — many updates patch vulnerabilities that malware exploits.",
                        "Consider using Windows Defender or Malwarebytes as a free baseline protection tool."
                    }
                ),
                new CyberTopic(
                    "two-factor authentication",
                    new[] { "two-factor", "2fa", "two factor", "multi-factor", "mfa", "otp", "authenticator" },
                    new[]
                    {
                        "Two-factor authentication (2FA) adds a second layer of security beyond your password. Even if your password is stolen, attackers cannot log in without the second factor.",
                        "Use an authenticator app like Google Authenticator or Microsoft Authenticator rather than SMS codes — they are more secure.",
                        "Enable 2FA on all important accounts: email, banking, and social media especially.",
                        "Never share your OTP with anyone, even someone claiming to be from your bank or IT support."
                    },
                    new[]
                    {
                        "Hardware security keys like YubiKey offer the strongest form of 2FA available.",
                        "If a site does not support 2FA, consider whether it is safe to store sensitive information there.",
                        "Backup codes are provided when you set up 2FA — store them safely in case you lose your device."
                    }
                ),
                new CyberTopic(
                    "data protection",
                    new[] { "data", "protect", "protection", "backup", "encrypt", "encryption" },
                    new[]
                    {
                        "Back up your data regularly using the 3-2-1 rule: 3 copies, on 2 different media, with 1 offsite.",
                        "Encrypt sensitive files before storing or sharing them. Tools like VeraCrypt make this straightforward.",
                        "Avoid storing sensitive data like ID numbers or banking details in unencrypted files or emails.",
                        "Use cloud services with end-to-end encryption for storing confidential information."
                    },
                    new[]
                    {
                        "Shred physical documents containing personal information rather than throwing them away.",
                        "Secure your devices with a PIN, password, or biometric lock to protect data if lost or stolen.",
                        "Review which third-party apps have access to your Google or Microsoft account and revoke unused permissions."
                    }
                ),
                new CyberTopic(
                    "suspicious links",
                    new[] { "link", "links", "url", "suspicious link", "click", "hyperlink" },
                    new[]
                    {
                        "Before clicking a link, hover over it to inspect the real URL. If it looks strange or unrelated to the sender, do not click it.",
                        "Shortened URLs like bit.ly can hide malicious destinations. Use a URL expander tool to preview them first.",
                        "Attackers register domains that look similar to real ones, e.g. 'g00gle.com'. Always check spelling carefully.",
                        "When in doubt, go directly to the website by typing the address manually rather than clicking a link."
                    },
                    new[]
                    {
                        "Use Google Safe Browsing or VirusTotal to check if a URL is flagged as malicious before visiting.",
                        "Never click links in unsolicited messages, even from people you know — their account may have been compromised.",
                        "QR codes can also lead to malicious URLs. Only scan codes from trusted, physical sources."
                    }
                )
            };
        }

        // ═════════════════════════════════════════════════════════
        //  MAIN RESPONSE METHOD
        // ═════════════════════════════════════════════════════════
        public string GetResponse(string userInput, string userName)
        {
            // ── Requirement 7: Edge case — empty input ────────────
            if (string.IsNullOrWhiteSpace(userInput))
                return "I'm not sure I understand. Can you try rephrasing?";

            string input = userInput.ToLower().Trim();

            // ── Built-in commands ─────────────────────────────────
            if (input == "exit")
                return "Goodbye, " + userName + ". Stay safe online!";

            // ── Requirement 5: Memory — favourite topic ───────────
            if (input.Contains("interested in") || input.Contains("i like") || input.Contains("favourite topic"))
            {
                foreach (CyberTopic topic in _topics)
                {
                    foreach (string keyword in topic.Keywords)
                    {
                        if (input.Contains(keyword))
                        {
                            _favouriteTopic = topic.Name;
                            _lastTopic = topic.Name;
                            return "Great! I'll remember that you're interested in " + topic.Name +
                                   ". It's a crucial part of staying safe online. " +
                                   GetRandomResponse(topic);
                        }
                    }
                }
            }

            // ── Requirement 4: Conversation flow — follow-up ──────
            if (_lastTopic != null && (
                input.Contains("tell me more") ||
                input.Contains("explain more") ||
                input.Contains("give me another tip") ||
                input.Contains("more info") ||
                input.Contains("another tip") ||
                input.Contains("more") ||
                input.Contains("elaborate")))
            {
                CyberTopic lastTopicObj = FindTopicByName(_lastTopic);
                if (lastTopicObj != null)
                {
                    return "Here is another tip about " + _lastTopic + ": " +
                           GetRandomMoreResponse(lastTopicObj);
                }
            }

            // ── Requirement 6: Sentiment detection ───────────────
            string sentimentPrefix = DetectSentiment(input);

            // ── Requirement 2: Keyword recognition ───────────────
            foreach (CyberTopic topic in _topics)
            {
                foreach (string keyword in topic.Keywords)
                {
                    if (input.Contains(keyword))
                    {
                        _lastTopic = topic.Name;

                        // ── Req 5: Personalise if favourite topic ─
                        string personalisation = "";
                        if (_favouriteTopic != null && _favouriteTopic == topic.Name)
                            personalisation = "As someone interested in " + topic.Name + ", " + userName + ", here is something important: ";

                        // ── Req 3: Random response selection ──────
                        return sentimentPrefix + personalisation + GetRandomResponse(topic);
                    }
                }
            }

            // ── General responses ─────────────────────────────────
            if (input.Contains("how are you"))
                return "I am doing well, " + userName + ". Thank you for asking! I am ready to help you with cybersecurity awareness.";

            if (input.Contains("what is your purpose") || input.Contains("what's your purpose"))
                return "My purpose is to educate users about cybersecurity risks and help them stay safe online.";

            if (input.Contains("what can i ask") || input.Contains("what can you do") || input.Contains("help"))
                return "You can ask me about: password safety, phishing, scams, privacy, safe browsing, malware, two-factor authentication, data protection, and suspicious links.";

            // ── Requirement 5: Recall favourite topic ─────────────
            if (_favouriteTopic != null && (input.Contains("tip") || input.Contains("advice")))
            {
                CyberTopic favTopic = FindTopicByName(_favouriteTopic);
                if (favTopic != null)
                    return "As someone interested in " + _favouriteTopic + ", you might want to know: " + GetRandomResponse(favTopic);
            }

            // ── Requirement 7: Default unknown input response ──────
            return sentimentPrefix + "I'm not sure I understand that. Can you try rephrasing? " +
                   "You can ask me about passwords, phishing, scams, privacy, or safe browsing. Type 'help' to see all topics.";
        }

        // ═════════════════════════════════════════════════════════
        //  REQUIREMENT 6: SENTIMENT DETECTION
        // ═════════════════════════════════════════════════════════
        private string DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("afraid") || input.Contains("fear"))
                return "It's completely understandable to feel that way. Cybersecurity can feel overwhelming, but you're taking the right steps by learning. ";

            if (input.Contains("confused") || input.Contains("don't understand") || input.Contains("dont understand") || input.Contains("lost"))
                return "No worries at all — cybersecurity can be complex. Let me try to make this as clear as possible. ";

            if (input.Contains("frustrated") || input.Contains("annoyed") || input.Contains("angry"))
                return "I understand your frustration. Cybersecurity challenges can be irritating, but I am here to help. ";

            if (input.Contains("curious") || input.Contains("interested") || input.Contains("want to know") || input.Contains("tell me about"))
                return "Great question — curiosity is the first step to staying safe! ";

            if (input.Contains("help") || input.Contains("not sure") || input.Contains("unsure"))
                return "No problem, I am here to help you. ";

            return "";
        }

        // ═════════════════════════════════════════════════════════
        //  HELPERS
        // ═════════════════════════════════════════════════════════

        // Requirement 3: Randomly select from a list of responses
        private string GetRandomResponse(CyberTopic topic)
        {
            return topic.Responses[_random.Next(topic.Responses.Length)];
        }

        private string GetRandomMoreResponse(CyberTopic topic)
        {
            return topic.MoreResponses[_random.Next(topic.MoreResponses.Length)];
        }

        private CyberTopic FindTopicByName(string name)
        {
            foreach (CyberTopic topic in _topics)
            {
                if (topic.Name == name)
                    return topic;
            }
            return null;
        }

        // Expose last topic for external use if needed
        public string GetLastTopic()
        {
            return _lastTopic;
        }

        public string GetFavouriteTopic()
        {
            return _favouriteTopic;
        }
    }
}