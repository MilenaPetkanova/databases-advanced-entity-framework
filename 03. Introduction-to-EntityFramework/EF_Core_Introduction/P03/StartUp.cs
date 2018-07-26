// 3. Employees Full Information

namespace P03
{
    using P02_DatabaseFirst.Data;
    using P02_DatabaseFirst.Data.Models;
    using System.IO;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var dbContext = new SoftUniContext())
            {
                var employeesInfo = dbContext.Employees
                    .OrderBy(e => e.EmployeeId)
                    .Select(x => new Employee()
                    {
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        JobTitle = x.JobTitle,
                        Salary = x.Salary
                    })
                    .ToArray();

                using (StreamWriter sw = new StreamWriter("../EmployeesInfo.txt"))
                {
                    foreach (var employee in employeesInfo)
                    {
                        sw.WriteLine($"{employee.FirstName} {employee.LastName} " +
                            $"{employee.MiddleName} {employee.JobTitle} " +
                            $"{employee.Salary:F2}");
                    }
                }
            }

        }
    }
}
