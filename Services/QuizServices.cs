using System.Collections.Generic;

namespace CyberSecurityAwarenessBot.Services
{
    public class QuizQuestion
    {
        public string Question { get; set; } = "";
        public string[] Options { get; set; } = new string[0];
        public int AnswerIndex { get; set; }
        public string Explanation { get; set; } = "";
        public bool IsTrueFalse
            => Options.Length == 2
               && Options[0] == "True"
               && Options[1] == "False";
    }

    public class QuizService
    {
        private int _currentIndex = 0;

        public int Score { get; private set; } = 0;
        public bool IsActive { get; private set; } = false;
        public bool IsFinished => _currentIndex >= _questions.Count;
        public int TotalQuestions => _questions.Count;
        public int CurrentIndex => _currentIndex;

        private readonly List<QuizQuestion> _questions = new List<QuizQuestion>
        {
            new QuizQuestion
            {
                Question    = "What should you do if you receive an email asking for your password?",
                Options     = new string[] {
                                "A) Reply with your password",
                                "B) Delete the email",
                                "C) Report it as phishing",
                                "D) Ignore it" },
                AnswerIndex = 2,
                Explanation = "Reporting phishing emails helps prevent scams and protects others."
            },
            new QuizQuestion
            {
                Question    = "True or False: Using the same password for multiple accounts is safe.",
                Options     = new string[] { "True", "False" },
                AnswerIndex = 1,
                Explanation = "If one account is breached, all accounts sharing that password are at risk."
            },
            new QuizQuestion
            {
                Question    = "What does HTTPS indicate about a website?",
                Options     = new string[] {
                                "A) It is fast",
                                "B) It is encrypted and secure",
                                "C) It is government-run",
                                "D) It has no ads" },
                AnswerIndex = 1,
                Explanation = "HTTPS means the connection is encrypted, protecting data in transit."
            },
            new QuizQuestion
            {
                Question    = "True or False: Public Wi-Fi is always safe to use for banking.",
                Options     = new string[] { "True", "False" },
                AnswerIndex = 1,
                Explanation = "Public Wi-Fi is unencrypted. Always use a VPN for sensitive transactions."
            },
            new QuizQuestion
            {
                Question    = "What is two-factor authentication (2FA)?",
                Options     = new string[] {
                                "A) Two passwords",
                                "B) A backup email",
                                "C) A second verification step",
                                "D) Antivirus software" },
                AnswerIndex = 2,
                Explanation = "2FA requires a second proof of identity, making accounts much harder to hack."
            },
            new QuizQuestion
            {
                Question    = "Which of these is the strongest password?",
                Options     = new string[] {
                                "A) password123",
                                "B) John1990",
                                "C) !Xk9#mQ2@pL",
                                "D) qwerty" },
                AnswerIndex = 2,
                Explanation = "Strong passwords mix upper/lowercase, numbers, and symbols with 12+ characters."
            },
            new QuizQuestion
            {
                Question    = "True or False: Antivirus software alone fully protects you from all cyber threats.",
                Options     = new string[] { "True", "False" },
                AnswerIndex = 1,
                Explanation = "Antivirus is one layer. Safe habits, updates, and 2FA are equally important."
            },
            new QuizQuestion
            {
                Question    = "What is phishing?",
                Options     = new string[] {
                                "A) A sport",
                                "B) Hacking via brute force",
                                "C) Tricking users into revealing data",
                                "D) A type of malware" },
                AnswerIndex = 2,
                Explanation = "Phishing uses deceptive messages to steal credentials or personal information."
            },
            new QuizQuestion
            {
                Question    = "What should you do before clicking a link in an email?",
                Options     = new string[] {
                                "A) Click it immediately",
                                "B) Hover over it to preview the URL",
                                "C) Forward it to friends",
                                "D) Print the email" },
                AnswerIndex = 1,
                Explanation = "Hovering reveals the real destination URL before you commit to clicking."
            },
            new QuizQuestion
            {
                Question    = "True or False: Software updates should be delayed as long as possible.",
                Options     = new string[] { "True", "False" },
                AnswerIndex = 1,
                Explanation = "Updates patch known security vulnerabilities — install them promptly."
            },
            new QuizQuestion
            {
                Question    = "What is ransomware?",
                Options     = new string[] {
                                "A) Free software",
                                "B) Malware that encrypts files for ransom",
                                "C) A firewall tool",
                                "D) A VPN service" },
                AnswerIndex = 1,
                Explanation = "Ransomware encrypts your files and demands payment. Backups are your best defence."
            },
            new QuizQuestion
            {
                Question    = "Which action best protects your data on a lost phone?",
                Options     = new string[] {
                                "A) Hope nobody finds it",
                                "B) Call the phone",
                                "C) Full-disk encryption and remote wipe",
                                "D) Change your wallpaper" },
                AnswerIndex = 2,
                Explanation = "Encryption and remote wipe prevent unauthorised access to data on lost devices."
            },
        };

        public void Start()
        {
            _currentIndex = 0;
            Score = 0;
            IsActive = true;
        }

        public void Stop() { IsActive = false; }

        public QuizQuestion CurrentQuestion()
        {
            return _currentIndex < _questions.Count ? _questions[_currentIndex] : null;
        }

        public KeyValuePair<bool, string> Answer(int choiceIndex)
        {
            var q = _questions[_currentIndex];
            bool correct = choiceIndex == q.AnswerIndex;
            if (correct) Score++;
            _currentIndex++;
            return new KeyValuePair<bool, string>(correct, q.Explanation);
        }

        public string FinalFeedback()
        {
            double pct = (double)Score / TotalQuestions * 100;
            if (pct >= 80)
                return "🏆 You scored " + Score + "/" + TotalQuestions + " — Great job! You're a cybersecurity pro!";
            else if (pct >= 50)
                return "👍 You scored " + Score + "/" + TotalQuestions + " — Good effort! Keep learning to stay safe online!";
            else
                return "📚 You scored " + Score + "/" + TotalQuestions + " — Keep learning to stay safe online!";
        }
    }
}
