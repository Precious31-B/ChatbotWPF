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
        private DatabaseHelper databaseHelper;
        private QuizManager quizManager;
        private ActivityLogger activityLogger;
        private string lastResponseTopic;
        private bool quizActive = false;
        private string currentQuizQuestion = "";

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            PlayVoiceGreeting();
            AskForUserName();
            activityLogger.AddLog("Application started");
        }

        private void InitializeServices()
        {
            keywordManager = new KeywordManager();
            sentimentAnalyser = new SentimentAnalyser();
            memoryStore = new MemoryStore();
            databaseHelper = new DatabaseHelper();
            quizManager = new QuizManager();
            activityLogger = new ActivityLogger();
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "voicegreeting.wav");
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
            AppendToChat("Bot", "Hello! What is your name?", Brushes.LightGreen);
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

            AppendToChat("You", userMessage, Brushes.White);
            UserInput.Clear();
            activityLogger.AddLog("User said: " + userMessage);

            string lowerMessage = userMessage.ToLower();

            // Handle quiz mode first before anything else
            if (quizActive)
            {
                int answerNumber;
                bool isValidNumber = int.TryParse(userMessage, out answerNumber);

                var currentQuestion = quizManager.GetNextQuestion();
                if (currentQuestion != null)
                {
                    if (isValidNumber && answerNumber >= 1 && answerNumber <= currentQuestion.Options.Count)
                    {
                        bool isCorrect = quizManager.CheckAnswer(answerNumber - 1);
                        if (isCorrect)
                        {
                            AppendToChat("Quiz Bot", "Correct! " + currentQuestion.Explanation, Brushes.LightGreen);
                        }
                        else
                        {
                            string correctAnswer = currentQuestion.Options[currentQuestion.CorrectAnswerIndex];
                            AppendToChat("Quiz Bot", "Wrong! The correct answer was: " + correctAnswer + ". " + currentQuestion.Explanation, Brushes.Orange);
                        }

                        quizManager.MoveToNextQuestion();
                        ShowNextQuestion();
                    }
                    else
                    {
                        AppendToChat("Quiz Bot", "Please type a number between 1 and " + currentQuestion.Options.Count, Brushes.Yellow);
                    }
                }
                else
                {
                    EndQuiz();
                }
                return;
            }

            // Normal chat mode continues here
            if (string.IsNullOrEmpty(memoryStore.UserName))
            {
                memoryStore.UserName = userMessage;
                AppendToChat("Bot", "Nice to meet you, " + memoryStore.UserName + "! I am here to help you stay safe online. What would you like to know about? You can ask about passwords, phishing, scams, or privacy.", Brushes.LightGreen);
                activityLogger.AddLog("User name stored: " + memoryStore.UserName);
                return;
            }

            if (userMessage.ToLower() == "exit" || userMessage.ToLower() == "bye")
            {
                AppendToChat("Bot", "Goodbye " + memoryStore.UserName + "! Stay safe online.", Brushes.LightGreen);
                SendButton.IsEnabled = false;
                UserInput.IsEnabled = false;
                activityLogger.AddLog("User exited chat");
                return;
            }

            if (userMessage.ToLower().Contains("what do you know about me") ||
                userMessage.ToLower().Contains("tell me about me"))
            {
                string userInfo = memoryStore.GetUserInfo();
                AppendToChat("Bot", userInfo, Brushes.LightGreen);
                activityLogger.AddLog("User asked for stored information");
                return;
            }

            if (userMessage.ToLower().Contains("tell me more") ||
                userMessage.ToLower().Contains("more tips"))
            {
                string followUp = keywordManager.GetFollowUpResponse(lastResponseTopic);
                AppendToChat("Bot", followUp, Brushes.LightGreen);
                activityLogger.AddLog("Follow-up response given for topic: " + lastResponseTopic);
                return;
            }

            // ========== TASK NLP - ORDERED BY PRIORITY ==========

            // 1. View tasks
            if (lowerMessage.Contains("show tasks") ||
                lowerMessage.Contains("view tasks") ||
                lowerMessage.Contains("list tasks") ||
                lowerMessage.Contains("what tasks") ||
                lowerMessage.Contains("my tasks") ||
                lowerMessage.Contains("show me my tasks") ||
                lowerMessage.Contains("display tasks"))
            {
                var tasks = databaseHelper.GetAllTasks();
                if (tasks.Count == 0)
                {
                    AppendToChat("Task Assistant", "You have no tasks. You can add one by saying 'Add task: [title]' or 'Remind me to [task]'.", Brushes.Cyan);
                }
                else
                {
                    AppendToChat("Task Assistant", "Your Tasks:", Brushes.Cyan);
                    foreach (var task in tasks)
                    {
                        string status = task.IsCompleted ? "Completed" : "Pending";
                        AppendToChat("", status + " [" + task.Id + "] " + task.Title, Brushes.White);
                        if (!string.IsNullOrEmpty(task.Description))
                        {
                            AppendToChat("", "    Description: " + task.Description, Brushes.Gray);
                        }
                        if (!string.IsNullOrEmpty(task.ReminderDate))
                        {
                            AppendToChat("", "    Reminder: " + task.ReminderDate, Brushes.Gray);
                        }
                    }
                    AppendToChat("Task Assistant", "Type 'Complete task [number]' or 'Delete task [number]' to manage tasks.", Brushes.Cyan);
                }
                activityLogger.AddLog("User viewed tasks via NLP");
                return;
            }

            // 2. Task with reminder: "remind me to [task] in [time]"
            if (lowerMessage.Contains("remind me to") && lowerMessage.Contains(" in "))
            {
                string taskTitle = "";
                string taskReminder = "";

                int startIndex = lowerMessage.IndexOf("remind me to") + 12;
                int endIndex = lowerMessage.IndexOf(" in ");
                if (endIndex > startIndex)
                {
                    taskTitle = userMessage.Substring(startIndex, endIndex - startIndex).Trim();
                    taskReminder = userMessage.Substring(endIndex + 4).Trim();
                }
                else
                {
                    taskTitle = userMessage.Substring(startIndex).Trim();
                    taskReminder = "7 days";
                }

                if (!string.IsNullOrEmpty(taskTitle))
                {
                    databaseHelper.AddTaskFull(taskTitle, "", taskReminder);
                    AppendToChat("Task Assistant", "Task added: " + taskTitle, Brushes.Cyan);
                    AppendToChat("", "Reminder set for: " + taskReminder, Brushes.White);
                    activityLogger.AddLog("Task added with reminder: " + taskTitle);
                    return;
                }
            }

            // 3. Simple task addition: "Add task: [title]" or "remind me to [task]"
            if (lowerMessage.Contains("add task") ||
                lowerMessage.Contains("create a task") ||
                lowerMessage.Contains("create task") ||
                lowerMessage.Contains("new task") ||
                lowerMessage.Contains("remind me to") ||
                lowerMessage.Contains("i need to") ||
                lowerMessage.Contains("save this task") ||
                lowerMessage.Contains("remember to") ||
                lowerMessage.Contains("add a task"))
            {
                string extractedTask = "";

                if (lowerMessage.Contains("remind me to") && !lowerMessage.Contains(" in "))
                {
                    int index = lowerMessage.IndexOf("remind me to") + 12;
                    extractedTask = userMessage.Substring(index).Trim();
                }
                else if (lowerMessage.Contains("i need to"))
                {
                    int index = lowerMessage.IndexOf("i need to") + 9;
                    extractedTask = userMessage.Substring(index).Trim();
                }
                else if (lowerMessage.Contains("remember to"))
                {
                    int index = lowerMessage.IndexOf("remember to") + 11;
                    extractedTask = userMessage.Substring(index).Trim();
                }
                else if (lowerMessage.Contains("add task"))
                {
                    int index = lowerMessage.IndexOf("add task") + 8;
                    extractedTask = userMessage.Substring(index).Trim();
                    extractedTask = extractedTask.TrimStart(':', ' ');
                }
                else
                {
                    extractedTask = userMessage;
                }

                if (!string.IsNullOrEmpty(extractedTask) && extractedTask.Length > 1)
                {
                    databaseHelper.AddTaskFull(extractedTask, "", "");
                    AppendToChat("Task Assistant", "Task added: " + extractedTask, Brushes.Cyan);
                    activityLogger.AddLog("Task added via NLP: " + extractedTask);
                    return;
                }
                else
                {
                    AppendToChat("Task Assistant", "Please specify a task. Example: Add task: Update password", Brushes.Cyan);
                    return;
                }
            }

            // 4. Full task with description and reminder: "Add task: [title] with description: [desc] with reminder: [time]"
            if (lowerMessage.Contains("with description") || lowerMessage.Contains("with reminder"))
            {
                string taskTitle = "";
                string taskDescription = "";
                string taskReminder = "";

                // Extract title (everything after "add task" or "add a task")
                string titlePart = lowerMessage.Replace("add task", "").Replace("add a task", "").Trim();
                if (!string.IsNullOrEmpty(titlePart))
                {
                    taskTitle = titlePart;
                }

                if (lowerMessage.Contains("with description"))
                {
                    int descIndex = lowerMessage.IndexOf("with description") + 17;
                    int reminderIndex = lowerMessage.IndexOf("with reminder");
                    if (reminderIndex > descIndex)
                    {
                        taskDescription = userMessage.Substring(descIndex, reminderIndex - descIndex).Trim();
                    }
                    else
                    {
                        taskDescription = userMessage.Substring(descIndex).Trim();
                    }
                }

                if (lowerMessage.Contains("with reminder") || lowerMessage.Contains("remind"))
                {
                    int reminderIndex = lowerMessage.IndexOf("with reminder") + 14;
                    if (reminderIndex < userMessage.Length)
                    {
                        taskReminder = userMessage.Substring(reminderIndex).Trim();
                    }
                    else
                    {
                        taskReminder = "7 days";
                    }
                }

                if (!string.IsNullOrEmpty(taskTitle))
                {
                    databaseHelper.AddTaskFull(taskTitle, taskDescription, taskReminder);
                    AppendToChat("Task Assistant", "Task added: " + taskTitle, Brushes.Cyan);
                    AppendToChat("", "Description: " + (string.IsNullOrEmpty(taskDescription) ? "None" : taskDescription), Brushes.White);
                    AppendToChat("", "Reminder: " + (string.IsNullOrEmpty(taskReminder) ? "None" : taskReminder), Brushes.White);
                    activityLogger.AddLog("Task added with full details: " + taskTitle);
                }
                else
                {
                    AppendToChat("Task Assistant", "Please specify a task title. Example: Add task: Update password with description: Change old password with reminder: 7 days", Brushes.Cyan);
                }
                return;
            }

            // 5. Complete task
            if (lowerMessage.Contains("complete task") ||
                lowerMessage.Contains("finish task") ||
                lowerMessage.Contains("mark task") ||
                lowerMessage.Contains("done task"))
            {
                string[] parts = userMessage.Split(' ');
                int taskId = -1;
                foreach (string part in parts)
                {
                    if (int.TryParse(part, out taskId))
                        break;
                }

                if (taskId > 0)
                {
                    databaseHelper.MarkTaskComplete(taskId);
                    AppendToChat("Task Assistant", "Task " + taskId + " marked as completed.", Brushes.Cyan);
                    activityLogger.AddLog("Task completed via NLP: " + taskId);
                }
                else
                {
                    AppendToChat("Task Assistant", "Please provide a task number. Example: Complete task 1", Brushes.Cyan);
                }
                return;
            }

            // 6. Delete task
            if (lowerMessage.Contains("delete task") ||
                lowerMessage.Contains("remove task") ||
                lowerMessage.Contains("erase task"))
            {
                string[] parts = userMessage.Split(' ');
                int taskId = -1;
                foreach (string part in parts)
                {
                    if (int.TryParse(part, out taskId))
                        break;
                }

                if (taskId > 0)
                {
                    databaseHelper.DeleteTask(taskId);
                    AppendToChat("Task Assistant", "Task " + taskId + " deleted.", Brushes.Cyan);
                    activityLogger.AddLog("Task deleted via NLP: " + taskId);
                }
                else
                {
                    AppendToChat("Task Assistant", "Please provide a task number. Example: Delete task 1", Brushes.Cyan);
                }
                return;
            }

            // ========== QUIZ NLP ==========

            if (lowerMessage.Contains("start quiz") ||
                lowerMessage.Contains("take quiz") ||
                lowerMessage.Contains("play quiz") ||
                lowerMessage.Contains("do the quiz") ||
                lowerMessage.Contains("i want to take the quiz") ||
                lowerMessage.Contains("begin quiz") ||
                lowerMessage.Contains("let us do the quiz"))
            {
                quizManager.ResetQuiz();
                quizActive = true;
                AppendToChat("Quiz Bot", "Starting Cybersecurity Quiz! You will get " + quizManager.GetTotalQuestions() + " questions. Let us begin.", Brushes.LightGreen);
                ShowNextQuestion();
                activityLogger.AddLog("Quiz started via NLP");
                return;
            }

            // ========== ACTIVITY LOG NLP ==========

            if (lowerMessage.Contains("activity log") ||
                lowerMessage.Contains("show log") ||
                lowerMessage.Contains("view log") ||
                lowerMessage.Contains("what have you done") ||
                lowerMessage.Contains("show activity") ||
                lowerMessage.Contains("recent actions"))
            {
                string log = activityLogger.GetFormattedLog();
                AppendToChat("Activity Log", log, Brushes.LightBlue);
                activityLogger.AddLog("User viewed activity log via NLP");
                return;
            }

            // ========== CLEAR CHAT NLP ==========

            if (lowerMessage.Contains("clear chat") ||
                lowerMessage.Contains("clear screen") ||
                lowerMessage.Contains("clear history") ||
                lowerMessage.Contains("reset chat"))
            {
                ChatHistory.Document.Blocks.Clear();
                AppendToChat("System", "Chat history cleared.", Brushes.Yellow);
                activityLogger.AddLog("Chat history cleared via NLP");
                return;
            }

            // ========== SENTIMENT + KEYWORD RESPONSES ==========

            // First detect sentiment
            string sentiment = sentimentAnalyser.DetectSentiment(userMessage);
            string empatheticPrefix = sentimentAnalyser.GetEmpatheticPrefix(sentiment);

            // Then check for keywords
            var response = keywordManager.GetResponseForInput(userMessage);

            if (response != null)
            {
                lastResponseTopic = response.Topic;

                if (userMessage.ToLower().Contains("interested in"))
                {
                    memoryStore.RememberInterest(response.Topic);
                    activityLogger.AddLog("User interest saved: " + response.Topic);
                }

                string personalizedPrefix = memoryStore.RecallPersonalizedMessage();
                string finalResponse = empatheticPrefix + personalizedPrefix + response.Text;
                AppendToChat("Bot", finalResponse, Brushes.LightGreen);
                activityLogger.AddLog("Bot responded on topic: " + response.Topic);
            }
            else
            {
                // If no keyword matched but sentiment detected, respond with empathy + general tip
                if (sentiment != "neutral")
                {
                    string generalTip = "Remember to stay vigilant online. Always use strong passwords, enable 2FA, and be careful of suspicious links and emails.";
                    AppendToChat("Bot", empatheticPrefix + generalTip, Brushes.LightGreen);
                    activityLogger.AddLog("Bot gave empathetic response for sentiment: " + sentiment);
                }
                else
                {
                    AppendToChat("Bot", "I am not sure I understand. You can ask about passwords, phishing, scams, or privacy. Or use the buttons above for tasks, quiz, or activity log.", Brushes.Orange);
                    activityLogger.AddLog("Bot gave default response for unrecognized input");
                }
            }
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            quizManager.ResetQuiz();
            quizActive = true;
            AppendToChat("Quiz Bot", "Starting Cybersecurity Quiz! You will get " + quizManager.GetTotalQuestions() + " questions. Let us begin.", Brushes.LightGreen);
            ShowNextQuestion();
            activityLogger.AddLog("Quiz started by user");
        }

        private void ShowNextQuestion()
        {
            var question = quizManager.GetNextQuestion();
            if (question != null)
            {
                currentQuizQuestion = question.Question;
                string optionsText = "";
                for (int i = 0; i < question.Options.Count; i++)
                {
                    optionsText = optionsText + "\n   " + (i + 1).ToString() + ". " + question.Options[i];
                }
                AppendToChat("Quiz Bot", "Question " + (quizManager.GetScore() + 1) + " of " + quizManager.GetTotalQuestions() + ":", Brushes.Yellow);
                AppendToChat("Quiz Bot", question.Question + optionsText + "\n\nType the number of your answer (1, 2, 3, or 4).", Brushes.Yellow);
            }
            else
            {
                EndQuiz();
            }
        }

        private void EndQuiz()
        {
            string feedback = quizManager.GetFeedbackMessage();
            AppendToChat("Quiz Bot", "Quiz Complete! " + feedback, Brushes.Gold);
            quizActive = false;
            activityLogger.AddLog("Quiz completed. Score: " + quizManager.GetScore() + "/" + quizManager.GetTotalQuestions());
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            var tasks = databaseHelper.GetAllTasks();
            if (tasks.Count == 0)
            {
                AppendToChat("Task Assistant", "You have no tasks. Type 'Add task: [title]' or 'Remind me to [task]' to create one.", Brushes.Cyan);
            }
            else
            {
                AppendToChat("Task Assistant", "Your Tasks:", Brushes.Cyan);
                foreach (var task in tasks)
                {
                    string status = task.IsCompleted ? "Completed" : "Pending";
                    AppendToChat("", status + " [" + task.Id + "] " + task.Title, Brushes.White);
                    if (!string.IsNullOrEmpty(task.Description))
                    {
                        AppendToChat("", "    Description: " + task.Description, Brushes.Gray);
                    }
                    if (!string.IsNullOrEmpty(task.ReminderDate))
                    {
                        AppendToChat("", "    Reminder: " + task.ReminderDate, Brushes.Gray);
                    }
                }
                AppendToChat("Task Assistant", "Type 'Complete task [number]' or 'Delete task [number]' to manage tasks.", Brushes.Cyan);
            }
            activityLogger.AddLog("User viewed tasks");
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            string log = activityLogger.GetFormattedLog();
            AppendToChat("Activity Log", log, Brushes.LightBlue);
            activityLogger.AddLog("User viewed activity log");
        }

        private void ClearChatButton_Click(object sender, RoutedEventArgs e)
        {
            ChatHistory.Document.Blocks.Clear();
            AppendToChat("System", "Chat history cleared.", Brushes.Yellow);
            activityLogger.AddLog("Chat history cleared");
        }

        private void AppendToChat(string sender, string message, Brush color)
        {
            Paragraph paragraph = new Paragraph();

            if (!string.IsNullOrEmpty(sender))
            {
                Run senderRun = new Run(sender + ": ");
                senderRun.Foreground = color;
                senderRun.FontWeight = FontWeights.Bold;
                paragraph.Inlines.Add(senderRun);
            }

            Run messageRun = new Run(message);
            messageRun.Foreground = Brushes.White;
            paragraph.Inlines.Add(messageRun);

            ChatHistory.Document.Blocks.Add(paragraph);
            ChatScrollViewer.ScrollToBottom();
        }
    }
}