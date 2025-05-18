
namespace PostgreSQL.Migrations.Console.Options {

	public record AddMigrationProfileOptions : AddMigrationProfileAdjustments {

		public int MigrationNumber { get; init; } = 0;

		public string Group { get; set; } = "";

		public string Issue { get; set; } = "";

		public string Description { get; set; } = "";

	}

}
