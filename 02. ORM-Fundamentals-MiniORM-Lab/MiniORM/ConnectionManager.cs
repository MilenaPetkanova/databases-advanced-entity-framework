namespace MiniORM
{
	using System;

    /// <summary>
    /// Used for wrapping a database connection with a using statement and
    /// automatically closing it when the using statement ends
    /// </summary>
    /// <comments>
    /// ConnectionManager – Simple DatabaseConnection wrapper, which allows it to be wrapped in a using block for opening and closing connections to the database
    /// </comments>
    internal class ConnectionManager : IDisposable
	{
		private readonly DatabaseConnection connection;

		public ConnectionManager(DatabaseConnection connection)
		{
			this.connection = connection;

			this.connection.Open();
		}

		public void Dispose()
		{
			this.connection.Close();
		}
	}
}