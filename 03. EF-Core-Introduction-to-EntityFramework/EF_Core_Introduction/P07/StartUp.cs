// 7. Employees and Projects

namespace P07
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
                var employeesProjects = context.Employees.Where(e =>
                                        e.EmployeesProjects.Any(ep =>
                                        ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                    .Select(e => new
                    {
                        EmployeeName = e.FirstName + " " + e.LastName,
                        ManagerName = e.Manager.FirstName + " " + e.Manager.LastName,
                        Projects = e.EmployeesProjects.Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            ProjectStartDate = ep.Project.StartDate,
                            ProjectEndDate = ep.Project.EndDate
                        })
                        .ToArray()
                    })
                    .Take(30);

                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {
                    foreach (var e in employeesProjects)
                    {
                        sw.WriteLine($"{e.EmployeeName} - Manager: {e.ManagerName}");

                        foreach (var p in e.Projects)
                        {
                            sw.WriteLine($"--{p.ProjectName} - {p.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt")} " +
                                $"- {p.ProjectEndDate?.ToString("M/d/yyyy h:mm:ss tt") ?? "not finished"}");
                        }
                    }
                }
            }
        }
    }
}
