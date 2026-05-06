using System;
using System.Collections.Generic;
using System.Linq;
using CyberSecurityChatbotWPF.Models;

namespace CyberSecurityChatbotWPF.Services
{
    public class KeywordManager
    {
        private Dictionary<string, List<string>> keywordResponses;

        public KeywordManager()
        {
            keywordResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["password"] = new List<string>
                {
                    "Use strong passwords with uppercase, lowercase, numbers, and symbols.",
                    "Never reuse passwords across different accounts.",
                    "Enable two-factor authentication for an extra layer of security.",
                    "A good password should be at least 12 characters long.",
                    "Consider using a password manager to keep track of your passwords."
                },
                ["phishing"] = new List<string>
                {
                    "Phishing scams trick you into revealing personal info through fake emails.",
                    "Always check the sender's email address before clicking links.",
                    "Report phishing emails to your email provider.",
                    "Hover over links before clicking to see the actual URL.",
                    "Legitimate companies never ask for your password via email."
                },
                ["scam"] = new List<string>
                {
                    "Scammers often impersonate trusted organizations like banks.",
                    "Never share OTPs or passwords with anyone, even if they claim to be from support.",
                    "If it sounds too good to be true, it probably is a scam.",
                    "Be wary of urgent requests for money or personal information.",
                    "Always verify caller identity by calling back on official numbers."
                },
                ["privacy"] = new List<string>
                {
                    "Review your privacy settings on social media regularly.",
                    "Use encrypted messaging apps for sensitive conversations.",
                    "Be careful what personal information you share online.",
                    "Limit the amount of personal data you post publicly.",
                    "Use a VPN when connecting to public Wi-Fi networks."
                },
                ["browsing"] = new List<string>
                {
                    "Always check for 'https://' in the URL before entering personal details.",
                    "Avoid clicking suspicious pop-ups or ads.",
                    "Use a trusted VPN on public Wi-Fi networks.",
                    "Clear your browser cache and cookies regularly.",
                    "Use ad-blockers and anti-tracking extensions."
                },
                ["hello"] = new List<string> { "Hello! How can I help you stay safe online today?" },
                ["hi"] = new List<string>
                {
                    "Hi there! Ready to learn about cybersecurity?",
                    "Hello! What cybersecurity topic would you like to learn about?"
                },
                ["hey"] = new List<string>
                {
                    "Hey! Ask me about passwords, phishing, or privacy.",
                    "Hi! What cybersecurity topic are you interested in?"
                },
                ["greetings"] = new List<string>
                {
                    "Hello! How can I help you stay safe online today?",
                    "Hi there! Ready to learn about cybersecurity?",
                    "Hey! Ask me about passwords, phishing, or privacy.",
                    "Greetings! I'm here to help you with cybersecurity awareness.",
                    "Hello! What cybersecurity topic would you like to learn about?"
                }
            };
        }

        // Get responses for a keyword (random selection)
        public ResponseData GetResponseForInput(string input)
        {
            string lowerInput = input.ToLower();

            foreach (var keyword in keywordResponses.Keys)
            {
                if (lowerInput.Contains(keyword))
                {
                    var responses = keywordResponses[keyword];
                    Random rand = new Random();
                    string selectedResponse = responses[rand.Next(responses.Count)];

                    return new ResponseData
                    {
                        Text = selectedResponse,
                        IsFollowUp = false,
                        Topic = keyword
                    };
                }
            }

            return null; // No keyword found
        }

        // For follow-up questions ("tell me more")
        public string GetFollowUpResponse(string topic)
        {
            if (!string.IsNullOrEmpty(topic) && keywordResponses.ContainsKey(topic))
            {
                var responses = keywordResponses[topic];
                Random rand = new Random();
                return $"Here's another tip about {topic}: {responses[rand.Next(responses.Count)]}";
            }
            return "I can share more details on passwords, phishing, scams, privacy, or safe browsing. What interests you?";
        }
    }
}