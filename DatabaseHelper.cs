using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace CyberSecurityChatbotWPF.Services
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            string databasePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.db");
            connectionString = $"Data Source={databasePath}";
            CreateTable();
        }

        private void CreateTable()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        ReminderDate TEXT,
                        IsCompleted INTEGER DEFAULT 0
                    )";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddTask(string title, string description, string reminderDate)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Tasks (Title, Description, ReminderDate) VALUES (@title, @description, @reminderDate)";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@reminderDate", reminderDate ?? "");
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Tasks ORDER BY Id DESC";
                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskItem
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2),
                                ReminderDate = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                IsCompleted = reader.GetInt32(4) == 1
                            });
                        }
                    }
                }
            }
            return tasks;
        }

        public void MarkTaskComplete(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @id";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTask(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Tasks WHERE Id = @id";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
