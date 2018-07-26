// 6. Adding a New Address and Updating Employee

namespace P06
{
    using P02_DatabaseFirst.Data;
    using P02_DatabaseFirst.Data.Models;
    using System.IO;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                var newAdress = new Address
                {
                    AddressText = "Vitoshka 15",
                    TownId = 4,
                };

                var employee = context.Employees.FirstOrDefault(e => e.LastName.Equals("Nakov"));

                employee.Address = newAdress;

                context.SaveChanges();

                var employeesAddresses = context.Employees
                    .OrderByDescending(e => e.AddressId)
                    .Take(10)
                    .Select(x => x.Address.AddressText)
                    .ToList();

                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {
                    employeesAddresses.ForEach(x => sw.WriteLine(x));
                }
            }
        }
    }
}
