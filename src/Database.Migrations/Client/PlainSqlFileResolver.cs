using Database.Migrations.Client;
using Database.Migrations.Readers;

namespace Database.Migrations {

	/// <summary>
	/// Migrations resolved based on metadata files containing in folders.
	/// </summary>
	public class PlainSqlFileResolver : IMigrationsAsyncResolver {

		private string m_group = "";

		private List<string> m_parameters = new List<string> ();

		public void SetGroup ( string group ) => m_group = group;

		public void SetParameters ( IEnumerable<string> parameters ) => m_parameters = parameters.ToList ();

		private static string? GetStringValueFromParameters ( string name, List<string> parameters, bool isRequired, string valueDescription = "" ) {
			var value = parameters.FirstOrDefault ( a => a.StartsWith ( $"{name}=" ) );
			if ( value == null && isRequired ) throw new Exception ( $"Parameter {name} is required! You need specify it as `{name}=<{valueDescription}>`" );

			return value?.Replace ( $"{name}=", "" ) ?? "";
		}

		public async Task GenerateNewMigrationAsync ( List<string> parameters, int migrationNumber, string issue, string groups, string description ) {
			var upFileName = GetStringValueFromParameters ( "upfilename", parameters, false, "Up file name" ) ?? "up.sql";
			var downFileName = GetStringValueFromParameters ( "downfilename", parameters, false, "Down file name" ) ?? "down.sql";
			var metaFileName = GetStringValueFromParameters ( "metafilename", parameters, false, "Metadata file name" ) ?? "metadata";
			var customFolderName = GetStringValueFromParameters ( "migrationfolder", parameters, false, "name of migration folder" ) ?? "Migration{MigrationNumber}";
			var containingFolder = GetStringValueFromParameters ( "containingFolder", parameters, true, "path to folder where need to store migrations" ) ?? "";

			var migrationFolder = Path.Combine ( containingFolder, customFolderName?.Replace ( "{MigrationNumber}", migrationNumber.ToString () ) ?? "" );
			if ( !Directory.Exists ( migrationFolder ) ) {
				try {
					Directory.CreateDirectory ( migrationFolder );
				} catch ( Exception ex ) {
					Console.WriteLine ( $"Can't create directory {migrationFolder}: {ex.Message}" );
					return;
				}
			}

			await SaveFileContent ( migrationFolder, upFileName, "-- replace on up script" );
			await SaveFileContent ( migrationFolder, downFileName, "-- replace on down script" );

			var metafileLines = new List<string> {
				$"number {migrationNumber}"
			};
			if ( !string.IsNullOrEmpty ( issue ) ) metafileLines.Add ( $"issue {issue}" );
			if ( !string.IsNullOrEmpty ( groups ) ) metafileLines.Add ( $"group {groups}" );
			if ( !string.IsNullOrEmpty ( description ) ) metafileLines.Add ( $"description {description}" );
			await File.WriteAllTextAsync ( Path.Combine ( migrationFolder, metaFileName ), string.Join ( '\n', metafileLines ) );

			Console.WriteLine ( $"Migration files {upFileName}, {downFileName} in folder {migrationFolder} created." );
		}

		public async Task<IEnumerable<AvailableMigration>> GetMigrationsAsync () {
			var result = new List<AvailableMigration> ();

			var upFileName = GetStringValueFromParameters ( "upfilename", m_parameters, false, "Up file name" ) ?? "up.sql";
			var downFileName = GetStringValueFromParameters ( "downfilename", m_parameters, false, "Down file name" ) ?? "down.sql";
			var metaFileName = GetStringValueFromParameters ( "metafilename", m_parameters, false, "Metadata file name" ) ?? "metadata";
			var customFolderName = GetStringValueFromParameters ( "migrationfolder", m_parameters, false, "name of migration folder" ) ?? "Migration{MigrationNumber}";
			var containingFolder = GetStringValueFromParameters ( "containingFolder", m_parameters, true, "path to folder where need to store migrations" ) ?? "";

			var directoryPath = Path.GetFullPath ( containingFolder ) ?? throw new ArgumentException ( $"Path to config file {containingFolder} irrelevant!" );
			var directories = Directory.EnumerateDirectories ( directoryPath );
			foreach ( string directory in directories ) {
				var metaPath = Path.Combine ( directory, metaFileName );
				var upFilePath = Path.Combine ( directory, upFileName );
				var downFilePath = Path.Combine ( directory, downFileName );
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

			return result;
		}

		private static async Task SaveFileContent ( string folder, string fileName, string content ) {
			try {
				await File.WriteAllTextAsync ( Path.Combine ( folder, fileName ), content );
			} catch ( Exception exception ) {
				Console.WriteLine ( $"Can't create file {fileName} {exception.Message}" );
			}
		}

		private static async Task<PlainSqlMetadataFile> ReadMetadataFile ( string fileName ) {
			var content = await File.ReadAllTextAsync ( fileName );

			return PlainSqlMetadataReader.Read ( content );
		}

	}

}
