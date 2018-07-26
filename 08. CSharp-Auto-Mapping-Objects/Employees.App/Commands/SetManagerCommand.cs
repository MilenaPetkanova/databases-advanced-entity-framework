namespace Employees.App.Commands
{
    using Employees.App.Contracts;
    using System;

    public class SetManagerCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public SetManagerCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        // <employeeId> <managerId> 
        public string Execute(string[] commandArgs)
        {
            var employeeId = int.Parse(commandArgs[0]);
            var managerId = int.Parse(commandArgs[1]);

            this.employeeController.SetManager(employeeId, managerId);

            return $"Employee with id {managerId} is set manager to employee with id {employeeId}";
        }
    }
}
