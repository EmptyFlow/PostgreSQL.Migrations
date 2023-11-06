namespace Database.Migrations.Readers {

	/// <summary>
	/// Reader config file in plain sql resolver.
	/// </summary>
	internal class PlainSqlConfigReader {

		private const string UpFileNameField = "upfile ";

		private const string DownFileNameField = "downfile ";

		private const string MetaFileNameField = "metafile ";

		public static PlainSqlConfigFile Read ( string content ) {
			string? upFileName = null;
			string? downFileName = null;
			string? metaFileName = null;
			foreach ( var item in content.Replace ( "\r", "" ).Split ( "\n" ) ) {
				if ( item == null ) continue;

				if ( item.StartsWith ( UpFileNameField ) ) upFileName = item.Replace ( UpFileNameField, "" );
				if ( item.StartsWith ( DownFileNameField ) ) downFileName = item.Replace ( DownFileNameField, "" );
				if ( item.StartsWith ( MetaFileNameField ) ) metaFileName = item.Replace ( MetaFileNameField, "" );
			}

			var result = new PlainSqlConfigFile ();
			if ( upFileName != null ) result = result with { UpFileName = upFileName };
			if ( downFileName != null ) result = result with { DownFileName = downFileName };
			if ( metaFileName != null ) result = result with { MetaFileName = metaFileName };

			return result;
		}

	}

}
