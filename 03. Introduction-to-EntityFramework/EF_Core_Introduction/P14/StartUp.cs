// 14. Delete Project by Id

namespace P14
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
                var projectsEmployees = context.EmployeesProjects.Where(p => p.ProjectId.Equals(2));

                foreach (var projectEmployee in projectsEmployees)
                {
                    context.EmployeesProjects.Remove(projectEmployee);
                }

                var project = context.Projects.FirstOrDefault(p => p.ProjectId.Equals(2));

                context.Projects.Remove(project);

                context.SaveChanges();

                var projectNames = context.Projects
                    .Take(10)
                    .Select(p => p.Name)
                    .ToList();

                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {
                    projectNames.ForEach(p => sw.WriteLine(p));
                }
            }
        }
    }
}
