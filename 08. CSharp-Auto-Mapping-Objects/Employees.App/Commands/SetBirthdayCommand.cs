namespace Employees.App.Commands
{
    using Employees.App.Contracts;
    using System;

    public class SetBirthdayCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public SetBirthdayCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        public string Execute(string[] commandArgs)
        {
            // <employeeId> <date: "dd-MM-yyyy"> 
            var employeeId = int.Parse(commandArgs[0]);
            var date = DateTime.ParseExact(commandArgs[1], "dd-MM-yyyy", null);

            this.employeeController.SetBirthday(employeeId, date);

            return $"Birthday seccessfully changed to {commandArgs[1]}";
        }
    }
}
