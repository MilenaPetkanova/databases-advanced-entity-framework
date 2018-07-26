namespace Employees.DtoModels
{
    using Employees.Models;
    using System.Collections.Generic;

    public class ManagerDto
    {
        public ManagerDto()
        {
            this.ManagerEmployees = new List<Employee>();
        }

        public int EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<Employee> ManagerEmployees { get; set; }

        public int ManagerEmployeesCount => this.ManagerEmployees.Count;
    }
}
