namespace Employees.DtoModels
{
    using Employees.Models;

    public class EmployeeDto
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal Salary { get; set; }

        public Employee Employee { get; set; }
    }
}
