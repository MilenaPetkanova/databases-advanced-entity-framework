namespace Employees.App.Commands
{
    using Employees.App.Contracts;
    using Employees.Services.Contracts;
    using System.Linq;

    public class SetAddressCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public SetAddressCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        public string Execute(string[] commandArgs)
        {
            // <employeeId> <address> 
            var employeeId = int.Parse(commandArgs[0]);
            var address = string.Join(" ", commandArgs.Skip(1).ToArray());

            this.employeeController.SetAddress(employeeId, address);

            return $"The address is changes successfully to {address}";
        }
    }
}
