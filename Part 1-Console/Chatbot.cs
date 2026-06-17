using System;

namespace CyberSecurityChatbot
{
    class Chatbot
    {
        public static string GetUserName()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("===============================================================================================");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nEnter your name: ");
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "User";
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nWelcome, {name}! I'm here to help you stay safe online.");
            Console.WriteLine("===============================================================================================");
            Console.ResetColor();

            return name;
        }

        public static void StartChat(string name)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nYou: ");
                Console.ResetColor();

                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Bot: Please enter something valid.");
                    continue;
                }

                string lowerInput = input.ToLower();

                if (lowerInput == "exit" || lowerInput == "bye")
                {
                    Console.WriteLine($"Bot: Goodbye {name}! Stay safe online.");
                    break;
                }

                Respond(lowerInput, name);
            }
        }

        private static void Respond(string input, string name)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Bot: ");

            if (input.Contains("phishing"))
            {
                Console.WriteLine("Phishing is when attackers trick you into revealing personal infoormation through fake emails or websites.");
                Console.WriteLine("======================================================================================================================");
            }
            else if (input == "hello" || input == "hi")
            {
                Console.WriteLine($"Hello {name}! How can I help you?");
            }
            else if (input.Contains("how are you"))
            {
                Console.WriteLine("I'm doing great! Ready to help you stay safe online.");
                Console.WriteLine("=======================================================================================================================");
            }
            else if (input.Contains("purpose"))
            {
                Console.WriteLine("My purpose is to educate you about cybersecurity and keep you safe online.");
            }
            else if (input.Contains("password"))
            {
                Console.WriteLine("Use strong passwords with uppercase, lowercase, numbers, and symbols.");
            }
            else if (input.Contains("browsing"))
            {
                Console.WriteLine("Always use secure websites (HTTPS) and avoid clicking suspicious links.");
                Console.WriteLine("====================================================================================================================");
            }
            else if (input.Contains("what can i ask"))
            {
                Console.WriteLine("You can ask me about passwords, phishing, and safe browsing.");
            }
            else
            {
                Console.WriteLine("I didn’t quite understand that. Could you rephrase?");
            }

            Console.ResetColor();
            Console.WriteLine("===================================================================================================================");
        }
    }
}