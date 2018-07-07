// 15. Remove Towns

namespace P15
{
    using P02_DatabaseFirst.Data;
    using System;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                var townName = Console.ReadLine();

                var adressesInTown = context.Addresses
                    .Where(a => a.Town.Name.Equals(townName)).ToArray();

                var deletedAddresses = 0;

                foreach (var address in adressesInTown)
                {
                    var employeesOnAddress = context.Employees
                        .Where(e => e.Address.AddressId == address.AddressId);

                    foreach (var employee in employeesOnAddress)
                    {
                        employee.AddressId = null;
                    }

                    context.Addresses.Remove(address);
                    deletedAddresses++;
                }

                var town = context.Towns.FirstOrDefault(t => t.Name.Equals(townName));

                context.Towns.Remove(town);

                context.SaveChanges();

                Console.WriteLine($"{deletedAddresses} address in {townName} was deleted");
            }
        }
    }
}
