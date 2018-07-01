using System;
using System.Data.SqlClient;

public class Startup
{
    public static void Main()
    {
        var minionInput = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var minionName = minionInput[1];
        var minionAge = int.Parse(minionInput[2]);
        var minionTown = minionInput[3];

        var villainInput = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var villainName = villainInput[1];

        using (var connection = new SqlConnection(Configuration.ConnectionString))
        {
            connection.Open();

            int townId = GetTownId(minionTown, connection);
            int villainId = GetVillainId(villainName, connection);
            int minionId = InsertMinionAndGetId(minionName, minionAge, townId, connection);

            try
            {
                AssignMinionToVillain(minionId, villainId, connection);
                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            connection.Close();
        }
    }

    private static void AssignMinionToVillain(int minionId, int villainId, SqlConnection connection)
    {
        var insertMinionToVillain = "INSERT INTO MinionsVillains (MinionId, VillainId) " +
                                    "VALUES (@minionId, @villainId)";

        using (var command = new SqlCommand(insertMinionToVillain, connection))
        {
            command.Parameters.AddWithValue("@minionId", minionId);
            command.Parameters.AddWithValue("@villainId", villainId);
            command.ExecuteNonQuery();
        }
    }

    private static int InsertMinionAndGetId(string minionName, int minionAge, int townId, SqlConnection connection)
    {
        var insertIntoMinionsSql = "INSERT INTO Minions (Name, Age, TownId) " +
                                    "VALUES (@name, @age, @townId)";

        using (var command = new SqlCommand(insertIntoMinionsSql, connection))
        {
            command.Parameters.AddWithValue("@name", minionName);
            command.Parameters.AddWithValue("@age", minionAge);
            command.Parameters.AddWithValue("@townId", townId);
            command.ExecuteNonQuery();
        }

        var minionNameSql = "SELECT Id, Name FROM Minions WHERE Name = @name";

        using (var command = new SqlCommand(minionNameSql, connection))
        {
            command.Parameters.AddWithValue("@name", minionName);
            return (int)command.ExecuteScalar();
        }
    }

    private static int GetVillainId(string villainName, SqlConnection connection)
    {
        var villainNameSql = "SELECT Id, Name FROM Villains WHERE Name = @villainName";

        using (var command = new SqlCommand(villainNameSql, connection))
        {
            command.Parameters.AddWithValue("@villainName", villainName);

            if (command.ExecuteScalar() == null)
            {
                InsertIntoVallains(villainName, connection);
            }

            return (int)command.ExecuteScalar();
        }
        
    }

    private static void InsertIntoVallains(string villainName, SqlConnection connection)
    {
        var insertIntoVillainsSql = "INSERT INTO Villains (Name, EvilnessFactorId)" +
                                $"VALUES(@villainName, 4)";

        using (var commnd = new SqlCommand(insertIntoVillainsSql, connection))
        {
            commnd.Parameters.AddWithValue("@villainName", villainName);
            commnd.ExecuteNonQuery();

        }

        Console.WriteLine($"Villain {villainName} was added to the database.");
    }

    private static int GetTownId(string minionTown, SqlConnection connection)
    {
        var townNameSql = "SELECT Id, Name FROM Towns " +
                            $"WHERE Name = @townName";

        using (SqlCommand command = new SqlCommand(townNameSql, connection))
        {
            command.Parameters.AddWithValue("@townName", minionTown);

            if (command.ExecuteScalar() == null)
            {
                InsertIntoTown(minionTown, connection);
            }

            return (int)command.ExecuteScalar();
        }
    }

    private static void InsertIntoTown(string minionTown, SqlConnection connection)
    {
        var insertIntoTown = $"INSERT INTO Towns (Name) VALUES(@townName)";

        using (SqlCommand command = new SqlCommand(insertIntoTown, connection))
        {
            command.Parameters.AddWithValue("@townName", minionTown);
            command.ExecuteNonQuery();
        }

        Console.WriteLine($"Town {minionTown} was added to the database.");
    }
}
