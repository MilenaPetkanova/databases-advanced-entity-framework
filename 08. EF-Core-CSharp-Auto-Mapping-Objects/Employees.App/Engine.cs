namespace Employees.App
{
    using System;
    using Employees.App.Contracts;
    using Employees.Services.Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;

    public class Engine : IEngine
    {
        private IServiceProvider serviceProvider;

        public Engine(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Run()
        {
            var initializeDb = this.serviceProvider.GetService<IDbInitializerService>();
            initializeDb.InitializeDatabase();

            var commandInterperter = this.serviceProvider.GetService<ICommandInterpreter>();

            while (true)
            {
                var input = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var commandName = input[0];

                if (commandName.Equals("Exit"))
                {
                    Environment.Exit(0);
                }

                try
                {
                    var commandArgs = input.Skip(1).ToArray();

                    var command = commandInterperter.Parse(this.serviceProvider, commandName);

                    var result = command.Execute(commandArgs);

                    Console.WriteLine(result);
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae.Message);
                }
            }
        }
    }
}
