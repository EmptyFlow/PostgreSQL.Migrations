using PostgreSQL.Migrations.Console.Options;
using SystemConsole = System.Console;

namespace PostgreSQL.Migrations.Console {

	public class AddMigrationOperations {

		public static async Task<int> AddMigration ( AddMigrationOptions options ) {
			SystemConsole.WriteLine ( $"Started add migration..." );

			var resolver = MigrationResolver.GetResolver ( options.Strategy );
			if ( resolver == null ) return 1;

			SystemConsole.WriteLine ( $"Trying to use a strategy: {options.Strategy}..." );

			await resolver.GenerateNewMigrationAsync ( options.Parameters.ToList (), options.MigrationNumber, options.Issue, options.Group, options.Description );

			SystemConsole.WriteLine ( $"Migration with number {options.MigrationNumber} created." );

			return 0;
		}

		public static async Task<int> AddMigrationProfile ( AddMigrationProfileOptions options ) {
			var model = await ReadModel<AddMigrationOptions> ( options.Profile );

			model.MigrationNumber = options.MigrationNumber;

			return await AddMigration ( model );
		}

		private static async Task<T> ReadModel<T> ( string profile ) where T : AddMigrationAdjustments, new() {
			var profileName = string.IsNullOrEmpty ( profile ) ? ProfileReader.DefaultProfileName : profile;

			if ( !File.Exists ( profileName ) ) {
				SystemConsole.WriteLine ( $"Profile `{profileName}` is not found!" );
				throw new Exception ( $"Profile `{profileName}` is not found!" );
			}

			var model = ProfileReader.ReadAddMigrationAdjustments<T> ( await File.ReadAllTextAsync ( profileName ) );

			if ( string.IsNullOrEmpty ( model.Strategy ) ) model.Strategy = MigrationResolver.DefaultStrategy;

			return model;
		}

	}

}
