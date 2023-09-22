using Npgsql;
using PostgreSQL.Migrations.SqlRunner;

namespace PostgreSQL.Migrations.Runner {


    public sealed class MigrationRunner {

        private readonly List<AvailableMigration> m_availableMigrations = new ();

        private readonly List<string> m_connectionStrings = new ();

        private readonly ConsoleMigrationRunnerLogger m_consoleLogger = new ();

        private readonly WeakReference<IMigrationRunnerLogger>? m_logger;

        public MigrationRunner ( IMigrationRunnerLogger? logger = default ) {
            m_logger = new WeakReference<IMigrationRunnerLogger> ( logger != default ? logger : m_consoleLogger );
        }

        private void Log ( string message ) {
            if ( m_logger!.TryGetTarget ( out var logger ) ) logger.Log ( message );
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

            Log ( "Operation: Apply migrations" );

            var migrations = m_availableMigrations
                .OrderBy ( a => a.MigrationNumber )
                .ToList ();

            Log ( $"Founded migrations: {migrations.Count}" );

            foreach ( var connectionString in m_connectionStrings ) {
                Log ( $"Applied for connection string: {connectionString}" );

                await sqlRunner.BeginTransactionAsync ( connectionString );

                var appliedMigrations = await sqlRunner.GetAppliedMigrations ( connectionString );

                var notAppliedMigrations = migrations
                    .Where ( a => !appliedMigrations.Contains ( a.MigrationNumber ) )
                    .ToArray ();

                foreach ( var notAppliedMigration in notAppliedMigrations ) {
                    Log ( $"Started applying migration with number {notAppliedMigration.MigrationNumber}" );
                    await sqlRunner.ApplyMigrationAsync ( connectionString, notAppliedMigration );
                    Log ( $"Finished applying migration with number {notAppliedMigration.MigrationNumber}" );
                }

                await sqlRunner.CommitTransactionAsync ( connectionString );
            }
        }

        public async Task ForceMigrationAsync ( ISqlRunner sqlRunner, int migration ) {
            Log ( "Operation: Force migration" );

            foreach ( var connectionString in m_connectionStrings ) {
                Log ( $"Applied for connection string: {connectionString}" );

                await sqlRunner.BeginTransactionAsync ( connectionString );

                var appliedMigrations = await sqlRunner.GetAppliedMigrations ( connectionString );
                var fullMigration = GetFullMigration ( migration );

                if ( appliedMigrations.Contains ( migration ) ) {
                    Log ( $"Started reverting migration with number {fullMigration.MigrationNumber}" );
                    await sqlRunner.RevertMigrationAsync ( connectionString, fullMigration );
                    Log ( $"Finished reverting migration with number {fullMigration.MigrationNumber}" );
                }

                Log ( $"Started applying migration with number {fullMigration.MigrationNumber}" );
                await sqlRunner.ApplyMigrationAsync ( connectionString, fullMigration );
                Log ( $"Finished applying migration with number {fullMigration.MigrationNumber}" );

                await sqlRunner.CommitTransactionAsync ( connectionString );
            }
        }

        private AvailableMigration GetFullMigration ( int migration ) {
            var fullMigration = m_availableMigrations.FirstOrDefault ( a => a.MigrationNumber == migration );
            return fullMigration ?? throw new Exception ( $"Migrations with number {migration} was applied but don't contains apropriate item in available migrations!" );
        }

        public async Task RevertMigrationAsync ( ISqlRunner sqlRunner, int migration ) {
            Log ( "Operation: Revert migrations" );

            foreach ( var connectionString in m_connectionStrings ) {
                Log ( $"Applied for connection string: {connectionString}" );

                await sqlRunner.BeginTransactionAsync ( connectionString );

                var appliedMigrations = await sqlRunner.GetAppliedMigrations ( connectionString );
                var migrations = m_availableMigrations.ToDictionary ( a => a.MigrationNumber );

                foreach ( var appliedMigration in appliedMigrations.OrderByDescending ( a => a ) ) {
                    if ( appliedMigration < migration ) break;

                    if ( migrations.TryGetValue ( appliedMigration, out var fullAppliedMigration ) ) {
                        Log ( $"Started reverting migration with number {fullAppliedMigration.MigrationNumber}" );
                        await sqlRunner.RevertMigrationAsync ( connectionString, migrations[appliedMigration] );
                        Log ( $"Finished reverting migration with number {fullAppliedMigration.MigrationNumber}" );
                    } else {
                        Log ( $"Migrations with number {appliedMigration} was applied but don't contains apropriate item in available migrations!" );
                    }
                }

                await sqlRunner.CommitTransactionAsync ( connectionString );
            }
        }

        public async Task RevertAllMigrationsAsync ( ISqlRunner sqlRunner ) {
            Log ( "Operation: Revert all migrations" );

            foreach ( var connectionString in m_connectionStrings ) {
                await sqlRunner.BeginTransactionAsync ( connectionString );

                var appliedMigrations = await sqlRunner.GetAppliedMigrations ( connectionString );
                var migrations = m_availableMigrations.ToDictionary ( a => a.MigrationNumber );

                foreach ( var appliedMigration in appliedMigrations.OrderByDescending ( a => a ) ) {
                    if ( migrations.TryGetValue ( appliedMigration, out var fullAppliedMigration ) ) {
                        Log ( $"Started reverting migration with number {fullAppliedMigration.MigrationNumber}" );
                        await sqlRunner.RevertMigrationAsync ( connectionString, migrations[appliedMigration] );
                        Log ( $"Finished reverting migration with number {fullAppliedMigration.MigrationNumber}" );
                    } else {
                        Log ( $"Migrations with number {appliedMigration} was applied but don't contains in available migrations!" );
                    }
                }

                await sqlRunner.CommitTransactionAsync ( connectionString );
            }
        }

    }

}
