# PostgreSQL.Migrations

To check the sources, visit the [github repository](https://github.com/EmptyFlow/PostgreSQL.Migrations).

## Goal of project

The goal of the project is to simplify the creation and management of database migrations.
Make it possible to easily integrate migrations into any project or process, be it target audience, tests, application, installer, etc.
Flexible extension of any part of the migration process.
ORM tooling independence makes it easy to change ORMs or use multiple ORMs at the same time in one project.

## Terminology

* `Apply operation` the operation of applying schema (or data) changes to a database
* `Revert operation` the operation of reverting a previously applied change to the schema (or data) from the database
* `Migration` the structure has a number (order in order) and two scripts - up and down
* `Up script` the script is performed when the Apply operation is launched
* `Down script` the script is performed when the Revert operation is launched

## What does the process look like?

* Create a migration (whatever migration creation strategy you choose)
* Apply migration to database (local or common developer)
* Run test (optional but highly recomended), need make three operations in sequence -> Apply, Revert, Apply
* Create a deployment unit (installer, script, etc.) and add a new migration to it.
* Apply migration to production database

## Migration Strategy
`Migration Strategy` controls the organization of migration in your project.
It can be classes like in Entity Framework or simple pair of SQL files with names like up.sql and down.sql.  
`PostgreSQL.Migrations` already has several `Migration Strategy` implemented, or you can implement your own for your project.
