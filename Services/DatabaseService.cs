using System;
using System.Collections.Generic;
using MySqlConnector;
using CyberSecurityAwarenessBot.Models;

namespace CyberSecurityAwarenessBot.Services
{
    public class DatabaseService
    {
        private readonly string _connStr =
            "Server=localhost;Database=cyberbot;Uid=root;Pwd=yourpassword;";

        private readonly string _noDbConn =
            "Server=localhost;Uid=root;Pwd=yourpassword;";

        public DatabaseService()
        {
            InitialiseDatabase();
        }

        // ─────────────────────────────────────────────
        // INIT DATABASE
        // ─────────────────────────────────────────────
        private void InitialiseDatabase()
        {
            try
            {
               var conn = new MySqlConnection(_noDbConn);
                conn.Open();

               var cmd = conn.CreateCommand();

                cmd.CommandText = "CREATE DATABASE IF NOT EXISTS cyberbot;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "USE cyberbot;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS tasks (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        title VARCHAR(255) NOT NULL,
                        description TEXT NOT NULL,
                        reminder VARCHAR(255),
                        reminder_date DATETIME,
                        is_completed TINYINT(1) DEFAULT 0,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB INIT ERROR] {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────
        // ADD TASK
        // ─────────────────────────────────────────────
        public int AddTask(TaskItem task)
        {
            try
            {
                var conn = new MySqlConnection(_connStr);
                conn.Open();

                 var cmd = conn.CreateCommand();

                cmd.CommandText = @"
                    INSERT INTO tasks (title, description, reminder, reminder_date)
                    VALUES (@title, @desc, @reminder, @rdate);
                    SELECT LAST_INSERT_ID();";

                AddParams(cmd,
                    ("@title", task.Title),
                    ("@desc", task.Description),
                    ("@reminder", task.Reminder),
                    ("@rdate", task.ReminderDate)
                );

                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ADD TASK ERROR] {ex.Message}");
                return -1;
            }
        }

        // ─────────────────────────────────────────────
        // GET ALL TASKS
        // ─────────────────────────────────────────────
        public List<TaskItem> GetAllTasks(object tasksReader)
        {
            var list = new List<TaskItem>();

            try
            {
                var conn = new MySqlConnection(_connStr);
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM tasks ORDER BY created_at DESC;";

                var reader = cmd.ExecuteReader();

                
                {
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GET TASKS ERROR] {ex.Message}");
            }

            return list;
        }

        // ─────────────────────────────────────────────
        // MARK COMPLETED
        // ─────────────────────────────────────────────
        public void MarkCompleted(int id)
        {
            ExecuteNonQuery(
                "UPDATE tasks SET is_completed = 1 WHERE id = @id",
                ("@id", id)
            );
        }

        // ─────────────────────────────────────────────
        // DELETE TASK
        // ─────────────────────────────────────────────
        public void DeleteTask(int id)
        {
            ExecuteNonQuery(
                "DELETE FROM tasks WHERE id = @id",
                ("@id", id)
            );
        }

        // ─────────────────────────────────────────────
        // GENERIC EXECUTE METHOD
        // ─────────────────────────────────────────────
        private void ExecuteNonQuery(string sql, params (string Key, object Value)[] parameters)
        {
            try
            {
                var conn = new MySqlConnection(_connStr);
                conn.Open();

               var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                AddParams(cmd, parameters);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXEC ERROR] {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────
        // PARAMETER HELPER (NO .ADD USED)
        // ─────────────────────────────────────────────
        private void AddParams(MySqlCommand cmd, params (string Key, object Value)[] parameters)
        {
            foreach (var (key, value) in parameters)
            {
                
            }
        }

        // ─────────────────────────────────────────────
        // MAP DATABASE ROW → MODEL
        // ─────────────────────────────────────────────
        
            };
        }
    
