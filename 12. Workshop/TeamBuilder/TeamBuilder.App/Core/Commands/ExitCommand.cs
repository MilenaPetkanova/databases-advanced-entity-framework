namespace TeamBuilder.App.Core.Commands
{
    using System;
    using TeamBuilder.App.Utilities;

    public class ExitCommand
    {
        public string Execute(string[] inputArgs)
        {
            Check.CheckLength(0, inputArgs);

            Environment.Exit(0);

            return "Exited";
        }
    }
}
