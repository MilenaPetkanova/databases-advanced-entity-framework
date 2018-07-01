using System;
using System.Data.SqlClient;

public class Startup
{
    public static void Main()
    {
        using (var connection = new SqlConnection(Configuration.ConnectionString))
        {
            connection.Open();

            var villainsWithMinionsCount = "SELECT V.Name, COUNT(MinionId) AS [MinionCount] " +
                "FROM MinionsVillains AS ref " +
                "JOIN Villains AS v ON v.Id = ref.VillainId " +
                "JOIN Minions AS m ON m.Id = ref.MinionId " +
                "GROUP BY VillainId, V.Name " +
                "HAVING COUNT(MinionId) > 3 " +
                "ORDER BY COUNT(MinionId) DESC";

            var command = new SqlCommand(villainsWithMinionsCount, connection);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader[0]} - {reader[1]}");
            }

            connection.Close();
        }
    }
}

