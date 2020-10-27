using System;
using DbUp;
using Npgsql;

namespace Demo.Database
{
    internal static class Extensions
    {
        public static void PostgresqlDatabase(this SupportedDatabasesForDropDatabase supportedDatabases, string connectionString)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(connectionString);

            string databaseName = builder.Database;
            builder.Database = "postgres";

            using (NpgsqlConnection connection = new NpgsqlConnection(builder.ToString()))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand($"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = \'{databaseName}\'; DROP DATABASE IF EXISTS \"{databaseName}\"", connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
                Console.WriteLine("Dropped database {0}", databaseName);
            }
        }
    }
}
