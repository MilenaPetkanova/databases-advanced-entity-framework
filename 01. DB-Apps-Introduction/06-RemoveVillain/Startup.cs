using System;
using System.Data.SqlClient;

public class Startup
{
    public static void Main()
    {
        var input = int.Parse(Console.ReadLine());

        using (var connection = new SqlConnection(Configuration.ConnectionString))
        {
            connection.Open();

            var villainId = FindVillain(input, connection);

            if (villainId == 0)
            {
                Console.WriteLine("No such villain was found.");
                return;
            }

            var villainName = GetVillainName(villainId, connection);

            int releasedMinionsCount = DeleteVillainAndReleaseMinions(villainId, connection);

            Console.WriteLine($"{villainName} was deleted.");
            Console.WriteLine($"{releasedMinionsCount} minions were released.");

            connection.Close();
        }
    }

    private static int DeleteVillainAndReleaseMinions(int villainId, SqlConnection connection)
    {
        int affectedRows = 0;

        var deleteFromMinionsVillains = "DELETE FROM MinionsVillains WHERE VillainId = @id";

        using (var command = new SqlCommand(deleteFromMinionsVillains, connection))
        {
            command.Parameters.AddWithValue("@id", villainId);
            int releasedMnions = command.ExecuteNonQuery();
            affectedRows = releasedMnions;
        }

        var deleteFromVillains = "DELETE FROM Villains WHERE Id = @id";

        using (var command = new SqlCommand(deleteFromVillains, connection))
        {
            command.Parameters.AddWithValue("@id", villainId);
            command.ExecuteNonQuery();
        }

        return affectedRows;
    }

    private static string GetVillainName(int villainId, SqlConnection connection)
    {
        var villainName = string.Empty;

        var selectVillainNameSql = "SELECT Name FROM Villains WHERE Id = @id";
        
        using (var command = new SqlCommand(selectVillainNameSql, connection))
        {
            command.Parameters.AddWithValue("@id", villainId);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                villainName = reader[0].ToString();
            }
        }

        return villainName;
    }

    private static int FindVillain(int input, SqlConnection connection)
    {
        var selectVillainById = "SELECT * FROM Villains WHERE Id = @Id";

        using (var command = new SqlCommand(selectVillainById, connection))
        {
            command.Parameters.AddWithValue("@Id", input);

            if (command.ExecuteScalar() == null)
            {
                return 0;
            }

            return (int)command.ExecuteScalar();
        }
    }
}
