using System;
using System.Data.SqlClient;
using System.Linq;

public class Startup
{
    public static void Main(string[] args)
    {
        var input = Console.ReadLine().Split().Select(int.Parse).ToArray();

        using (var connection = new SqlConnection(Configuration.ConnectionString))
        {
            connection.Open();

            var updateMinionAgeSql = "UPDATE Minions " +
                                        "SET Age += 1, Name = UPPER(Name) " +
                                        "WHERE Id = @id ";

            foreach (var id in input)
            {
                using (var command = new SqlCommand(updateMinionAgeSql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }

            var selectAllMinionsNameAgeSql = "SELECT Name, Age FROM Minions";

            using (var command = new SqlCommand(selectAllMinionsNameAgeSql, connection))
            {
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"{reader[0]} {reader[1]}");
                }
            }

            connection.Close();
        }
    }
}
