﻿using CommandLine;
using PostgreSQL.Migrations.Console;
using PostgreSQL.Migrations.Console.Options;

Dependencies.RegisterDependencies ();

var parser = Parser.Default.ParseArguments<
	ApplyOptions, RevertOptions, ForceRevertOptions,
	ApplyProfileOptions, RevertProfileOptions, ForceRevertProfileOptions,
	AddMigrationOptions, AddMigrationProfileOptions> ( args );

await parser.WithParsedAsync<ApplyOptions> ( DatabaseOperations.ApplyMigrationsToDatabase );
await parser.WithParsedAsync<RevertOptions> ( DatabaseOperations.RevertMigrationsToDatabase );
await parser.WithParsedAsync<ForceRevertOptions> ( DatabaseOperations.ForceRevertMigrationInDatabase );
await parser.WithParsedAsync<ApplyProfileOptions> ( DatabaseOperations.ApplyMigrationProfileToDatabase );
await parser.WithParsedAsync<RevertProfileOptions> ( DatabaseOperations.RevertMigrationProfileToDatabase );
await parser.WithParsedAsync<ForceRevertProfileOptions> ( DatabaseOperations.ForceRevertMigrationProfileToDatabase );
await parser.WithParsedAsync<AddMigrationOptions> ( AddMigrationOperations.AddMigration );
await parser.WithParsedAsync<AddMigrationProfileOptions> ( AddMigrationOperations.AddMigrationProfile );
await parser.WithNotParsedAsync ( HandleParseError );

static Task<int> HandleParseError ( IEnumerable<Error> errors ) {
	if ( errors.IsVersion () ) {
		Console.WriteLine ( $"Current version is {typeof ( ApplyOptions ).Assembly.GetName ().Version}\nDownload latest version from https://github.com/EmptyFlow/PostgreSQL.Migrations/releases" );
		return Task.FromResult ( 0 );
	}

	if ( errors.IsHelp () ) return Task.FromResult ( 0 );

	return Task.FromResult ( 1 );
}