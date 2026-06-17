using System;

namespace CyberSecurityChatbot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Chatbot";

            VoiceGreeting.PlayGreeting();
            UIHelper.DisplayHeader();

            string userName = Chatbot.GetUserName();
            Chatbot.StartChat(userName);
        }
    }
}