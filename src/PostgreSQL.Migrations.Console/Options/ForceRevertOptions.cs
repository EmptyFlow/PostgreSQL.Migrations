using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

	[Verb ( "force-revert", HelpText = "Revert only one migration specified in parameter." )]
	public class ForceRevertOptions : DatabaseAdjustments {

		[Option ( 'm', "migration", Required = true, HelpText = "The parameter specifies the number of the migration which will be reverted (if it was applied before) and after it applied once again." )]
		public int Migration { get; set; } = 0;

	}

}
