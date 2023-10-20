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

		private const string GenerationParameterField = "genparameter ";

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
			foreach ( var item in content.Replace ( "\r", "" ).Split ( "\n" ) ) {
				if ( item.StartsWith ( ConnectionStringField ) ) connectionStrings.Add ( item.Replace ( ConnectionStringField, "" ) );
				if ( item.StartsWith ( FilesField ) ) files.Add ( item.Replace ( FilesField, "" ) );
				if ( item.StartsWith ( StrategiesField ) ) adjustmentsModel.Strategy = item.Replace ( StrategiesField, "" );
				if ( item.StartsWith ( GroupField ) ) adjustmentsModel.Group = item.Replace ( GroupField, "" );
			}

			adjustmentsModel.ConnectionStrings = connectionStrings;
			adjustmentsModel.Files = files;

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
			}

			adjustmentsModel.Parameters = parameters;

			return model;
		}

	}

}
