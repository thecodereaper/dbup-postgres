using System;
using System.Linq;
using DbUp;
using DbUp.Engine;
using DbUp.ScriptProviders;

namespace Demo.Database
{
    internal sealed class Program
    {
        private const string SRC_PATH = "./Scripts/src";

        internal static int Main(string[] args)
        {
            string connectionString = args.FirstOrDefault();
            bool dropDatabase = Convert.ToBoolean(args[1]);
            DatabaseUpgradeResult result = RunDatabaseUpdate(connectionString, dropDatabase);

            return !result.Successful ? ShowError(result.Error) : ShowSuccess();
        }

        private static DatabaseUpgradeResult RunDatabaseUpdate(string connectionString, bool dropDatabase)
        {
            if (dropDatabase)
                DropDatabase.For.PostgresqlDatabase(connectionString);

            EnsureDatabase.For.PostgresqlDatabase(connectionString);

            UpgradeEngine upgradeEngine = DeployChanges
                .To
                .PostgresqlDatabase(connectionString)
                .WithScriptsFromFileSystem
                (
                    SRC_PATH, new FileSystemScriptOptions
                    {
                        IncludeSubDirectories = true
                    }
                )
                .WithTransactionPerScript()
                .WithVariablesDisabled()
                .Build();

            return upgradeEngine.PerformUpgrade();
        }

        private static int ShowError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ResetColor();

            return -1;
        }

        private static int ShowSuccess()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();

            return 0;
        }
    }
}
