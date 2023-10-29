namespace Database.Migrations {

	public record PlainSqlConfigFile {

		public string ContainingFolder { get; init; } = "";

		public string UpFileName { get; init; } = "up.sql";

		public string DownFileName { get; init; } = "down.sql";

		public string MetaFileName { get; init; } = "metadata";

	}

}
