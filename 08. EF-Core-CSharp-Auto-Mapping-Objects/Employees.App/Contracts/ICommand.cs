namespace Employees.App.Contracts
{
    public interface ICommand
    {
        string Execute(string[] commandArgs);
    }
}
