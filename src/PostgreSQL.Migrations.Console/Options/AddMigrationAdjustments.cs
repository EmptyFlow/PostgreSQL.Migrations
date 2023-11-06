using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

	public record AddMigrationAdjustments {

		[Option ( 'm', "migrationnumber", Required = true, HelpText = "Migration number for the new migration file(s)." )]
		public int MigrationNumber { get; set; } = 0;

		[Option ( 'p', "parameters", Required = true, HelpText = "List of parameters." )]
		public IEnumerable<string> Parameters { get; set; } = Enumerable.Empty<string> ();

		[Option ( 's', "strategy", Default = "MigrationResolverAttribute", HelpText = "Select strategy for adding migration." )]
		public string Strategy { get; set; } = "";

		[Option ( 'g', "group", HelpText = "You can specify group(s) for new migration." )]
		public string Group { get; set; } = "";

		[Option ( 'i', "issue", HelpText = "You can specify issue for new migration." )]
		public string Issue { get; set; } = "";

		[Option ( 'd', "description", HelpText = "You can specify description for new migration." )]
		public string Description { get; set; } = "";

	}

}
