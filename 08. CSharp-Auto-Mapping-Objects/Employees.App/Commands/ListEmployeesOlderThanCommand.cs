namespace Employees.App.Commands
{
    using Employees.App.Contracts;
    using System.Text;

    public class ListEmployeesOlderThanCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public ListEmployeesOlderThanCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        // <age>
        public string Execute(string[] commandArgs)
        {
            int age = int.Parse(commandArgs[0]);

            var employees = this.employeeController.ListEmployeesOlderThan(age);

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                var managerLastName = string.Empty;

                if (employee.Manager == null)
                {
                    managerLastName = "[no manager]";
                }
                else
                {
                    managerLastName = employee.Manager.LastName;
                }

                sb.AppendLine($"{employee.FirstName} {employee.LastName} - ${employee.Salary:F2}" +
                                $" - Manager: {managerLastName}");              
            }

            return sb.ToString().Trim();
        }
    }
}
