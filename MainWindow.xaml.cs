using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CyberSecurityChatbotWPF.Services;

namespace CyberSecurityChatbotWPF
{
    public partial class MainWindow : Window
    {
        private KeywordManager keywordManager;
        private SentimentAnalyser sentimentAnalyser;
        private MemoryStore memoryStore;
        private string lastResponseTopic;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            PlayVoiceGreeting();
            AskForUserName();
        }

        private void InitializeServices()
        {
            keywordManager = new KeywordManager();
            sentimentAnalyser = new SentimentAnalyser();
            memoryStore = new MemoryStore();
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "voiceGreeting.wav");
                if (System.IO.File.Exists(soundPath))
                {
                    using (SoundPlayer player = new SoundPlayer(soundPath))
                    {
                        player.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToChat("System", "Could not play voice greeting.", Brushes.Yellow);
            }
        }

        private void AskForUserName()
        {
            AppendToChat("Bot", "Hello! What's your name?", Brushes.LightGreen);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void UserInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ProcessUserInput();
            }
        }

        private void ProcessUserInput()
        {
            string userMessage = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                AppendToChat("System", "Please enter a message.", Brushes.Yellow);
                return;
            }

            // Display user message
            AppendToChat("You", userMessage, Brushes.White);
            UserInput.Clear();

            // Check if we haven't asked for name yet
            if (string.IsNullOrEmpty(memoryStore.UserName))
            {
                memoryStore.UserName = userMessage;
                AppendToChat("Bot", $"Nice to meet you, {memoryStore.UserName}! I'm here to help you stay safe online. What would you like to know about? (Try asking about passwords, phishing, scams, or privacy)", Brushes.LightGreen);
                return;
            }

            // Handle exit commands
            if (userMessage.ToLower() == "exit" || userMessage.ToLower() == "bye")
            {
                AppendToChat("Bot", $"Goodbye {memoryStore.UserName}! Stay safe online! 🛡️", Brushes.LightGreen);
                SendButton.IsEnabled = false;
                UserInput.IsEnabled = false;
                return;
            }

            // Handle "what do you know about me"
            if (userMessage.ToLower().Contains("what do you know about me") ||
                userMessage.ToLower().Contains("tell me about me") ||
                userMessage.ToLower().Contains("what do you remember") ||
                userMessage.ToLower().Contains("what do you know"))
            {
                string userInfo = memoryStore.GetUserInfo();
                AppendToChat("Bot", userInfo, Brushes.LightGreen);
                return;
            }

            // Handle follow-up requests
            if (userMessage.ToLower().Contains("tell me more") ||
                userMessage.ToLower().Contains("more tips") ||
                userMessage.ToLower().Contains("another tip"))
            {
                string followUp = keywordManager.GetFollowUpResponse(lastResponseTopic);
                AppendToChat("Bot", followUp, Brushes.LightGreen);
                return;
            }

            // Detect sentiment
            string sentiment = sentimentAnalyser.DetectSentiment(userMessage);
            string empatheticPrefix = sentimentAnalyser.GetEmpatheticPrefix(sentiment);

            // Get response based on keywords
            var response = keywordManager.GetResponseForInput(userMessage);

            if (response != null)
            {
                lastResponseTopic = response.Topic;

                // Remember if user mentions an interest
                if (userMessage.ToLower().Contains("interest") ||
                    userMessage.ToLower().Contains("privacy") ||
                    userMessage.ToLower().Contains("password") ||
                    userMessage.ToLower().Contains("phishing") ||
                    userMessage.ToLower().Contains("scam") ||
                    userMessage.ToLower().Contains("interested in"))
                {
                    memoryStore.RememberInterest(response.Topic);
                }

                // Personalize with memory if available
                string personalizedPrefix = memoryStore.RecallPersonalizedMessage();

                string finalResponse = empatheticPrefix + personalizedPrefix + response.Text;
                AppendToChat("Bot", finalResponse, Brushes.LightGreen);
            }
            else
            {
                // Default response for unrecognized input
                AppendToChat("Bot", "I'm not sure I understand. Can you ask about passwords, phishing, scams, or privacy? Or type 'exit' to leave.", Brushes.Orange);
            }
        }

        private void AppendToChat(string sender, string message, Brush color)
        {
            // Create a rich text paragraph
            Paragraph paragraph = new Paragraph();

            // Add sender name in bold with color
            Run senderRun = new Run($"{sender}: ");
            senderRun.Foreground = color;
            senderRun.FontWeight = FontWeights.Bold;
            paragraph.Inlines.Add(senderRun);

            // Add message
            Run messageRun = new Run(message);
            messageRun.Foreground = Brushes.White;
            paragraph.Inlines.Add(messageRun);

            // Add to chat
            ChatHistory.Document.Blocks.Add(paragraph);

            // Auto-scroll
            ChatScrollViewer.ScrollToBottom();
        }
    }
}