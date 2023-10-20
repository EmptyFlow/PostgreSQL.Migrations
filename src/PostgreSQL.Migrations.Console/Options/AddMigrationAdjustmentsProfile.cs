using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

	public record AddMigrationProfileAdjustments {

		[Option ( 'p', "profile", HelpText = "This is an optional parameter where you can specify the path to the profile. If the parameter is not specified, an attempt will be made to find the profile in the current directory." )]
		public string Profile { get; set; } = "";

	}

}
