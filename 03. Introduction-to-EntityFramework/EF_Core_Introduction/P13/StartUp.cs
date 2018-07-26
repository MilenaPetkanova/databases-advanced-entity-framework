// 13. Find Employees by First Name Starting With Sa

namespace P13
{
    using Microsoft.EntityFrameworkCore;
    using P02_DatabaseFirst.Data;
    using System.IO;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                var employees = context.Employees
                    .Where(e => EF.Functions.Like(e.FirstName, "Sa%"))
                    .Select(e => new
                    {
                        e.FirstName, e.LastName, e.JobTitle, e.Salary
                    })
                    .OrderBy(e => e.FirstName).ThenBy(e => e.LastName);

                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {
                    foreach (var employee in employees)
                    {
                        sw.WriteLine($"{employee.FirstName} {employee.LastName} " +
                            $"- {employee.JobTitle} - (${employee.Salary:F2})");
                    }
                }
            }
        }
    }
}
