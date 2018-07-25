namespace Employees.Services
{
    using Microsoft.EntityFrameworkCore;

    using Employees.Services.Contracts;
    using Employees.Data;

    public class DbInitializerService : IDbInitializerService
    {
        private readonly EmployeeContext context;

        public DbInitializerService(EmployeeContext context)
        {
            this.context = context;
        }

        public void InitializeDatabase()
        {
            this.context.Database.Migrate();
        }
    }
}
