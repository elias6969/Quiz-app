using System;
using System.Collections.Generic;
using Npgsql;

namespace QuizApp
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }

    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class QuizRepository
    {
        private readonly string _connectionString;

        public QuizRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Brings the questions from the database
        public List<Question> GetAllQuestions()
        {
            var questions = new List<Question>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Brings the questions
                string query = "SELECT id, questiontext FROM Questions;";
                using (var cmd = new NpgsqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        questions.Add(new Question
                        {
                            Id = reader.GetInt32(0),
                            QuestionText = reader.GetString(1)
                        });
                    }
                }

                // Brings the answers for each question
                foreach (var question in questions)
                {
                    string answerQuery = "SELECT id, questionid, answertext, iscorrect FROM Answers WHERE questionid = @questionId;";
                    using (var cmd = new NpgsqlCommand(answerQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("questionId", question.Id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                question.Answers.Add(new Answer
                                {
                                    Id = reader.GetInt32(0),
                                    QuestionId = reader.GetInt32(1),
                                    AnswerText = reader.GetString(2),
                                    IsCorrect = reader.GetBoolean(3)
                                });
                            }
                        }
                    }
                }
            }

            return questions;
        }
    }

    // Class that handles the quiz logic
    public class QuizManager
    {
        private readonly QuizRepository _repository;

        public QuizManager(QuizRepository repository)
        {
            _repository = repository;
        }

        // Run the quiz in the console
        public void RunQuiz()
        {
            var questions = _repository.GetAllQuestions();
            int score = 0;

            foreach (var question in questions)
            {
                Console.WriteLine("Question: " + question.QuestionText);
                for (int i = 0; i < question.Answers.Count; i++)
                {
                    Console.WriteLine($"{i + 1}: {question.Answers[i].AnswerText}");
                }
                Console.Write("Your answer (Choose a number): ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out int selectedIndex))
                {
                    if (selectedIndex - 1 < question.Answers.Count && selectedIndex > 0)
                    {
                        if (question.Answers[selectedIndex - 1].IsCorrect)
                        {
                            Console.WriteLine("Right answer!");
                            score++;
                        }
                        else
                        {
                            Console.WriteLine("Wrong answer!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("invalid choice.");
                    }
                }
                else
                {
                    Console.WriteLine("invalid.");
                }
                Console.WriteLine();
            }

            Console.WriteLine($"Quiz done! your points: {score}/{questions.Count}");
        }
    }

    // Program entry point
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Host=localhost;Username=postgres;Password=fotboll2017;Database=pigeonquiz";

            var repository = new QuizRepository(connectionString);
            var quizManager = new QuizManager(repository);

            Console.WriteLine("Welcome to PigeonQuiz!");
            quizManager.RunQuiz();

            Console.WriteLine("Press a key to end the game!");
            Console.ReadKey();
        }
    }
}
