using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

	[Verb ( "revert-all", HelpText = "Revert database to state before all migrations." )]
	public class RevertAllOptions : DatabaseAdjustments {

	}

}
