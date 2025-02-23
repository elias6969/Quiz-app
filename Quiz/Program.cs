using System;
using Quiz.Data;

namespace QuizApp.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Quiz App...");

            DatabaseInitializer.CreateTables();
            Console.WriteLine("Database tables created.");

            DatabaseInitializer.InsertQuestion("What is the capital of Sweden?");
            Console.WriteLine("A sample question was inserted.");

            Console.WriteLine("Listing all questions:");
            DatabaseInitializer.SelectAllQuestions();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
