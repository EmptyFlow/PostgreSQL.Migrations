# Console Application

To download binary build please check the [github repository](https://github.com/EmptyFlow/PostgreSQL.Migrations/releases).

## Commands
 
* `apply [options]` - Apply all new migrations to database(s)
* `revert [options]` - Revert database to state before migration specified in parameter
* `force-revert [options]` - Revert only one migration specified in parameter
* `revert-all [options]` - Revert database to state before all migrations
* `add-migration [options]` - Add new migration file(s)
* `apply-profile [options]` - Apply all new migrations to database(s). Most options readed from profile file.
* `revert-profile [options]` - Revert database to state before migration specified in parameter. Most options are read from the profile file.
* `force-revert-profile [options]` - Revert only one migration specified in parameter. Most options are read from the profile file.
* `revert-all-profile [options]` - Revert database to state before all migrations. Most options are read from the profile file.
* `add-migration-profile [options]` - Add new migration file(s). Most options are read from the profile file.
* `version` - Display version information

### apply
For check available options use command `apply --help`.    

* `-f=[file] [file] or --files=[file] [file]` - [required] List of files containing migrations.
* `-c=[string] [string] or --connectionStrings=[string] [string]` - [required] List of connection strings to which migrations will be applied.
* `-s=[string] or --strategy=[string]` - [default = CSharpClasses] Select strategy for read migrations.
* `-g=[string] or --group=[string]` - If you specify some group or groups (separated by commas), migrations will be filtered by these groups
* `-t=[string] or --tablename=[string]` - You can change the name of the table in which the migrations will be stored.

### revert
For check available options use command `revert --help`.    

* `-m=[number] or --migration=[number]` - [required] The parameter specifies the number of the migration to which you want to roll back the changes
* `-f=[file] [file] or --files=[file] [file]` - [required] List of files containing migrations.
* `-c=[string] [string] or --connectionStrings [string] [string]` - [required] List of connection strings to which migrations will be applied
* `-s=[string] or --strategy [string]` - [default = CSharpClasses] Select strategy for read migrations.
* `-g=[string] or --group=[string]` - If you specify some group or groups (separated by commas), migrations will be filtered by these groups
* `-t=[string] or --tablename=[string]` - You can change the name of the table in which the migrations will be stored.

### force-revert
For check available options use command `force-revert --help`.    

* `-m=[number] or --migration [number]` - [required] The parameter specifies the number of the migration which will be reverted (if it was applied before) and after it applied once again
* `-f=[file] [file] or --files=[file] [file]` - [required] List of files containing migrations.
* `-c=[string] [string] or --connectionStrings=[string] [string]` - [required] List of connection strings to which migrations will be applied.
* `-s=[string] or --strategy=[string]` - [default = CSharpClasses] Select strategy for read migrations.
* `-g=[string] or --group=[string]` - If you specify some group or groups (separated by commas), migrations will be filtered by these groups
* `-t=[string] or --tablename=[string]` - You can change the name of the table in which the migrations will be stored.

### add-migration
For check available options use command `force-revert --help`.    

* `-m=[number] or --migrationnumber=[number]` - [required] Migration number for the new migration file(s)
* `-p=[file] or --parameters=[string]=[string] [string]=[string]` - [required] List of parameters.
* `-s=[string] or --strategy=[string]` - [default = CSharpClasses] Select strategy for generate migrations.
* `-g=[string] or --group=[string]` - Adding group to new migration.
* `-i=[string] or --issue=[string]` - Adding issue to new migration.
* `-d=[string] or --description=[string]` - You can specify description for new migration.

### revert-all
For check available options use command `force-revert --help`.    

* `-f=[file] [file] or --files=[file] [file]` - [required] List of files containing migrations.
* `-c=[string] [string] or --connectionStrings=[string] [string]` - [required] List of connection strings to which migrations will be applied.
* `-s=[string] or --strategy=[string]` - [default = CSharpClasses] Select strategy for read migrations.
* `-g=[string] or --group=[string]` - If you specify some group or groups (separated by commas), migrations will be filtered by these groups
* `-t=[string] or --tablename=[string]` - You can change the name of the table in which the migrations will be stored.

### apply-profile
For check available options use command `apply-profile --help`.    

* `-p=[file] or --profile=[file]` - Path to file contains profile (check `Profile file` section below).

### revert-profile
For check available options use command `revert-profile --help`.    

* `-m=[number] or --migration=[number]` - [required] The parameter specifies the number of the migration to which you want to roll back the changes
* `-p=[file] or --profile=[file]` - Path to file contains profile (check `Profile file` section below).

### force-revert-profile
For check available options use command `force-revert-profile --help`.    

* `-m=[number] or --migration=[number]` - [required] The parameter specifies the number of the migration which will be reverted (if it was applied before) and after it applied once again
* `-p=[file] or --profile=[file]` - Path to file contains profile (check `Profile file` section below).

### add-migration-profile
For check available options use command `add-migration-profile --help`.    

* `-m=[number] or --migrationnumber=[number]` - [required] Migration number for the new migration file(s)
* `-p=[file] or --profile=[file]` - Path to file contains profile (check `Profile file` section below).
* `-g=[string] or --group=[string]` - Adding group to new migration.
* `-i=[string] or --issue=[string]` - Adding issue to new migration.
* `-d=[string] or --description=[string]` - You can specify description for new migration.

### revert-all-profile
For check available options use command `revert-all-profile --help`.    

* `-p=[file] or --profile=[file]` - Path to file contains profile (check `Profile file` section below).

## Strategies

### CSharpClasses
Migrations are organized into C# classes.
Each class inherits from the `MigrationScript` class from the `PostgreSQL.Migrations` assembly and decorated `MigrationNumber` attribute.
You must implement the `Up` and `Down` methods, where `Up` returns the SQL script that will be executed during the `Apply operation`, and `Down` returns the SQL script that will be executed during the `Revert operation`.
Optional you can fill fields `Issue` (to bound the issue from bugtracker) and `Group` (to bound migration with group or groups).
```csharp
[MigrationNumber ( 1, "http://issue/1", "firstGroup" )]
public class InitialMigration : MigrationScript {

    public override string Down () => "DROP TABLE test;";

    public override string Up () => "CREATE TABLE test(id int4);";

}
```

## Profile file
A profile file is a simple text file containing a few lines. A profile file is an alternative way to populate some parameters such as connection strings, files, strategy, and group for operations `apply`, `revert`, `force-revert` etc.
If profile file not is not specified then will be try to read `migrationprofile` in current directory.
Format in profile file as follow:
```
[name of parameter] [value of parameter]
```
### Parameters
* `constring` - [miltiple] for fill connection string.
* `file` - [miltiple] for fill connection string.
* `strategy` - for fill `strategy` parameter.
* `group` - for fill `group` parameter.
* `genparameter` - for fill parameters for `add-migration` operation.
* `migrationtable` - for fill `tablename` parameter.
### Example
```
constring Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=firstdatabase
constring Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=seconddatabase
file ~/project1/Migrations1.dll
file ~/project2/Migrations2.dll
strategy CSharpClasses
group ProductionMigrations
```

