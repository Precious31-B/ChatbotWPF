# Cybersecurity Awareness Chatbot

## Module Information
- Module: PROG6221 - Programming 2A
- Assessment: Portfolio of Evidence (POE)
- Year: 2026

## Project Overview
This is a cybersecurity awareness chatbot that educates South African citizens about online safety. The project was developed in three parts, starting from a console application and evolving into a full WPF GUI application with MySQL cloud database integration.

---

## Version History

Version | Part | Description
--------|------|------------
v1.0 | Part 1 | Console-based chatbot with voice greeting, ASCII art, keyword responses, and input validation
v2.0 | Part 2 | WPF GUI with keyword recognition, random responses, memory recall, sentiment detection, and conversation flow
v3.0 | Part 3 | Complete WPF application with MySQL cloud database, 11-question quiz, activity log, and NLP simulation

---

## Features by Version

### Part 1 (v1.0) - Console Application
- Voice greeting on startup using WAV file
- ASCII art logo display
- Personalized user interaction (asks for name)
- Basic keyword responses for passwords, phishing, and browsing
- Input validation for empty or unrecognized inputs
- Colored console UI for better readability
- Exit command ("exit" or "bye")

### Part 2 (v2.0) - WPF GUI Application
- Professional GUI with dark theme
- Keyboard shortcut (Enter key to send)
- Keyword recognition for: password, phishing, scam, privacy, browsing
- Random responses - each keyword has 5 or more responses, randomly selected
- Memory store to remember user name and interests
- Sentiment detection for worried, frustrated, and curious users
- Empathetic responses based on detected sentiment
- Conversation flow with "tell me more" follow-ups
- Error handling for unrecognized inputs

### Part 3 (v3.0) - Complete Application

#### Task Assistant with Cloud MySQL Database
- Add tasks with title, description, and reminder date
- View all tasks with status (Pending or Completed)
- Mark tasks as complete
- Delete tasks
- Data persists in cloud database
- No local database installation needed

#### Cybersecurity Quiz
- 11 multiple choice questions
- Topics include: passwords, phishing, 2FA, ransomware, VPN, social engineering
- Score tracking throughout quiz
- Immediate feedback with explanations for each answer
- Final score with personalized feedback message

#### Activity Log
- Tracks last 10 user actions
- Timestamps for each action in HH:mm:ss format
- View log via button or NLP commands
- Logs include: user messages, quiz attempts, task actions, chat clearing

#### NLP Simulation
- Recognizes different phrasings for the same intent
- Task addition: "add task", "remind me to", "i need to", "remember to"
- Task with reminder: "remind me to [task] in [time]"
- Full task: "Add task: [title] with description: [desc] with reminder: [time]"
- View tasks: "show tasks", "view my tasks", "list tasks"
- Task completion: "complete task", "finish task", "mark task"
- Task deletion: "delete task", "remove task", "erase task"
- Start quiz: "take quiz", "play quiz", "do the quiz"
- Activity log: "show log", "what have you done", "recent actions"
- Clear chat: "clear screen", "reset chat", "clear history"

---

## Database Setup

This application uses a free cloud MySQL database hosted on Alwaysdata. **No installation or setup is required.**

### How It Works
- The application connects to Alwaysdata cloud MySQL automatically
- The Tasks table is created automatically
- All tasks are stored in the cloud
- No local database installation needed
- No connection string changes needed

### Database Details
- Host: mysql-prog6221.alwaysdata.net
- Database: prog6221_cybersecurity_chatbot_db
- User: prog6221_chatbot_user

### Requirements
- Internet connection (to connect to the cloud database)
- MySql.Data NuGet package (already included)

### Tech Stack
- MySQL (cloud-hosted on Alwaysdata)
- MySql.Data NuGet package

---

## How to Run the Application

### Requirements
- Visual Studio 2022 or later
- .NET 8.0 SDK
- Internet connection (for cloud database)

### Steps
1. Clone this repository
2. Open CyberSecurityChatbotWPF.sln in Visual Studio
3. Press F5 to build and run

---

## Testing Commands

### Chat Commands
Command | Expected Response
--------|-------------------
Tell me about password | Random password safety tip
What is phishing | Phishing explanation
I am worried about scams | Empathetic response plus scam tips
Tell me more | Follow-up tip on last topic
What do you know about me | Recalls stored name and interests
exit | Goodbye message

### NLP Commands
Command | Action
--------|-------
remind me to update my password | Adds a task
remind me to update antivirus in 5 days | Adds task with reminder
Add task: Enable 2FA with description: Use Google Authenticator with reminder: 3 days | Adds full task
show my tasks | Displays all tasks
complete task 1 | Marks task 1 as completed
delete task 1 | Deletes task 1
take quiz | Starts the cybersecurity quiz
what have you done | Shows activity log
clear screen | Clears chat history

### Task Management Commands
Command | Action
--------|-------
Add task: Enable 2FA | Adds a new task
Add task: Update password with description: Change old password with reminder: 7 days | Adds full task
Complete task 1 | Marks task 1 as completed
Delete task 1 | Deletes task 1

---

## GitHub Actions CI

This repository uses GitHub Actions for continuous integration. The workflow builds the .NET project on every push to ensure no compilation errors.

CI Workflow Status: Passing (Green Checkmark)

---

## Project Structure

CyberSecurityChatbotWPF/
├── MainWindow.xaml              # GUI layout
├── MainWindow.xaml.cs           # Code-behind logic
├── voicegreeting.wav            # Voice greeting audio
├── Part1-Console/               # Part 1 console application
│   ├── Program.cs
│   ├── Chatbot.cs
│   └── UIHelper.cs
├── Services/
│   ├── KeywordManager.cs        # Keyword responses and random selection
│   ├── SentimentAnalyser.cs     # Emotion detection
│   ├── MemoryStore.cs           # User data storage
│   ├── DatabaseHelper.cs        # Cloud MySQL database operations
│   ├── QuizManager.cs           # Quiz questions and scoring
│   └── ActivityLogger.cs        # Action logging
├── Models/
│   └── ResponseData.cs          # Response data structure
└── .github/workflows/
    └── dotnet.yml               # CI workflow

---

## Releases

Version | Part | Description
--------|------|------------
v1.0 | Part 1 | Console-based chatbot
v2.0 | Part 2 | WPF GUI chatbot
v3.0 | Part 3 | Complete chatbot with cloud MySQL, Quiz, and NLP

---

## Author

Field | Information
------|------------
Name | Boitumelo Motaung
Student Number | ST10465326
Module | PROG6221 - Programming 2A
Year | 2026

---

## References

- Pieterse, H. 2021. The Cyber Threat Landscape in South Africa: A 10-Year Review. The African Journal of Information and Communication, 28(28). doi: https://doi.org/10.23962/10539/32213