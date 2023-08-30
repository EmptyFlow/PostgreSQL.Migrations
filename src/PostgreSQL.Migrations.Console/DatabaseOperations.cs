using PostgreSQL.Migrations.Console.Options;
using PostgreSQL.Migrations.Runner;
using PostgreSQL.Migrations.SqlRunner;
using SystemConsole = System.Console;

namespace PostgreSQL.Migrations.Console {

    public class DatabaseOperations {

        static async Task<MigrationRunner> GetRunner ( IEnumerable<string> connectionStrings, List<IMigrationsAsyncResolver> migrationResolvers ) {
            var runner = new MigrationRunner ();
            await runner.LoadMigrationsAsync ( migrationResolvers );
            runner.ConnectionString ( connectionStrings );
            return runner;
        }

        public static async Task<int> ApplyMigrationsToDatabase ( ApplyOptions options ) {
            var migrationResolvers = await MigrationResolver.GetResolvers ( options.Files, options.Group, options.Strategy );

            var runner = await GetRunner ( options.ConnectionStrings, migrationResolvers );

            SystemConsole.WriteLine ( $"Starting operation Apply..." );
            await runner.ApplyMigrationsAsync ( Dependencies.GetService<ISqlRunner> () );
            SystemConsole.WriteLine ( $"Operation Apply is completed!" );

            return 0;
        }

        public static async Task<int> RevertMigrationsToDatabase ( RevertOptions options ) {
            var migrationResolvers = await MigrationResolver.GetResolvers ( options.Files, options.Group, options.Strategy );
            var runner = await GetRunner ( options.ConnectionStrings, migrationResolvers );

            SystemConsole.WriteLine ( $"Starting operation Revert..." );
            await runner.RevertMigrationAsync ( Dependencies.GetService<ISqlRunner> (), options.Migration );
            SystemConsole.WriteLine ( $"Operation Revert is completed!" );

            return 0;
        }

        public static async Task<int> ForceRevertMigrationInDatabase ( ForceRevertOptions options ) {
            var migrationResolvers = await MigrationResolver.GetResolvers ( options.Files, options.Group, options.Strategy );
            var runner = await GetRunner ( options.ConnectionStrings, migrationResolvers );

            SystemConsole.WriteLine ( $"Starting operation Force Revert..." );
            await runner.ForceMigrationAsync ( Dependencies.GetService<ISqlRunner> (), options.Migration );
            SystemConsole.WriteLine ( $"Operation Force Revert is completed!" );

            return 0;
        }

    }

}
