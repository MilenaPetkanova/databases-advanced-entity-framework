using System.Data.SqlClient;

public class Startup
{
    public static void Main()
    {
        var builder = new SqlConnectionStringBuilder();
        builder["Server"] = @"PETKANOVA-PC\SQLEXPRESS";
        builder["Integrated Security"] = "true";

        using (var connection = new SqlConnection(builder.ToString()))
        {
            connection.Open();

            var command = new SqlCommand("CREATE DATABASE MinionsDB", connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        builder["Initial Catalog"] = "MinionsDB";

        using (var connection = new SqlConnection(builder.ToString()))
        {
            connection.Open();

            var createTableCountriesSQL = new SqlCommand("CREATE TABLE Countries (Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50))", connection);
            var createTableTownsSQL = new SqlCommand("CREATE TABLE Towns(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50), CountryCode INT FOREIGN KEY REFERENCES Countries(Id))", connection);
            var createTableMinionsSQL = new SqlCommand("CREATE TABLE Minions(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(30), Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id))", connection);
            var createTableEvelnessFactorSQL = new SqlCommand("CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50))", connection);
            var createTableVilliansSQL = new SqlCommand("CREATE TABLE Villains (Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50), EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))", connection);
            var createTableMinionsVilliansSQL = new SqlCommand("CREATE TABLE MinionsVillains (MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id),CONSTRAINT PK_MinionsVillains PRIMARY KEY (MinionId, VillainId))", connection);

            createTableCountriesSQL.ExecuteNonQuery();
            createTableTownsSQL.ExecuteNonQuery();
            createTableMinionsSQL.ExecuteNonQuery();
            createTableEvelnessFactorSQL.ExecuteNonQuery();
            createTableVilliansSQL.ExecuteNonQuery();
            createTableMinionsVilliansSQL.ExecuteNonQuery();

            var insertIntoCountiresSQL = new SqlCommand("INSERT INTO Countries ([Name]) VALUES ('Bulgaria'),('England'),('Cyprus'),('Germany'),('Norway')", connection);
            var insertIntoTownsSQL = new SqlCommand("INSERT INTO Towns ([Name], CountryCode) VALUES ('Plovdiv', 1),('Varna', 1),('Burgas', 1),('Sofia', 1),('London', 2),('Southampton', 2),('Bath', 2),('Liverpool', 2),('Berlin', 3),('Frankfurt', 3),('Oslo', 4)", connection);
            var insertIntoMinionsSQL = new SqlCommand("INSERT INTO Minions (Name,Age, TownId) VALUES('Bob', 42, 3),('Kevin', 1, 1),('Bob ', 32, 6),('Simon', 45, 3),('Cathleen', 11, 2),('Carry ', 50, 10),('Becky', 125, 5),('Mars', 21, 1),('Misho', 5, 10),('Zoe', 125, 5),('Json', 21, 1)", connection);
            var insertIntoEvelnessFactorSQL = new SqlCommand("INSERT INTO EvilnessFactors (Name) VALUES ('Super good'),('Good'),('Bad'), ('Evil'),('Super evil')", connection);
            var insertIntoVilliansSQL = new SqlCommand("INSERT INTO Villains (Name, EvilnessFactorId) VALUES ('Gru',2),('Victor',1),('Jilly',3),('Miro',4),('Rosen',5),('Dimityr',1),('Dobromir',2)", connection);
            var insertIntoMinionsVilliansSQL = new SqlCommand("INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (4,2),(1,1),(5,7),(3,5),(2,6),(11,5),(8,4),(9,7),(7,1),(1,3),(7,3),(5,3),(4,3),(1,2),(2,1),(2,7)", connection);

            insertIntoCountiresSQL.ExecuteNonQuery();
            insertIntoTownsSQL.ExecuteNonQuery();
            insertIntoMinionsSQL.ExecuteNonQuery();
            insertIntoEvelnessFactorSQL.ExecuteNonQuery();
            insertIntoVilliansSQL.ExecuteNonQuery();
            insertIntoMinionsVilliansSQL.ExecuteNonQuery();

            connection.Close();
        }
    }
}
