
namespace PostgreSQL.Migrations.Console.Options {

	//[Verb ( "add-migration-profile", HelpText = "Add new migration file(s) based on profile." )]
	public record AddMigrationProfileOptions : AddMigrationProfileAdjustments {

		//[Option ( 'm', "migrationnumber", Required = true, HelpText = "Migration number for the new migration file(s)." )]
		public int MigrationNumber { get; init; } = 0;

		//[Option ( 'g', "group", HelpText = "You can specify group(s) for new migration." )]
		public string Group { get; set; } = "";

		//[Option ( 'i', "issue", HelpText = "You can specify issue for new migration." )]
		public string Issue { get; set; } = "";

		//[Option ( 'd', "description", HelpText = "You can specify description for new migration." )]
		public string Description { get; set; } = "";

	}

}
