using System;
using System.Collections.Generic;

namespace CyberSecurityChatbotWPF.Services
{
    public class SentimentAnalyser
    {
        private List<string> worriedWords = new List<string> { "worried", "scared", "afraid", "nervous", "anxious", "concerned" };
        private List<string> frustratedWords = new List<string> { "frustrated", "annoyed", "tired", "confused", "difficult" };
        private List<string> curiousWords = new List<string> { "curious", "interested", "want to learn", "tell me", "explain" };

        public string DetectSentiment(string input)
        {
            string lowerInput = input.ToLower();

            if (ContainsAny(lowerInput, worriedWords))
                return "worried";
            if (ContainsAny(lowerInput, frustratedWords))
                return "frustrated";
            if (ContainsAny(lowerInput, curiousWords))
                return "curious";

            return "neutral";
        }

        public string GetEmpatheticPrefix(string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "I understand your concern. It's normal to feel worried about online threats. ";
                case "frustrated":
                    return "I know cybersecurity can feel overwhelming. Let me help simplify things. ";
                case "curious":
                    return "That's great that you want to learn! ";
                default:
                    return "";
            }
        }

        private bool ContainsAny(string input, List<string> words)
        {
            foreach (var word in words)
            {
                if (input.Contains(word))
                    return true;
            }
            return false;
        }
    }
}