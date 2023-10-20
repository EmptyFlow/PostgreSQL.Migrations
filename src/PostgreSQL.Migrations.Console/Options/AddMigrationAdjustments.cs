using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

	public record AddMigrationAdjustments {

		[Option ( 'm', "migrationnumber", Required = true, HelpText = "Migration number for the new migration file(s)." )]
		public int MigrationNumber { get; set; } = 0;

		[Option ( 'p', "parameters", Required = true, HelpText = "List of parameters." )]
		public IEnumerable<string> Parameters { get; set; } = Enumerable.Empty<string> ();

		[Option ( 's', "strategy", Default = "MigrationResolverAttribute", HelpText = "Select strategy for adding migration." )]
		public string Strategy { get; set; } = "";

	}

}
