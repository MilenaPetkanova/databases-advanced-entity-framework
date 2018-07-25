namespace Employees.App.Commands
{
    using Employees.App.Contracts;
    using Employees.Services.Contracts;
    using System;

    public class EmployeePersonalInfoCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public EmployeePersonalInfoCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        // <employeeId>
        public string Execute(string[] commandArgs)
        {
            var employeeId = int.Parse(commandArgs[0]);

            var employee = employeeController.GetEmployeePersonalInfo(employeeId);

            return $"ID: {employee.EmployeeId} - {employee.FirstName} {employee.LastName} - ${employee.Salary:f2}" +
                    Environment.NewLine +
                    $"Birthday: {employee.Birthday.Value.ToString("dd-MM-yyyy")}" + 
                    Environment.NewLine +
                    $"Address: {employee.Address}";
        }
    }
}
