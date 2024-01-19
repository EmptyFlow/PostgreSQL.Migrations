using CommandLine;
using PostgreSQL.Migrations.Console;

namespace Migrations.Console.Options {

	[Verb ( "pack", HelpText = "Read migrations and create single file containing all migrations." )]
	public record PackMigrationsOptions {

		[Option ( 'f', "files", Required = true, HelpText = "List of files containing migrations." )]
		public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string> ();

		[Option ( 's', "strategy", Default = MigrationResolver.DefaultStrategy, HelpText = "Select strategy for adding migration." )]
		public string Strategy { get; set; } = "";

		[Option ( 'g', "group", HelpText = "You can specify group(s) for new migration." )]
		public string Group { get; set; } = "";

		[Option ( 'r', "result", HelpText = "Path to result file." )]
		public string ResultPath { get; set; } = "";

	}

}
