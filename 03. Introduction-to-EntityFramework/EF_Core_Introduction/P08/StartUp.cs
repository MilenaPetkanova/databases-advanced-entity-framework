// 8. Addresses by Town

namespace P08
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
                var adresses = context.Addresses
                    .Select(a => new
                    {
                        AddressText = a.AddressText,
                        TownName = a.Town.Name,
                        EmployeeCount = a.Employees.Count()
                    })
                    .OrderByDescending(a => a.EmployeeCount)
                    .ThenBy(a => a.TownName)
                    .ThenBy(a => a.EmployeeCount)
                    .Take(10)
                    .ToArray();
                    
                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {
                    foreach (var a in adresses)
                    {
                        sw.WriteLine($"{a.AddressText}, {a.TownName} " +
                            $"- {a.EmployeeCount} employees");
                    }
                }
            }
        }
    }
}
