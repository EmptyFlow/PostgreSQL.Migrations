using Database.Migrations.Client;
using Database.Migrations.Readers;

namespace Database.Migrations {

	/// <summary>
	/// Migrations resolved based on metadata files containing in folders.
	/// </summary>
	public class PlainSqlFileResolver : IMigrationsAsyncResolver {

		private string m_group = "";

		private IEnumerable<string> m_fileNames = Enumerable.Empty<string> ();

		public void SetGroup ( string group ) => m_group = group;

		public void SetConfigFiles ( IEnumerable<string> fileNames ) => m_fileNames = fileNames;

		public Task GenerateNewMigrationAsync ( List<string> parameters, int migrationNumber ) {
			throw new NotImplementedException ();
		}

		public async Task<IEnumerable<AvailableMigration>> GetMigrationsAsync () {
			var result = new List<AvailableMigration> ();

			foreach ( string configFile in m_fileNames ) {
				var config = await ReadConfigFile ( configFile );
				var directories = Directory.EnumerateDirectories ( config.ContainingFolder );
				foreach ( string directory in directories ) {
					var metaPath = Path.Combine ( directory, config.MetaFileName );
					var upFilePath = Path.Combine ( directory, config.UpFileName );
					var downFilePath = Path.Combine ( directory, config.UpFileName );
					if ( !File.Exists ( metaPath ) ) continue;

					var metadata = await ReadMetadataFile ( metaPath );

					if ( !ResolverHelpers.CheckInGroup ( metadata.Group, m_group ) ) continue;

					if ( !File.Exists ( upFilePath ) || !File.Exists ( downFilePath ) ) continue;

					result.Add (
						new AvailableMigration {
							MigrationNumber = metadata.MigrationNumber,
							Issue = metadata.Issue,
							Description = metadata.Description,
							Group = metadata.Group,
							UpScript = await File.ReadAllTextAsync ( upFilePath ),
							DownScript = await File.ReadAllTextAsync ( downFilePath ),
						}
					);
				}
			}

			return result;
		}

		private async Task<PlainSqlConfigFile> ReadConfigFile ( string fileName ) {
			var content = await File.ReadAllTextAsync ( fileName );

			return PlainSqlConfigReader.Read ( content );
		}

		private static async Task<PlainSqlMetadataFile> ReadMetadataFile ( string fileName ) {
			var content = await File.ReadAllTextAsync ( fileName );

			return PlainSqlMetadataReader.Read ( content );
		}

	}

}
