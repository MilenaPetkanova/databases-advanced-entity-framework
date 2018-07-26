namespace Employees.App.Commands
{
    using Employees.App.Contracts;
    using System.Text;

    public class ManagerInfoCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public ManagerInfoCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        // <employeeId>
        public string Execute(string[] commandArgs)
        {
            var employeeId = int.Parse(commandArgs[0]);

            var managerDto = this.employeeController.GetManagerInfo(employeeId);

            var sb = new StringBuilder();

            sb.AppendLine($"{managerDto.FirstName} {managerDto.LastName} | Employees: {managerDto.ManagerEmployeesCount}");

            foreach (var employee in managerDto.ManagerEmployees)
            {
                sb.AppendLine($"  - {employee.FirstName} {employee.LastName} - ${employee.Salary:F2}");
            }

            return sb.ToString().Trim();
        }
    }
}
