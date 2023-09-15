# PostgreSQL.Migrations

To check the sources, visit the [github repository](https://github.com/EmptyFlow/PostgreSQL.Migrations).

## Goal of project

The goal of the project is to simplify the creation and management of database migrations.
Make it possible to easily integrate migrations into any project or process, be it target audience, tests, application, installer, etc.
Flexible extension of any part of the migration process.
ORM tooling independence makes it easy to change ORMs or use multiple ORMs at the same time in one project.

## What does the process look like?

* Create a migration (whatever migration creation strategy you choose)
* Apply migration to database (local or common developer)
* Run test (optional but highly recomended), need make three operations in sequence -> Apply, Revert, Apply
* Create a deployment unit (installer, script, etc.) and add a new migration to it.
* Apply migration to production database

## 

    mkdocs.yml    # The configuration file.
    docs/
        index.md  # The documentation homepage.
        ...       # Other markdown pages, images and other files.
