// 4. Employees with Salary Over 50 000

namespace P04
{
    using P02_DatabaseFirst.Data;
    using System.IO;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var dbContext = new SoftUniContext())
            {
                var employeesInfo = dbContext.Employees
                    .Select(x => new
                    {
                        FirstName = x.FirstName,
                        Salary = x.Salary
                    })
                    .Where(x => x.Salary > 50000)
                    .OrderBy(e => e.FirstName)
                    .ToArray();

                using (StreamWriter sw = new StreamWriter("../EmployeesInfo.txt"))
                {
                    foreach (var employee in employeesInfo)
                    {
                        sw.WriteLine($"{employee.FirstName}");
                    }
                }
            }
        }
    }
}
