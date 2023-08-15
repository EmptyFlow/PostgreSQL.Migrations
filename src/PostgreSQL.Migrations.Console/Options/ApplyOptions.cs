using CommandLine;

namespace PostgreSQL.Migrations.Console.Options
{

    [Verb("apply", HelpText = "Apply all new migrations to database(s).")]
    public class ApplyOptions
    {

        [Option('f', "files", Required = true, HelpText = "List of files containing migrations.")]
        public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string>();

        [Option('s', "strategy", Default = "MigrationResolverAttribute", HelpText = "Select strategy for read migrations.")]
        public string Strategy { get; set; } = "";

        [Option('g', "group", HelpText = "If you specify some group or groups (separated by commas), migrations will be filtered by these groups")]
        public string Group { get; set; } = "";

    }

}
