using PostgreSQL.Migrations.Console.Options;
using PostgreSQL.Migrations.Runner;
using PostgreSQL.Migrations.SqlRunner;
using SystemConsole = System.Console;

namespace PostgreSQL.Migrations.Console {

    public class DatabaseOperations {

        static async Task<MigrationRunner> GetRunner ( IEnumerable<string> connectionStrings, List<IMigrationsAsyncResolver> migrationResolvers ) {
            var runner = new MigrationRunner ();

            SystemConsole.WriteLine ( $"Started loading migrations..." );

            await runner.LoadMigrationsAsync ( migrationResolvers );

            SystemConsole.WriteLine ( $"Migrations loaded. Founded {runner.CountMigrations ()} migrations." );

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

        public static async Task<int> ApplyMigrationProfileToDatabase ( ApplyProfileOptions options ) {
            var model = await ReadModel<ApplyOptions> ( options.Profile );

            return await ApplyMigrationsToDatabase ( model );
        }

        public static async Task<int> RevertMigrationProfileToDatabase ( RevertProfileOptions options ) {
            var model = await ReadModel<RevertOptions> ( options.Profile );

            model.Migration = options.Migration;

            return await RevertMigrationsToDatabase ( model );
        }

        public static async Task<int> ForceRevertMigrationProfileToDatabase ( ForceRevertProfileOptions options ) {
            var model = await ReadModel<ForceRevertOptions> ( options.Profile );

            model.Migration = options.Migration;

            return await ForceRevertMigrationInDatabase ( model );
        }

        private static async Task<T> ReadModel<T> ( string profile ) where T : DatabaseAdjustments, new() {
            var profileName = string.IsNullOrEmpty ( profile ) ? "migrationprofile" : profile;

            if ( !File.Exists ( profileName ) ) {
                SystemConsole.WriteLine ( $"Profile `{profileName}` is not found!" );
                throw new Exception ( "" );
            }

            var model = ProfileReader.Read<T> ( await File.ReadAllTextAsync ( profileName ) );

            if ( string.IsNullOrEmpty ( model.Strategy ) ) model.Strategy = "MigrationResolverAttribute";

            return model;
        }

    }

}
