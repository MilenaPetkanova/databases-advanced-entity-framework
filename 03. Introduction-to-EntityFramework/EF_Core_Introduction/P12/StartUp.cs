// 12. Increase Salaries

namespace P12
{
    using P02_DatabaseFirst.Data;
    using System.IO;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var departmentIds = new int[] { 1, 2, 4, 11 };

            using (var context = new SoftUniContext())
            {
                var employees = context.Employees
                    .Where(e => departmentIds.Contains(e.DepartmentId))
                    .OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
                    .ToArray();

                foreach (var employee in employees)
                {
                    employee.Salary *= (decimal)1.12;
                }

                context.SaveChanges();

                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {
                    foreach (var employee in employees)
                    {
                        sw.WriteLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:F2})");
                    }
                }
            }
        }
    }
}
