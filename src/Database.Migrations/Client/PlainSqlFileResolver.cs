﻿using Database.Migrations.Client;
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

		private static string? GetStringValueFromParameters ( string name, List<string> parameters, bool isRequired, string valueDescription = "" ) {
			var value = parameters.FirstOrDefault ( a => a.StartsWith ( $"{name}=" ) );
			if ( value == null && isRequired ) throw new Exception ( $"Parameter {name} is required! You need specify it as `{name}=<{valueDescription}>`" );

			return value?.Replace ( $"{name}=", "" ) ?? "";
		}

		public async Task GenerateNewMigrationAsync ( List<string> parameters, int migrationNumber, string issue, string groups, string description ) {
			var configFile = GetStringValueFromParameters ( "config", parameters, true, "configuration file name" );
			var customFolderName = GetStringValueFromParameters ( "migrationfolder", parameters, true, "name of migration folder" );

			if ( !File.Exists ( Path.GetFullPath ( configFile! ) ) ) throw new Exception ( $"Configuration file {configFile} don't exists!" );

			var configuration = await ReadConfigFile ( configFile! );
			var containingFolder = Path.GetDirectoryName ( Path.GetFullPath ( configFile! ) ) ?? throw new ArgumentException ( $"Path to config file {configFile} irrelevant!" );
			if ( !Directory.Exists ( containingFolder ) ) {
				Console.WriteLine ( $"Containing folder {containingFolder} is not exists." );
				return;
			}

			var migrationFolder = Path.Combine ( containingFolder, customFolderName?.Replace ( "{MigrationNumber}", migrationNumber.ToString () ) ?? "" );
			Directory.CreateDirectory ( migrationFolder );
			await File.WriteAllTextAsync ( Path.Combine ( migrationFolder, configuration.UpFileName ), "-- up script" );
			await File.WriteAllTextAsync ( Path.Combine ( migrationFolder, configuration.DownFileName ), "-- down script" );
			var metafileLines = new List<string> {
				$"number {migrationNumber}"
			};
			if ( !string.IsNullOrEmpty ( issue ) ) metafileLines.Add ( $"issue {issue}" );
			if ( !string.IsNullOrEmpty ( groups ) ) metafileLines.Add ( $"group {groups}" );
			if ( !string.IsNullOrEmpty ( description ) ) metafileLines.Add ( $"description {description}" );

			await File.WriteAllTextAsync ( Path.Combine ( migrationFolder, configuration.MetaFileName ), string.Join ( '\n', metafileLines ) );
			Console.WriteLine ( $"Migration files {configuration.UpFileName}, {configuration.DownFileName}, {configuration.MetaFileName} in folder {migrationFolder} created." );
		}

		public async Task<IEnumerable<AvailableMigration>> GetMigrationsAsync () {
			var result = new List<AvailableMigration> ();

			foreach ( string configFile in m_fileNames ) {
				var config = await ReadConfigFile ( configFile );
				var directoryPath = Path.GetDirectoryName ( Path.GetFullPath ( configFile ) ) ?? throw new ArgumentException ( $"Path to config file {configFile} irrelevant!" );
				var directories = Directory.EnumerateDirectories ( directoryPath );
				foreach ( string directory in directories ) {
					var metaPath = Path.Combine ( directory, config.MetaFileName );
					var upFilePath = Path.Combine ( directory, config.UpFileName );
					var downFilePath = Path.Combine ( directory, config.DownFileName );
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

		private static async Task<PlainSqlConfigFile> ReadConfigFile ( string fileName ) {
			var content = await File.ReadAllTextAsync ( fileName );

			return PlainSqlConfigReader.Read ( content );
		}

		private static async Task<PlainSqlMetadataFile> ReadMetadataFile ( string fileName ) {
			var content = await File.ReadAllTextAsync ( fileName );

			return PlainSqlMetadataReader.Read ( content );
		}

	}

}
