namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Contracts;

    public class ExitCommand : ICommand
    {
        private const string EXIT_MESSAGE = "Good Bye.";

        // Exits application.
        public string Execute(string[] data)
        {
            Console.WriteLine(EXIT_MESSAGE);

            Environment.Exit(0);

            return EXIT_MESSAGE;
        }
    }
}
