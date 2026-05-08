using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using TimeAnchor.Models;

namespace TimeAnchor.Repositories
{
    public class DatabaseHelper
    {
        private readonly string dbPath = "Data Source=TimeAnchor.db";

        public void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Reminders (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        EventDate TEXT NOT NULL,
                        IsTimeSpecific INTEGER NOT NULL,
                        IsCompleted INTEGER NOT NULL,
                        IsActive INTEGER NOT NULL DEFAULT 1,
                        IsSynced INTEGER NOT NULL,
                        Recurrence INTEGER NOT NULL DEFAULT 0,
                        Category TEXT,
                        AlarmSoundPath TEXT
                    );";

                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddReminder(Reminder reminder)
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                string insertQuery = @"
                    INSERT INTO Reminders (Title, Description, EventDate, IsTimeSpecific, IsCompleted, IsActive, IsSynced, Recurrence, Category, AlarmSoundPath) 
                    VALUES (@Title, @Description, @EventDate, @IsTimeSpecific, @IsCompleted, @IsActive, @IsSynced, @Recurrence, @Category, @AlarmSoundPath)";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Title", reminder.Title);
                    command.Parameters.AddWithValue("@Description", reminder.Description ?? "");
                    command.Parameters.AddWithValue("@EventDate", reminder.EventDate.ToString("o"));
                    command.Parameters.AddWithValue("@IsTimeSpecific", reminder.IsTimeSpecific ? 1 : 0);
                    command.Parameters.AddWithValue("@IsCompleted", reminder.IsCompleted ? 1 : 0);
                    command.Parameters.AddWithValue("@IsActive", reminder.IsActive ? 1 : 0);
                    command.Parameters.AddWithValue("@IsSynced", reminder.IsSynced ? 1 : 0);
                    command.Parameters.AddWithValue("@Recurrence", (int)reminder.Recurrence);
                    command.Parameters.AddWithValue("@Category", string.IsNullOrWhiteSpace(reminder.Category) ? "Genel" : reminder.Category);
                    command.Parameters.AddWithValue("@AlarmSoundPath", reminder.AlarmSoundPath ?? "");

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Reminder> GetAllReminders()
        {
            List<Reminder> reminders = new List<Reminder>();
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM Reminders ORDER BY EventDate ASC";

                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reminders.Add(new Reminder
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                EventDate = DateTime.Parse(reader["EventDate"].ToString()),
                                IsTimeSpecific = Convert.ToInt32(reader["IsTimeSpecific"]) == 1,
                                IsCompleted = Convert.ToInt32(reader["IsCompleted"]) == 1,
                                IsActive = Convert.ToInt32(reader["IsActive"]) == 1,
                                IsSynced = Convert.ToInt32(reader["IsSynced"]) == 1,
                                Recurrence = (RecurrenceType)Convert.ToInt32(reader["Recurrence"]),
                                Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "Genel",
                                AlarmSoundPath = reader["AlarmSoundPath"] != DBNull.Value ? reader["AlarmSoundPath"].ToString() : ""
                            });
                        }
                    }
                }
            }
            return reminders;
        }

        public void UpdateReminder(Reminder reminder)
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                string updateQuery = @"
                    UPDATE Reminders 
                    SET Title = @Title, 
                        Description = @Description, 
                        EventDate = @EventDate, 
                        IsTimeSpecific = @IsTimeSpecific, 
                        IsCompleted = @IsCompleted, 
                        IsActive = @IsActive,
                        IsSynced = @IsSynced,
                        Recurrence = @Recurrence,
                        Category = @Category,
                        AlarmSoundPath = @AlarmSoundPath
                    WHERE Id = @Id";

                using (var command = new SqliteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", reminder.Id);
                    command.Parameters.AddWithValue("@Title", reminder.Title);
                    command.Parameters.AddWithValue("@Description", reminder.Description ?? "");
                    command.Parameters.AddWithValue("@EventDate", reminder.EventDate.ToString("o"));
                    command.Parameters.AddWithValue("@IsTimeSpecific", reminder.IsTimeSpecific ? 1 : 0);
                    command.Parameters.AddWithValue("@IsCompleted", reminder.IsCompleted ? 1 : 0);
                    command.Parameters.AddWithValue("@IsActive", reminder.IsActive ? 1 : 0);
                    command.Parameters.AddWithValue("@IsSynced", reminder.IsSynced ? 1 : 0);
                    command.Parameters.AddWithValue("@Recurrence", (int)reminder.Recurrence);
                    command.Parameters.AddWithValue("@Category", string.IsNullOrWhiteSpace(reminder.Category) ? "Genel" : reminder.Category);
                    command.Parameters.AddWithValue("@AlarmSoundPath", reminder.AlarmSoundPath ?? "");

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteReminder(int id)
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Reminders WHERE Id = @Id";
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}