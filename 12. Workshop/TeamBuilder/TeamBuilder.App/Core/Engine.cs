using System;

namespace TeamBuilder.App.Core
{
    public class Engine
    {
        private CommandDispatcher commandDispatcher;

        public Engine(CommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher;
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    var input = Console.ReadLine();
                    var output = this.commandDispatcher.Dispatch(input);

                    Console.WriteLine(output);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.GetBaseException().Message);
                }
            }
        }
    }
}
