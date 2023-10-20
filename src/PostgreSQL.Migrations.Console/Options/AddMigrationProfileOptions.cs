using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

	[Verb ( "add-migration-profile", HelpText = "Add new migration file(s) based on profile." )]
	public record AddMigrationProfileOptions : AddMigrationProfileAdjustments {

		[Option ( 'm', "migrationnumber", Required = true, HelpText = "Migration number for the new migration file(s)." )]
		public int MigrationNumber { get; init; } = 0;

	}

}
