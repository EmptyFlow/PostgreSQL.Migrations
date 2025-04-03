using PostgreSQL.Migrations.Console.Options;
using Database.Migrations;
using SystemConsole = System.Console;
using Migrations.Console.Options;
using System.Text.Json;
using Database.Migrations.JsonSerializers;

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

		public static ISqlRunner GetSqlRunner ( DatabaseAdjustments options ) {
			var sqlRunner = Dependencies.GetService<ISqlRunner> ();
			if ( !string.IsNullOrEmpty ( options.MigrationTable ) ) sqlRunner.SetTableName ( options.MigrationTable );

			return sqlRunner;
		}

		public static async Task ApplyMigrationsToDatabase ( ApplyOptions options ) {
			var migrationResolvers = await MigrationResolver.GetResolvers ( options.Files, options.Group, options.Strategy );

			var runner = await GetRunner ( options.ConnectionStrings, migrationResolvers );

			SystemConsole.WriteLine ( $"Starting operation Apply..." );
			await runner.ApplyMigrationsAsync ( GetSqlRunner ( options ) );
			SystemConsole.WriteLine ( $"Operation Apply is completed!" );
		}

		public static async Task RevertMigrationsToDatabase ( RevertOptions options ) {
			var migrationResolvers = await MigrationResolver.GetResolvers ( options.Files, options.Group, options.Strategy );
			var runner = await GetRunner ( options.ConnectionStrings, migrationResolvers );

			SystemConsole.WriteLine ( $"Starting operation Revert..." );
			await runner.RevertMigrationAsync ( GetSqlRunner ( options ), options.Migration );
			SystemConsole.WriteLine ( $"Operation Revert is completed!" );
		}

		public static async Task ForceRevertMigrationInDatabase ( ForceRevertOptions options ) {
			var migrationResolvers = await MigrationResolver.GetResolvers ( options.Files, options.Group, options.Strategy );
			var runner = await GetRunner ( options.ConnectionStrings, migrationResolvers );

			SystemConsole.WriteLine ( $"Starting operation Force Revert..." );
			await runner.ForceMigrationAsync ( GetSqlRunner ( options ), options.Migration );
			SystemConsole.WriteLine ( $"Operation Force Revert is completed!" );
		}

		public static async Task ApplyMigrationProfileToDatabase ( ApplyProfileOptions options ) {
			var model = await ReadModel<ApplyOptions> ( options.Profile );

			await ApplyMigrationsToDatabase ( model );
		}

		public static async Task RevertMigrationProfileToDatabase ( RevertProfileOptions options ) {
			var model = await ReadModel<RevertOptions> ( options.Profile );

			model.Migration = options.Migration;

			await RevertMigrationsToDatabase ( model );
		}

		public static async Task ForceRevertMigrationProfileToDatabase ( ForceRevertProfileOptions options ) {
			var model = await ReadModel<ForceRevertOptions> ( options.Profile );

			model.Migration = options.Migration;

			await ForceRevertMigrationInDatabase ( model );
		}

		private static async Task<T> ReadModel<T> ( string profile ) where T : DatabaseAdjustments, new() {
			var profileName = string.IsNullOrEmpty ( profile ) ? ProfileReader.DefaultProfileName : profile;

			if ( !File.Exists ( profileName ) ) {
				SystemConsole.WriteLine ( $"Profile `{profileName}` is not found!" );
				throw new Exception ( $"Profile `{profileName}` is not found!" );
			}

			var model = ProfileReader.ReadDatabaseAdjustments<T> ( await File.ReadAllTextAsync ( profileName ) );

			if ( string.IsNullOrEmpty ( model.Strategy ) ) model.Strategy = MigrationResolver.DefaultStrategy;

			return model;
		}

		public static async Task RevertAllMigrations ( RevertAllOptions options ) {
			var migrationResolvers = await MigrationResolver.GetResolvers ( options.Files, options.Group, options.Strategy );
			var runner = await GetRunner ( options.ConnectionStrings, migrationResolvers );

			SystemConsole.WriteLine ( $"Starting operation Revert All..." );
			await runner.RevertAllMigrationsAsync ( GetSqlRunner ( options ) );
			SystemConsole.WriteLine ( $"Operation Revert is completed!" );
		}

		public static async Task RevertAllMigrationsProfile ( RevertAllProfileOptions options ) {
			var model = await ReadModel<RevertAllOptions> ( options.Profile );

			await RevertAllMigrations ( model );
		}

		public static async Task PackMigrations ( PackMigrationsOptions options ) {
			if ( string.IsNullOrEmpty ( options?.ResultPath ) ) {
				SystemConsole.WriteLine ( $"Parameter `resultfile` is not defined!" );
				return;
			}

			var migrationResolvers = await MigrationResolver.GetResolvers ( options.Files, options.Group, options.Strategy );

			var result = new List<AvailableMigration> ();

			foreach ( var resolver in migrationResolvers ) {
				result.AddRange ( await resolver.GetMigrationsAsync () );
			}

			SystemConsole.WriteLine ( $"Starting operation Pack..." );
			var content = JsonSerializer.Serialize ( result, typeof ( List<AvailableMigration> ), PackSerializer.Default );
			try {
				await File.WriteAllTextAsync ( options.ResultPath, content );
				SystemConsole.WriteLine ( $"File saved by path: {Path.GetFullPath ( options.ResultPath )}" );
				SystemConsole.WriteLine ( $"Operation Pack is completed!" );
			} catch ( Exception exception ) {
				var errorMessage = $"Error while saving file `{options.ResultPath}`: {exception.Message}";
				SystemConsole.WriteLine ( errorMessage );
				throw new Exception ( errorMessage );
			}
		}

		public static async Task PackMigrationsProfile ( PackMigrationsProfileOptions options ) {
			var profileName = string.IsNullOrEmpty ( options.Profile ) ? ProfileReader.DefaultProfileName : options.Profile;

			if ( !File.Exists ( profileName ) ) {
				SystemConsole.WriteLine ( $"Profile `{profileName}` is not found!" );
				throw new Exception ( $"Profile `{profileName}` is not found!" );
			}

			var model = ProfileReader.ReadPackMigrations<PackMigrationsOptions> ( await File.ReadAllTextAsync ( profileName ) );

			if ( string.IsNullOrEmpty ( model.Strategy ) ) model.Strategy = MigrationResolver.DefaultStrategy;

			await PackMigrations ( model );
		}

	}

}
