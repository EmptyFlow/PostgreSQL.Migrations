namespace PostgreSQL.Migrations.Console.Options {

	/// <summary>
	/// Database adjustments.
	/// </summary>
	public class DatabaseAdjustments {

		//[Option ( 'f', "files", Required = true, HelpText = "List of files containing migrations." )]
		public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string> ();

		//[Option ( 'c', "connectionStrings", Required = true, HelpText = "List of connection strings to which migrations will be applied." )]
		public IEnumerable<string> ConnectionStrings { get; set; } = Enumerable.Empty<string> ();

		public string Strategy { get; set; } = MigrationResolver.DefaultStrategy;

		public string Group { get; set; } = "";

		public string MigrationTable { get; set; } = "";

	}

}
