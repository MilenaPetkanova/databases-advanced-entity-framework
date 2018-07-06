// 5. Employees from Research and Development

namespace P05
{
    using P02_DatabaseFirst.Data;
    using System.Linq;
    using System.IO;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var departmentName = "Research and Development";

            using (var context = new SoftUniContext())
            {
                var employees = context.Employees
                    .Where(e => e.Department.Name.Equals(departmentName))
                    .OrderBy(e => e.Salary)
                    .ThenByDescending(e => e.FirstName)
                    .Select(e => new
                    {
                        e.FirstName, e.LastName, e.Salary, e.Department
                    })
                    .ToArray();

                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {
                    foreach (var employee in employees)
                    {
                        sw.WriteLine($"{employee.FirstName} {employee.LastName} " +
                            $"from {employee.Department.Name} - ${employee.Salary:F2}");
                    }
                }
            }
        }
    }
}
