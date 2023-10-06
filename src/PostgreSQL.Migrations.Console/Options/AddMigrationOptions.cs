using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

    public record AddMigrationOptions {

        [Option ( 'f', "folder", Required = true, HelpText = "The folder in which the new migration file will be created." )]
        public string Folder { get; init; } = "";

        [Option ( 'o', "options", Required = true, HelpText = "List of options." )]
        public IEnumerable<string> Options { get; init; } = Enumerable.Empty<string> ();

    }

}
