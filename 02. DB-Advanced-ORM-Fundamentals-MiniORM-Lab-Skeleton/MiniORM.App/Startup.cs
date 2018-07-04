using System.Linq;
using MiniORM.App.Data.Entities;
using MiniORM.App.Data;

public class Startup
{
    public static void Main()
    {
        var connectionString = @"Server=PETKANOVA-PC\SQLEXPRESS;Initial Catalog=MiniORM;Integrated Security=True;";

        var context = new SoftUniDbContext(connectionString);

        context.Departments.Add(new Department
        {
            Name = "Engineer"
        });

        context.Departments.Add(new Department
        {
            Name = "Writer"
        });

        context.Employees.Add(new Employee
        {
            FirstName = "Sami",
            LastName = "Unmodified",
            DepartmentId = context.Departments.First(d => d.Name == "Writer").Id,
            IsEmployed = true
        });

        context.Employees.Add(new Employee
        {
            FirstName = "Mena",
            LastName = "Petkanova",
            DepartmentId = context.Departments.First(d => d.Name == "Engineer").Id,
            IsEmployed = true
        });

        var employee = context.Employees.FirstOrDefault(e => e.Id == 2);
        employee.LastName = "Petkanov";
        employee.DepartmentId = 3;

        context.SaveChanges();
    }
}
