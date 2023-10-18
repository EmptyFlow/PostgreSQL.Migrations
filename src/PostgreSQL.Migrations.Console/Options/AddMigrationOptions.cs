using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

    [Verb ( "add-migration", HelpText = "Add new migration file(s)." )]
    public record AddMigrationOptions : AddMigrationAdjustments {

    }

}
