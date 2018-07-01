using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

public class Startup
{
    public static void Main(string[] args)
    {
        var input = Console.ReadLine();
        var output = new StringBuilder();

        using (var connection = new SqlConnection(Configuration.ConnectionString))
        {
            connection.Open();

            var setTownsInCountryUpperCaseSql = "UPDATE t " +
                                                "SET t.Name = UPPER(t.Name) " +
                                                "FROM Towns AS t " +
                                                "JOIN Countries AS c ON c.Id = t.CountryCode " +
                                                "WHERE c.Name = @countryName ";

            using (var command = new SqlCommand(setTownsInCountryUpperCaseSql, connection))
            {
                command.Parameters.AddWithValue("@countryName", input);

                int affectedTowns = command.ExecuteNonQuery();

                if (affectedTowns == 0)
                {
                    output.AppendLine($"No town names were affected.");
                    return;
                }
            }

            CreateOutput(input, output, connection);

            connection.Close();
        }

        Console.WriteLine(output.ToString().Trim());
    }

    private static void CreateOutput(string input, StringBuilder output, SqlConnection connection)
    {
        var selectTownsInCountry = "SELECT t.Name FROM Towns AS t " +
                                    "JOIN Countries AS c ON c.Id = t.CountryCode " +
                                    "WHERE c.Name = @countryName ";

        using (var command = new SqlCommand(selectTownsInCountry, connection))
        {
            command.Parameters.AddWithValue("@countryName", input);

            using (var reader = command.ExecuteReader())
            {
                int affectedTowns = 0;
                var affectedTownNames = new List<string>();

                while (reader.Read())
                {
                    affectedTownNames.Add(reader["Name"].ToString());
                    affectedTowns++;
                }

                output.AppendLine($"{affectedTowns} town names were affected.");
                output.AppendLine("[" + string.Join(", ", affectedTownNames) + "]");
            }
        }
    }
}
