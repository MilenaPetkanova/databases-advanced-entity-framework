// 10. Departments with More Than 5 Employees

namespace P10
{
    using P02_DatabaseFirst.Data;
    using System.IO;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                var departments = context.Departments
                    .Where(d => d.Employees.Count() > 5)
                    .Select(d => new
                    {
                        Name = d.Name,
                        EmployeesCount = d.Employees.Count(),
                        Employees = d.Employees,
                        ManagerName = d.Manager.FirstName + " " + d.Manager.LastName
                    })
                    .OrderBy(d => d.EmployeesCount)
                    .ThenBy(d => d.Name);

                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {
                    foreach (var department in departments)
                    {
                        sw.WriteLine($"{department.Name} - {department.ManagerName}");

                        var employees = department.Employees
                            .Select(e => new
                            {
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                JobTitle = e.JobTitle
                            })
                            .OrderBy(e => e.FirstName).ThenBy(e => e.LastName);

                        foreach (var employee in employees)
                        {
                            sw.WriteLine($"{employee.FirstName} {employee.LastName}" +
                                $" - {employee.JobTitle}");
                        }

                        sw.WriteLine("----------");
                    }
                }
            }
        }
    }
}
