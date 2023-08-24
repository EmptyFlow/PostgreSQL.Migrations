using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

    [Verb ( "revert", HelpText = "Revert database to state before migration specified in parameter with same name." )]
    public class RevertOptions {

        [Option ( 'f', "files", Required = true, HelpText = "List of files containing migrations." )]
        public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string> ();

        [Option ( 'c', "connectionStrings", Required = true, HelpText = "List of connection strings to which migrations will be applied." )]
        public IEnumerable<string> ConnectionStrings { get; set; } = Enumerable.Empty<string> ();

        [Option ( 's', "strategy", Default = "MigrationResolverAttribute", HelpText = "Select strategy for read migrations." )]
        public string Strategy { get; set; } = "";

        [Option ( 'g', "group", HelpText = "If you specify some group or groups (separated by commas), migrations will be filtered by these groups." )]
        public string Group { get; set; } = "";

        [Option ( 'm', "migration", Required = true, HelpText = "The parameter specifies the number of the migration to which you want to roll back the changes." )]
        public int Migration { get; set; } = 0;

    }

}
