using Npgsql;
using PostgreSQL.Migrations.SqlRunner;

namespace PostgreSQL.Migrations.Runner {


    public class MigrationRunner {

        private readonly List<AvailableMigration> m_availableMigrations = new ();

        private readonly List<string> m_connectionStrings = new ();

        public void LoadMigrations ( IMigrationsResolver resolver ) {
            if ( m_availableMigrations.Any () ) m_availableMigrations.Clear ();

            m_availableMigrations.AddRange ( resolver.GetMigrations () );
        }

        public void LoadMigrations ( IEnumerable<IMigrationsResolver> resolvers ) {
            if ( m_availableMigrations.Any () ) m_availableMigrations.Clear ();

            foreach ( var resolver in resolvers ) m_availableMigrations.AddRange ( resolver.GetMigrations () );
        }

        public async Task LoadMigrationsAsync ( IMigrationsAsyncResolver resolver ) {
            if ( m_availableMigrations.Any () ) m_availableMigrations.Clear ();

            m_availableMigrations.AddRange ( await resolver.GetMigrationsAsync () );
        }

        public async Task LoadMigrationsAsync ( IEnumerable<IMigrationsAsyncResolver> resolvers ) {
            if ( m_availableMigrations.Any () ) m_availableMigrations.Clear ();

            foreach ( var resolver in resolvers ) m_availableMigrations.AddRange ( await resolver.GetMigrationsAsync () );
        }

        public void ConnectionString ( string connectionString ) {
            m_connectionStrings.Clear ();
            m_connectionStrings.Add ( connectionString );
        }

        public void ConnectionString ( NpgsqlConnectionStringBuilder builder ) {
            m_connectionStrings.Clear ();
            m_connectionStrings.Add ( builder.ToString () );
        }

        public void ConnectionString ( IEnumerable<string> connectionStrings ) {
            m_connectionStrings.Clear ();
            m_connectionStrings.AddRange ( connectionStrings );
        }

        public void ConnectionString ( IEnumerable<NpgsqlConnectionStringBuilder> builders ) {
            m_connectionStrings.Clear ();
            m_connectionStrings.AddRange ( builders.Select ( a => a.ToString () ) );
        }

        public async Task ApplyMigrationsAsync ( ISqlRunner sqlRunner ) {
            if ( !m_availableMigrations.Any () ) return;

            var migrations = m_availableMigrations
                .OrderBy ( a => a.MigrationNumber )
                .ToList ();

            foreach ( var connectionString in m_connectionStrings ) {
                await sqlRunner.BeginTransactionAsync ( connectionString );

                var appliedMigrations = await sqlRunner.GetAppliedMigrations ( connectionString );

                var notAppliedMigrations = migrations
                    .Where ( a => appliedMigrations.Contains ( a.MigrationNumber ) )
                    .ToArray ();

                foreach ( var notAppliedMigration in notAppliedMigrations ) {
                    await sqlRunner.ApplyMigrationAsync ( connectionString, notAppliedMigration );
                }

                await sqlRunner.CommitTransactionAsync ( connectionString );
            }
        }

        public async Task ForceMigrationAsync ( ISqlRunner sqlRunner, int migration ) {
            foreach ( var connectionString in m_connectionStrings ) {
                await sqlRunner.BeginTransactionAsync ( connectionString );

                var appliedMigrations = await sqlRunner.GetAppliedMigrations ( connectionString );
                var fullMigration = GetFullMigration ( migration );
                if ( appliedMigrations.Contains(migration) )  await sqlRunner.RevertMigrationAsync ( connectionString, fullMigration );

                await sqlRunner.ApplyMigrationAsync ( connectionString, fullMigration );

                await sqlRunner.CommitTransactionAsync ( connectionString );
            }
        }

        private AvailableMigration GetFullMigration ( int migration ) {
            var fullMigration = m_availableMigrations.FirstOrDefault ( a => a.MigrationNumber == migration );
            return fullMigration ?? throw new Exception ( $"Migrations with number {migration} was applied but don't contains apropriate item in available migrations!" );
        }

        public async Task RevertMigrationAsync ( ISqlRunner sqlRunner, int migration ) {
            foreach ( var connectionString in m_connectionStrings ) {
                await sqlRunner.BeginTransactionAsync ( connectionString );

                var appliedMigrations = await sqlRunner.GetAppliedMigrations ( connectionString );
                var migrations = m_availableMigrations.ToDictionary ( a => a.MigrationNumber );

                foreach ( var appliedMigration in appliedMigrations.OrderByDescending ( a => a ) ) {
                    if ( appliedMigration < migration ) break;

                    if ( migrations.TryGetValue ( appliedMigration, out var fullAppliedMigration ) ) {
                        await sqlRunner.RevertMigrationAsync ( connectionString, migrations[appliedMigration] );
                    } else {
                        throw new Exception ( $"Migrations with number {appliedMigration} was applied but don't contains apropriate item in available migrations!" );
                    }
                }

                await sqlRunner.CommitTransactionAsync ( connectionString );
            }
        }

        public async Task RevertAllMigrationsAsync ( ISqlRunner sqlRunner ) {
            foreach ( var connectionString in m_connectionStrings ) {
                await sqlRunner.BeginTransactionAsync ( connectionString );

                var appliedMigrations = await sqlRunner.GetAppliedMigrations ( connectionString );
                var migrations = m_availableMigrations.ToDictionary ( a => a.MigrationNumber );

                foreach ( var appliedMigration in appliedMigrations.OrderByDescending ( a => a ) ) {
                    if ( migrations.TryGetValue( appliedMigration, out var fullAppliedMigration ) ) {
                        await sqlRunner.RevertMigrationAsync ( connectionString, migrations[appliedMigration] );
                    } else {
                        throw new Exception ( $"Migrations with number {appliedMigration} was applied but don't contains in available migrations!" );
                    }
                }

                await sqlRunner.CommitTransactionAsync ( connectionString );
            }
        }

    }

}
