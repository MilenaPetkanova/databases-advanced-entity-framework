// 15. Remove Towns

namespace P15
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
                using (StreamWriter sw = new StreamWriter("../Employees.txt"))
                {

                }
            }
        }
    }
}
