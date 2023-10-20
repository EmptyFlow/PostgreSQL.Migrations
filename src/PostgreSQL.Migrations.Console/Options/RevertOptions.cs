using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

	[Verb ( "revert", HelpText = "Revert database to state before migration specified in parameter." )]
	public class RevertOptions : DatabaseAdjustments {

		[Option ( 'm', "migration", Required = true, HelpText = "The parameter specifies the number of the migration to which you want to roll back the changes." )]
		public int Migration { get; set; } = 0;

	}

}
