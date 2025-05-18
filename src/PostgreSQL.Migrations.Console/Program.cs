using FlowCommandLine;
using Migrations.Console.Options;
using PostgreSQL.Migrations.Console;
using PostgreSQL.Migrations.Console.Options;

Dependencies.RegisterDependencies ();

var databaseAdjustments = new List<FlowCommandParameter> {
	FlowCommandParameter.Create("f", "files", "List of files containing migrations."),
	FlowCommandParameter.Create("p", "parameters", "List of parameters."),
	FlowCommandParameter.CreateRequired("c", "connectionStrings", "List of connection strings to which migrations will be applied."),
	FlowCommandParameter.Create("s", "strategy", "Select strategy for read migrations."),
	FlowCommandParameter.Create("g", "group", "If you specify some group or groups (separated by commas), migrations will be filtered by these groups."),
	FlowCommandParameter.Create("t", "tablename", "You can change the name of the table in which the migrations will be stored.", "MigrationTable"),
};
var revertOptionsDatabaseAdjustments = databaseAdjustments.Concat (
	new List<FlowCommandParameter> {
		FlowCommandParameter.CreateRequired ( "m", "migration", "The parameter specifies the number of the migration to which you want to roll back the changes." )
	}
);
var forceRevertDatabaseAdjustments = databaseAdjustments.Concat (
	new List<FlowCommandParameter> {
		FlowCommandParameter.CreateRequired ( "m", "migration", "The parameter specifies the number of the migration which will be reverted (if it was applied before) and after it applied once again." )
	}
);
var revertProfileDatabaseAdjustments = databaseAdjustments.Concat (
	new List<FlowCommandParameter> {
		FlowCommandParameter.CreateRequired ( "m", "migration", "The parameter specifies the number of the migration to which you want to roll back the changes." )
	}
);

var profileAdjustments = new List<FlowCommandParameter> {
	FlowCommandParameter.Create("p", "profile", "This is an optional parameter where you can specify the path to the profile. If the parameter is not specified, an attempt will be made to find the profile in the current directory.")
};

var addMigrationAdjustments = new List<FlowCommandParameter> {
	FlowCommandParameter.CreateRequired("m", "migrationnumber", "Migration number for the new migration file(s)."),
	FlowCommandParameter.CreateRequired("p", "parameters", "List of parameters."),
	FlowCommandParameter.Create("s", "strategy", "Select strategy for adding migration."),
	FlowCommandParameter.Create("g", "group", "You can specify group(s) for new migration."),
	FlowCommandParameter.Create("i", "issue", "You can specify issue for new migration."),
	FlowCommandParameter.Create("d", "description", "You can specify description for new migration."),
};

var packMigrationAdjustments = new List<FlowCommandParameter> {
	FlowCommandParameter.CreateRequired("f", "files", "List of files containing migrations."),
	FlowCommandParameter.Create("s", "strategy", "Select strategy for adding migration."),
	FlowCommandParameter.Create("g", "group", "You can specify group(s) for new migration."),
	FlowCommandParameter.Create("r", "result", "Path to result file."),
};

await CommandLine.Console ()
	.Application (
		"Flow Migrations",
		$"{typeof ( ApplyOptions ).Assembly.GetName ().Version}",
		"Tool for performing database migrations.\nYou can download latest version from https://github.com/EmptyFlow/PostgreSQL.Migrations/releases",
		"Copyright (c) Roman Vladimirov",
		"fmigrations"
	)
	.AddAsyncCommand<ApplyOptions> (
		"apply",
		DatabaseOperations.ApplyMigrationsToDatabase,
		"Apply all new migrations to database(s).",
		databaseAdjustments
	)
	.AddAsyncCommand<ApplyProfileOptions> (
		"apply-profile",
		DatabaseOperations.ApplyMigrationProfileToDatabase,
		"Read options from profile and apply all new migrations to database(s).",
		profileAdjustments
	)
	.AddAsyncCommand<RevertOptions> (
		"revert",
		DatabaseOperations.RevertMigrationsToDatabase,
		"Revert database to state before migration specified in parameter.",
		revertOptionsDatabaseAdjustments
	)
	.AddAsyncCommand<RevertProfileOptions> (
		"revert-profile",
		DatabaseOperations.RevertMigrationProfileToDatabase,
		"Read options from profile and revert database to state before migration specified in parameter.",
		revertProfileDatabaseAdjustments
	)
	.AddAsyncCommand<ForceRevertOptions> (
		"force-revert",
		DatabaseOperations.ForceRevertMigrationInDatabase,
		"Revert only one migration specified in parameter.",
		forceRevertDatabaseAdjustments
	)
	.AddAsyncCommand<ForceRevertProfileOptions> (
		"force-revert-profile",
		DatabaseOperations.ForceRevertMigrationProfileToDatabase,
		"Revert database to state before all migrations.",
		forceRevertDatabaseAdjustments
	)
	.AddAsyncCommand<RevertAllOptions> (
		"revert-all",
		DatabaseOperations.RevertAllMigrations,
		"Revert database to state before all migrations.",
		databaseAdjustments
	)
	.AddAsyncCommand<RevertAllProfileOptions> (
		"revert-all-profile",
		DatabaseOperations.RevertAllMigrationsProfile,
		"Revert database to state before all migrations.",
		profileAdjustments
	)
	.AddAsyncCommand<AddMigrationOptions> (
		"add-migration",
		AddMigrationOperations.AddMigration,
		"Add new migration file(s).",
		addMigrationAdjustments
	)
	.AddAsyncCommand<AddMigrationProfileOptions> (
		"add-migration-profile",
		AddMigrationOperations.AddMigrationProfile,
		"Add new migration file(s) based on profile.",
		new List<FlowCommandParameter> {
			FlowCommandParameter.CreateRequired("m", "migrationnumber", "Migration number for the new migration file(s)."),
			FlowCommandParameter.Create("s", "strategy", "Select strategy for adding migration."),
			FlowCommandParameter.Create("g", "group", "You can specify group(s) for new migration."),
			FlowCommandParameter.Create("i", "issue", "You can specify issue for new migration."),
			FlowCommandParameter.Create("d", "description", "You can specify description for new migration."),
			FlowCommandParameter.Create("p", "profile", "This is an optional parameter where you can specify the path to the profile. If the parameter is not specified, an attempt will be made to find the profile in the current directory.")
		}
	)
	.AddAsyncCommand<PackMigrationsOptions> (
		"pack",
		DatabaseOperations.PackMigrations,
		"Read migrations and create single file containing all migrations.",
		packMigrationAdjustments
	)
	.AddAsyncCommand<PackMigrationsProfileOptions> (
		"pack-profile",
		DatabaseOperations.PackMigrationsProfile,
		"Read migrations and create single file containing all migrations based on profile.",
		profileAdjustments
	)
	.AddCommand (
		"version",
		( VersionOptions options ) => {
			Console.WriteLine ( typeof ( ApplyOptions ).Assembly.GetName ().Version );
		},
		"Display version of application",
		new List<FlowCommandParameter> ()
	)
	.RunCommandAsync ();