namespace Employees.App.Contracts
{
    using System;

    public interface ICommandInterpreter
    {
        ICommand Parse(IServiceProvider serviceProvider, string commandName);
    }
}
