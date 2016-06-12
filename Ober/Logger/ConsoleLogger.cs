using System;
using Ober.Tool.Interfaces;

namespace Ober.Tool.Logger
{
    internal class ConsoleLogger : ILogger
    {
        private readonly ISpinner _spinner;

        public bool Verbose { get; set; }

        public ConsoleLogger(ISpinner spinner)
        {
            _spinner = spinner;
        }

        public void Debug(string message)
        {
            if (Verbose)
                Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void InfoWithProgress(string message)
        {
            Console.Write($"{message}  ");
            _spinner.Start();
        }

        public void StopProgress()
        {
            _spinner.Stop();
            Console.Write("\n");
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"I'm sorry Sir, {message}");
            Console.ResetColor();
        }
    }
}
