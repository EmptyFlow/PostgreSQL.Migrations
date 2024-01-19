using CommandLine;

namespace Migrations.Console.Options {

	[Verb ( "pack-profile", HelpText = "Read migrations and create single file containing all migrations." )]
	public record PackMigrationsProfileOptions {

		[Option ( 'p', "profile", Required = true, HelpText = "Path to profile file." )]
		public string Profile { get; set; } = "";

	}

}
