using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class Startup
{
    public static void Main()
    {
        using (var connection = new SqlConnection(Configuration.ConnectionString))
        {
            connection.Open();

            var selectMinionNames = "SELECT Name FROM Minions";

            var minionNames = new List<string>();

            using (var command = new SqlCommand(selectMinionNames, connection))
            {
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    minionNames.Add(reader[0].ToString());
                }
            }

            for (int i = 0; i < minionNames.Count / 2; i++)
            {
                Console.WriteLine(minionNames[i]);

                var lastNameIndex = minionNames.Count - 1;
                Console.WriteLine(minionNames[lastNameIndex - i]);
            }

            if (minionNames.Count % 2 == 1)
            {
                int middleNameIndex = minionNames.Count / 2;
                Console.WriteLine(minionNames[middleNameIndex]);
            }

            connection.Close();
        }
    }
}
