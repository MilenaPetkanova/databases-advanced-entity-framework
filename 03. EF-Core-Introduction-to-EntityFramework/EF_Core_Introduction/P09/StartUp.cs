// 9. Employee 147

namespace P09
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
                var employee = context.Employees
                        .FirstOrDefault(e => e.EmployeeId.Equals(147));

                var employeeProjectsIds = context.EmployeesProjects
                        .Where(ep => ep.EmployeeId.Equals(employee.EmployeeId))
                        .Select(p => p.ProjectId)
                        .ToArray();

                var projects = context.Projects
                        .Where(p => employeeProjectsIds.Contains(p.ProjectId))
                        .OrderBy(p => p.Name)
                        .Select(p => p.Name)
                        .ToArray();

                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {
                    sw.WriteLine($"{employee.FirstName} {employee.LastName} " +
                        $"- {employee.JobTitle}");

                    foreach (var project in projects)
                    {
                        sw.WriteLine($"{project}");
                    }
                }
            }
        }
    }
}
