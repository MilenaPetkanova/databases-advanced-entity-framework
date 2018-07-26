namespace Employees.App
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Employees.App.Contracts;

    public class CommandInterpreter : ICommandInterpreter
    {
        public ICommand Parse(IServiceProvider serviceProvider, string commandName)
        {
            var commandType = Assembly
                .GetEntryAssembly()
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(ICommand)))
                .SingleOrDefault(t => t.Name.Equals($"{commandName}Command"));

            if (commandType == null)
            {
                throw new InvalidOperationException("Invalid command!");
            }

            var constructor = commandType.GetConstructors().First();
            var constructorArgs = constructor
                .GetParameters()
                .Select(pi => pi.ParameterType)
                .Select(p => serviceProvider.GetService(p))
                .ToArray();

            return (ICommand)constructor.Invoke(constructorArgs);
        }
    }
}
