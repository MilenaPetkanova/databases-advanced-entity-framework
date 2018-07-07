using System;
using System.Data;
using System.Data.SqlClient;

public class Startup
{
    public static void Main()
    {
        var id = int.Parse(Console.ReadLine());

        using (var connection = new SqlConnection(Configuration.ConnectionString))
        {
            connection.Open();

            using (var command = new SqlCommand("usp_GetOlder", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Id", id));

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine(reader["Name"] + " - " + reader["Age"]);
                }
            }

            connection.Close();
        }
    }
}
