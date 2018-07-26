// 11. Find Latest 10 Projects

namespace P11
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
                var lastProjets = context.Projects
                    .OrderByDescending(p => p.StartDate)
                    .Take(10)
                    .Select(p => new
                    {
                        Name = p.Name,
                        Description = p.Description,
                        StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt")
                    })
                    .OrderBy(p => p.Name);

                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {
                    foreach (var project in lastProjets)
                    {
                        sw.WriteLine($"{project.Name}");
                        sw.WriteLine($"{project.Description}");
                        sw.WriteLine($"{project.StartDate}");
                    }
                }
            }
        }
    }
}
