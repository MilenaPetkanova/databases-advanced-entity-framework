using System;
using System.Data.SqlClient;
using System.Text;

public class Startup
{
    public static void Main()
    {
        var input = int.Parse(Console.ReadLine());
        var ouput = new StringBuilder();

        using (var connection = new SqlConnection(Configuration.ConnectionString))
        {
            connection.Open();

            var villainsWithMinions = "SELECT v.Name AS VillainName, m.Name AS MinionName, m.Age AS MinionAge " +
                                        "FROM MinionsVillains AS ref " +
                                        "JOIN Villains AS v ON v.Id = ref.VillainId " +
                                        "JOIN Minions AS m ON m.Id = ref.MinionId " +
                                        $"WHERE v.Id = {input}" +
                                        "ORDER BY m.Name";

            using (var command = new SqlCommand(villainsWithMinions, connection))
            {
                var villainName = command.ExecuteScalar();

                if (villainName == null)
                {
                    Console.WriteLine($"No villain with ID {input} exists in the database.");
                    return;
                }

                ouput.AppendLine($"Villain: {villainName}");

                var reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    Console.WriteLine("(no minions)");
                    return;
                }

                var minionsCounter = 1;
                while (reader.Read())
                {
                    ouput.AppendLine($"{minionsCounter}. {reader["MinionName"]} {reader["MinionAge"]}");
                    minionsCounter++;
                }
            }

            connection.Close();
        }

        Console.WriteLine(ouput.ToString().Trim());
    }
}
