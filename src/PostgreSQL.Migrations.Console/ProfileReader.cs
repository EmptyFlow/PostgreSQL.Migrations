using Migrations.Console.Options;
using PostgreSQL.Migrations.Console.Options;

namespace PostgreSQL.Migrations.Console {

	/// <summary>
	/// Class that can read profile files and turn it to 
	/// </summary>
	public static class ProfileReader {

		private const string ConnectionStringField = "constring ";

		private const string FilesField = "file ";

		private const string StrategiesField = "strategy ";

		private const string GroupField = "group ";

		private const string GenerationGroupField = "gengroup ";

		private const string ResultFileField = "resultfile ";

		private const string IssueField = "genissue ";

		private const string DescriptionField = "gendescription ";

		private const string GenerationParameterField = "genparameter ";
		
		private const string MigrationTableField = "migrationtable ";

		public const string DefaultProfileName = "migrationprofile";

		/// <summary>
		/// Read profile from file.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public static T ReadDatabaseAdjustments<T> ( string content ) where T : DatabaseAdjustments, new() {
			var model = new T ();
			var adjustmentsModel = (DatabaseAdjustments) model;
			var connectionStrings = new List<string> ();
			var files = new List<string> ();
			var parameters = new List<string> ();
			foreach ( var item in content.Replace ( "\r", "" ).Split ( "\n" ) ) {
				if ( item.StartsWith ( GenerationParameterField ) ) parameters.Add ( item.Replace ( GenerationParameterField, "" ) );
				if ( item.StartsWith ( ConnectionStringField ) ) connectionStrings.Add ( item.Replace ( ConnectionStringField, "" ) );
				if ( item.StartsWith ( FilesField ) ) files.Add ( item.Replace ( FilesField, "" ) );
				if ( item.StartsWith ( StrategiesField ) ) adjustmentsModel.Strategy = item.Replace ( StrategiesField, "" );
				if ( item.StartsWith ( GroupField ) ) adjustmentsModel.Group = item.Replace ( GroupField, "" );
				if ( item.StartsWith ( MigrationTableField ) ) adjustmentsModel.MigrationTable = item.Replace ( MigrationTableField, "" );
			}

			adjustmentsModel.ConnectionStrings = connectionStrings;
			adjustmentsModel.Files = files;
			adjustmentsModel.Parameters = parameters;

			return model;
		}

		/// <summary>
		/// Read profile from file.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public static T ReadAddMigrationAdjustments<T> ( string content ) where T : AddMigrationAdjustments, new() {
			var model = new T ();
			var adjustmentsModel = (AddMigrationAdjustments) model;
			var parameters = new List<string> ();
			foreach ( var item in content.Replace ( "\r", "" ).Split ( "\n" ) ) {
				if ( item.StartsWith ( GenerationParameterField ) ) parameters.Add ( item.Replace ( GenerationParameterField, "" ) );
				if ( item.StartsWith ( StrategiesField ) ) adjustmentsModel.Strategy = item.Replace ( StrategiesField, "" );
				if ( item.StartsWith ( IssueField ) ) adjustmentsModel.Issue = item.Replace ( IssueField, "" );
				if ( item.StartsWith ( DescriptionField ) ) adjustmentsModel.Description = item.Replace ( DescriptionField, "" );
				if ( item.StartsWith ( GenerationGroupField ) ) adjustmentsModel.Group = item.Replace ( GenerationGroupField, "" );
			}

			adjustmentsModel.Parameters = parameters;

			return model;
		}

		/// <summary>
		/// Read profile from file.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public static T ReadPackMigrations<T> ( string content ) where T : PackMigrationsOptions, new() {
			var model = new T ();
			var packModel = (PackMigrationsOptions) model;
			var files = new List<string> ();
			foreach ( var item in content.Replace ( "\r", "" ).Split ( "\n" ) ) {
				if ( item.StartsWith ( StrategiesField ) ) packModel.Strategy = item.Replace ( StrategiesField, "" );
				if ( item.StartsWith ( FilesField ) ) files.Add ( item.Replace ( FilesField, "" ) );
				if ( item.StartsWith ( GenerationGroupField ) ) packModel.Group = item.Replace ( GenerationGroupField, "" );
				if ( item.StartsWith ( ResultFileField ) ) packModel.ResultPath = item.Replace ( ResultFileField, "" );
			}

			packModel.Files = files;

			return model;
		}

	}

}
