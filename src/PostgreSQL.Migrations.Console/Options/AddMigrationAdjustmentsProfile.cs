using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

	public record AddMigrationProfileAdjustments {

		[Option ( 'p', "profile", HelpText = "Path to profile file." )]
		public string Profile { get; set; } = "";

	}

}
