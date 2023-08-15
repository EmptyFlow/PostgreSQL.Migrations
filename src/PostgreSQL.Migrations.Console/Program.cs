using CommandLine;
using PostgreSQL.Migrations.Console.Options;
using PostgreSQL.Migrations.Console.Strategies;
using PostgreSQL.Migrations.Runner;

await Parser.Default.ParseArguments<ApplyOptions> ( args )
    .MapResult (
      ApplyMigrationsToDatabase,
      HandleParseError
    );

static async Task<int> ApplyMigrationsToDatabase ( ApplyOptions options ) {
    var migrations = new List<AvailableMigration> ();

    switch ( options.Strategy ) {
        case "MigrationResolverAttribute":
            var strategies = new StrategyMigrationResolverAttribute ();
            migrations.AddRange ( await strategies.Run ( options.Files, options.Group ) );
            break;
        default:
            break;
    }
    return 0;
}

static Task<int> HandleParseError ( IEnumerable<Error> errs ) {
    if ( errs.IsVersion () ) {
        Console.WriteLine ( $"Current version is {typeof ( ApplyOptions ).Assembly.GetName ().Version}\nDownload latest version from https://github.com/EmptyFlow/PostgreSQL.Migrations/releases" );
        return Task.FromResult ( 0 );
    }

    if ( errs.IsHelp () ) {
        Console.WriteLine ( "Help Request" );
        return Task.FromResult ( 0 );
    }

    return Task.FromResult ( 1 );
}