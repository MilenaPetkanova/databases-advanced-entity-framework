namespace Employees.DtoModels
{
    using Employees.Models;
    using System;

    public class EmployeeWithManagerDto
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal Salary { get; set; }

        public DateTime? Birthday { get; set; }

        public int? ManagerId { get; set; }
        public Employee Manager { get; set; }
    }
}
