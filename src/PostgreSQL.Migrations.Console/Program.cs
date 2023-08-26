using CommandLine;
using PostgreSQL.Migrations.Console;
using PostgreSQL.Migrations.Console.Options;
using PostgreSQL.Migrations.Runner;
using PostgreSQL.Migrations.SqlRunner;

Dependencies.RegisterMigrations ();

var parser = Parser.Default.ParseArguments<ApplyOptions, RevertOptions> ( args );

await parser.WithParsedAsync<ApplyOptions> ( ApplyMigrationsToDatabase );
await parser.WithParsedAsync<RevertOptions> ( RevertMigrationsToDatabase );
await parser.WithNotParsedAsync ( HandleParseError );

static async Task<int> ApplyMigrationsToDatabase ( ApplyOptions options ) {
    var migrationResolvers = await MigrationResolver.GetResolvers ( options.Files, options.Group, options.Strategy );

    var runner = await GetRunner ( options.ConnectionStrings, migrationResolvers );

    Console.WriteLine ( $"Starting operation Apply..." );
    await runner.ApplyMigrationsAsync ( Dependencies.GetService<ISqlRunner> () );
    Console.WriteLine ( $"Operation Apply is completed!" );

    return 0;
}

static async Task<int> RevertMigrationsToDatabase ( RevertOptions options ) {
    var migrationResolvers = await MigrationResolver.GetResolvers ( options.Files, options.Group, options.Strategy );
    var runner = await GetRunner ( options.ConnectionStrings, migrationResolvers );

    Console.WriteLine ( $"Starting operation Revert..." );
    await runner.RevertMigrationAsync ( Dependencies.GetService<ISqlRunner> (), options.Migration );
    Console.WriteLine ( $"Operation Revert is completed!" );

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

static async Task<MigrationRunner> GetRunner ( IEnumerable<string> connectionStrings, List<IMigrationsAsyncResolver> migrationResolvers ) {
    var runner = new MigrationRunner ();
    await runner.LoadMigrationsAsync ( migrationResolvers );
    runner.ConnectionString ( connectionStrings );
    return runner;
}