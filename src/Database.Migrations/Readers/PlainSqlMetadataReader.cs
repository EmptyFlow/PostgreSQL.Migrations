using Database.Migrations.Client;

namespace Database.Migrations.Readers {

	/// <summary>
	/// Reader metadata file in plain sql resolver.
	/// </summary>
	internal class PlainSqlMetadataReader {

		private const string MigrationNumberField = "number ";

		private const string GroupField = "group ";

		private const string IssueField = "issue ";

		private const string DescriptionField = "description ";

		public static PlainSqlMetadataFile Read ( string content ) {
			var model = new PlainSqlMetadataFile ();
			foreach ( var item in content.Replace ( "\r", "" ).Split ( "\n" ) ) {
				if ( item == null ) continue;

				if ( item.StartsWith ( MigrationNumberField ) ) model.MigrationNumber = Convert.ToInt32 ( item.Replace ( MigrationNumberField, "" ) );
				if ( item.StartsWith ( GroupField ) ) model.Group = item.Replace ( GroupField, "" );
				if ( item.StartsWith ( IssueField ) ) model.Issue = item.Replace ( IssueField, "" );
				if ( item.StartsWith ( DescriptionField ) ) model.Description = item.Replace ( DescriptionField, "" );
			}

			return model;
		}

	}

}
