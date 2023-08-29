using CommandLine;

namespace PostgreSQL.Migrations.Console.Options {

    [Verb ( "force-revert", HelpText = "Revert only one migration specified in parameter." )]
    public class ForceRevertOptions {

        [Option ( 'f', "files", Required = true, HelpText = "List of files containing migrations." )]
        public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string> ();

        [Option ( 'c', "connectionStrings", Required = true, HelpText = "List of connection strings to which migrations will be applied." )]
        public IEnumerable<string> ConnectionStrings { get; set; } = Enumerable.Empty<string> ();

        [Option ( 's', "strategy", Default = "MigrationResolverAttribute", HelpText = "Select strategy for read migrations." )]
        public string Strategy { get; set; } = "";

        [Option ( 'g', "group", HelpText = "If you specify some group or groups (separated by commas), migrations will be filtered by these groups." )]
        public string Group { get; set; } = "";

        [Option ( 'm', "migration", Required = true, HelpText = "The parameter specifies the number of the migration which will be reverted (if it was applied before) and after it applied once again." )]
        public int Migration { get; set; } = 0;

    }

}
