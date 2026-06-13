using System;
using System.Collections.Generic;

namespace CyberSecurityChatbotWPF.Services
{
    public class QuizManager
    {
        private List<QuizQuestion> questions;
        private int currentQuestionIndex;
        private int score;

        public QuizManager()
        {
            LoadQuestions();
            currentQuestionIndex = 0;
            score = 0;
        }

        private void LoadQuestions()
        {
            questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What does HTTPS stand for?",
                    Options = new List<string> { "HyperText Transfer Protocol Secure", "High Transfer Text Protocol Secure", "Hyper Transfer Text Protocol System", "Hidden Text Transfer Protocol Secure" },
                    CorrectAnswerIndex = 0,
                    Explanation = "HTTPS adds encryption to HTTP, keeping your data safe."
                },
                new QuizQuestion
                {
                    Question = "Which of these is a strong password?",
                    Options = new List<string> { "password123", "qwerty", "MyD0g!sFluffy#2024", "admin" },
                    CorrectAnswerIndex = 2,
                    Explanation = "A strong password has uppercase, lowercase, numbers, and symbols."
                },
                new QuizQuestion
                {
                    Question = "What is phishing?",
                    Options = new List<string> { "A type of computer virus", "Fake emails/websites that steal your info", "A fishing technique", "A password manager" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Phishing tricks you into giving away personal information."
                },
                new QuizQuestion
                {
                    Question = "True or False: You should use the same password for all accounts.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Using the same password puts all your accounts at risk."
                },
                new QuizQuestion
                {
                    Question = "What is two-factor authentication (2FA)?",
                    Options = new List<string> { "Having two passwords", "A second verification step", "Two usernames", "A type of antivirus" },
                    CorrectAnswerIndex = 1,
                    Explanation = "2FA adds an extra layer of security beyond just your password."
                },
                new QuizQuestion
                {
                    Question = "What should you do if you get a suspicious email?",
                    Options = new List<string> { "Click the link to check", "Reply asking if it's real", "Report it and delete it", "Forward to friends" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Never click suspicious links. Report and delete instead."
                },
                new QuizQuestion
                {
                    Question = "Which of these is NOT a safe browsing practice?",
                    Options = new List<string> { "Using HTTPS websites", "Clicking pop-up ads", "Using a VPN on public WiFi", "Clearing browser cookies" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Pop-up ads are often malicious and should never be clicked."
                },
                new QuizQuestion
                {
                    Question = "What is ransomware?",
                    Options = new List<string> { "Software that holds your files hostage", "A type of antivirus", "A password manager", "A firewall" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Ransomware locks your files and demands payment to unlock them."
                },
                new QuizQuestion
                {
                    Question = "True or False: Public WiFi is always safe to use.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Public WiFi can be intercepted. Always use a VPN."
                },
                new QuizQuestion
                {
                    Question = "What is a VPN used for?",
                    Options = new List<string> { "To make internet faster", "To hide your IP address and encrypt traffic", "To store passwords", "To block all websites" },
                    CorrectAnswerIndex = 1,
                    Explanation = "VPN protects your privacy by encrypting your connection."
                },
                new QuizQuestion
                {
                    Question = "What is social engineering?",
                    Options = new List<string> { "A type of coding", "Manipulating people to give up information", "Building social networks", "A firewall setting" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Social engineering tricks people, not computers."
                }
            };
        }

        public QuizQuestion GetNextQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                return questions[currentQuestionIndex];
            }
            return null;
        }

        public bool CheckAnswer(int selectedIndex)
        {
            bool isCorrect = (selectedIndex == questions[currentQuestionIndex].CorrectAnswerIndex);
            if (isCorrect)
            {
                score++;
            }
            return isCorrect;
        }

        public void MoveToNextQuestion()
        {
            currentQuestionIndex++;
        }

        public bool IsQuizComplete()
        {
            return currentQuestionIndex >= questions.Count;
        }

        public int GetScore()
        {
            return score;
        }

        public int GetTotalQuestions()
        {
            return questions.Count;
        }

        public void ResetQuiz()
        {
            currentQuestionIndex = 0;
            score = 0;
        }

        public string GetFeedbackMessage()
        {
            int percentage = (score * 100) / questions.Count;
            if (percentage >= 80)
                return $"Great job! You scored {score}/{questions.Count}. You're a cybersecurity pro! ";
            else if (percentage >= 50)
                return $"Good effort! You scored {score}/{questions.Count}. Keep learning to stay safe online! ";
            else
                return $"You scored {score}/{questions.Count}. Review the tips and try again to improve your cybersecurity knowledge! ";
        }
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
    }
}
