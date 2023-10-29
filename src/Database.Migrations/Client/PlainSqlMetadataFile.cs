namespace Database.Migrations.Client {

	public class PlainSqlMetadataFile {

		public int MigrationNumber { get; set; }

		public string Group { get; set; } = "";

		public string Issue { get; set; } = "";

		public string Description { get; set; } = "";

	}

}
