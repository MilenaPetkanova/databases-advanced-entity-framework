namespace Employees.App.Commands
{
    using Employees.App.Contracts;
    using Employees.Services.Contracts;

    public class EmployeeInfoCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public EmployeeInfoCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        // <employeeID>
        public string Execute(string[] commandArgs)
        {
            var employeeId = int.Parse(commandArgs[0]);

            var employee = this.employeeController.GetEmployeeInfo(employeeId);

            return $"ID: {employeeId} - {employee.FirstName} {employee.LastName}" +
                $" -  ${employee.Salary:f2}";
        }
    }
}
