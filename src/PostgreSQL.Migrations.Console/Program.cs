using CommandLine;
using PostgreSQL.Migrations.Console;
using PostgreSQL.Migrations.Console.Options;
using PostgreSQL.Migrations.Console.Strategies;
using PostgreSQL.Migrations.Runner;
using PostgreSQL.Migrations.SqlRunner;

Dependencies.RegisterMigrations();

await Parser.Default.ParseArguments<ApplyOptions> ( args )
    .MapResult (
      ApplyMigrationsToDatabase,
      HandleParseError
    );

static async Task<int> ApplyMigrationsToDatabase ( ApplyOptions options ) {
    var migrationResolvers = new List<IMigrationsAsyncResolver> ();

    switch ( options.Strategy ) {
        case "MigrationResolverAttribute":
            migrationResolvers.AddRange ( await StrategyMigrationResolverAttribute.Run ( options.Files, options.Group ) );
            break;
        default:
            break;
    }

    var runner = new MigrationRunner ();
    await runner.LoadMigrationsAsync ( migrationResolvers );
    runner.ConnectionString ( options.ConnectionStrings );
    await runner.ApplyMigrationsAsync ( Dependencies.GetService<ISqlRunner> () );

    return 0;
}

static Task<int> HandleParseError ( IEnumerable<Error> errors ) {
    if ( errors.IsVersion () ) {
        Console.WriteLine ( $"Current version is {typeof ( ApplyOptions ).Assembly.GetName ().Version}\nDownload latest version from https://github.com/EmptyFlow/PostgreSQL.Migrations/releases" );
        return Task.FromResult ( 0 );
    }

    if ( errors.IsHelp () ) return Task.FromResult ( 0 );

    return Task.FromResult ( 1 );
}