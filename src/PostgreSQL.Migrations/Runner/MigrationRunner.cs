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

    }

}
