# Library API

## How to install library?

### Net CLI
```
dotnet add package PostgreSQL.Migrations
```
### Package Manager
```
Install-Package PostgreSQL.Migrations
```
### NuGet UI
Just search package by `PostgreSQL.Migrations` name

## MigrationRunner
Entry point for working with migration is class `MigrationRunner`.
To work, it needs few things `MigrationResolver`, `ConnectionString` and `SqlRunner`.
Simple example how it can works:
```csharp
var resolver = ...; // initialize resolver

var runner = new MigrationRunner (); // create instance of runner

await runner.LoadMigrationsAsync ( resolver ); // load migrations using the specified resolver

runner.ConnectionString ( "Host=localhost;Database=postgres" ); // configure the connection string to the database where you want to migrate.

var sqlRunner = new PostgresSqlRunner(); // create instance of default sql runner
runner.ApplyMigrationsAsync(sqlRunner); // Apply all new migrations to database(s).

return runner;
```

You can check all the operations supported by `MigrationRunner` in the `MigrationRunner Operations` section.

### MigrationResolver

This class is responsible for retrieving all the necessary migration information from the source and creating a list of classes of type `AvailableMigration` based on this information.
Also it responsible for generating new migration file or files.
It have two methods `GetMigrationsAsync` and `GenerateNewMigrationAsync`.
#### GetMigrationsAsync
Result of performing this method must be list of all available migrations in project.

#### `GenerateNewMigrationAsync(parameters)`.
The result of this method will be to create migrations in the source code. This could be a file or files or another form.
`parameters` depend on the specific migration resolver and can be in any form, please see the documentation for the required MigrationResolver for more information.

### SqlRunner
This class is responsible for low-level interaction with the database like creating transactions and perform SQL queries.

## MigrationRunner Operations

* `ApplyMigrationsAsync` - Apply all new migrations to database(s).
* `RevertMigrationAsync` - Revert database to state before migration specified in parameter.
* `RevertAllMigrationAsync` - Return the database to a state before performing any migrations.
* `ForceMigrationAsync` - Revert only one migration specified in parameter. This is useful during development migrations because you can make `revert`+`apply` only one migration and don't touch other migrations, but in production it is not recommended to perform this operation as it may compromise the integrity of the database.
