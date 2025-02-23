using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace QuizApp.Data
{
    public static class DatabaseInitializer
    {
        private static string connectionString = "Host=localhost;Username=postgres;Password=Hjibncvnzx;Database=quizapp";

        public static void CreateTables()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var createQuestionsTable = @"
                CREATE TABLE IF NOT EXISTS Questions (
                    QuestionId SERIAL PRIMARY KEY,
                    QuestionText VARCHAR(255) NOT NULL
                );";

            var createAnswersTable = @"
                CREATE TABLE IF NOT EXISTS Answers (
                    AnswerId SERIAL PRIMARY KEY,
                    QuestionId INT NOT NULL REFERENCES Questions(QuestionId),
                    AnswerText VARCHAR(255) NOT NULL,
                    IsCorrect BOOLEAN NOT NULL
                );";

            using var command = new NpgsqlCommand(createQuestionsTable, connection);
            command.ExecuteNonQuery();

            command.CommandText = createAnswersTable;
            command.ExecuteNonQuery();

            connection.Close();
        }

        public static void InsertQuestion(string questionText)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var insertQuery = "INSERT INTO Questions (QuestionText) VALUES (@questionText)";
            using var command = new NpgsqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("questionText", questionText);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public static void SelectAllQuestions()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var selectQuery = "SELECT QuestionId, QuestionText FROM Questions";
            using var command = new NpgsqlCommand(selectQuery, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader.GetInt32(0)} - Question: {reader.GetString(1)}");
            }

            connection.Close();
        }
    }
}
