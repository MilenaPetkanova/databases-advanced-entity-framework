namespace Employees.App.Commands
{
    using Employees.App.Contracts;
    using Employees.Services.Contracts;
    using Employees.DtoModels;

    public class AddEmployeeCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public AddEmployeeCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        //<firstName> <lastName> <salary> 
        public string Execute(string[] commandArgs)
        {
            string firstName = commandArgs[0];
            string lastName = commandArgs[1];
            decimal salary = decimal.Parse(commandArgs[2]);

            var employeeDto = new EmployeeDto
            {
                FirstName = firstName,
                LastName = lastName,
                Salary = salary
            };

            this.employeeController.AddEmployee(employeeDto);

            return $"Employee {firstName} {lastName} successfully added.";
        }
    }
}
