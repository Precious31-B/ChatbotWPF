using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CyberSecurityChatbotWPF.Services
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            //MYSQL root Password
            connectionString = "Server=localhost;Database=cybersecurity_db;Uid=root;Pwd=Tumi1#;";
            CreateTable();
        }
        public void AddTaskFull(string title, string description, string reminderDate)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Tasks (Title, Description, ReminderDate) VALUES (@title, @description, @reminderDate)";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@description", description ?? "");
                        command.Parameters.AddWithValue("@reminderDate", reminderDate ?? "");
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddTaskFull error: " + ex.Message);
            }
        }

        private void CreateTable()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"
                        CREATE TABLE IF NOT EXISTS Tasks (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            Title VARCHAR(255) NOT NULL,
                            Description TEXT,
                            ReminderDate VARCHAR(50),
                            IsCompleted BOOLEAN DEFAULT FALSE
                        )";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }
        }

        public void AddTask(string title, string description, string reminderDate)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Tasks (Title, Description, ReminderDate) VALUES (@title, @description, @reminderDate)";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@description", description ?? "");
                        command.Parameters.AddWithValue("@reminderDate", reminderDate ?? "");
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddTask error: " + ex.Message);
            }
        }

        public List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Tasks ORDER BY Id DESC";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TaskItem task = new TaskItem();
                                task.Id = reader.GetInt32("Id");
                                task.Title = reader.GetString("Title");
                                task.Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description");
                                task.ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? "" : reader.GetString("ReminderDate");
                                task.IsCompleted = reader.GetBoolean("IsCompleted");
                                tasks.Add(task);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetAllTasks error: " + ex.Message);
            }
            return tasks;
        }

        public void MarkTaskComplete(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Tasks SET IsCompleted = TRUE WHERE Id = @id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MarkTaskComplete error: " + ex.Message);
            }
        }

        public void DeleteTask(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Tasks WHERE Id = @id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DeleteTask error: " + ex.Message);
            }
        }

        public int GetTaskCount()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT COUNT(*) FROM Tasks";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetTaskCount error: " + ex.Message);
                return 0;
            }
        }

        public void ClearAllTasks()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Tasks";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ClearAllTasks error: " + ex.Message);
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