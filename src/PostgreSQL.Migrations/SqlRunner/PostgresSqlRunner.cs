using Npgsql;
using PostgreSQL.Migrations.Runner;

namespace PostgreSQL.Migrations.SqlRunner {

    public class PostgresSqlRunner : ISqlRunner {

        private readonly Dictionary<string, (NpgsqlConnection, NpgsqlTransaction)> m_connections = new ();

        private const string MigrationTable = "postgresmigrations";

        private static async Task CreateMigrationTableIfRequired ( NpgsqlConnection connection, NpgsqlTransaction transaction ) {
            await using var cmd = new NpgsqlCommand (
                $"CREATE TABLE IF NOT EXISTS {MigrationTable}(timestamp integer NOT NULL PRIMARY KEY, description text, issue text, created timestamp NOT NULL DEFAULT now())",
                connection,
                transaction
            );

            await cmd.ExecuteNonQueryAsync ();
        }

        public async Task BeginTransactionAsync ( string connectionString ) {
            if ( string.IsNullOrEmpty ( connectionString ) ) throw new ArgumentNullException ( nameof ( connectionString ) );
            if ( m_connections.ContainsKey ( connectionString ) ) throw new ArgumentException ( $"Transaction for connection string: {connectionString} already created! If you want to start it again need commit current." );

            var connection = new NpgsqlConnection ( connectionString );

            await connection.OpenAsync ();
            var transaction = await connection.BeginTransactionAsync ();

            await CreateMigrationTableIfRequired ( connection, transaction );

            m_connections[connectionString] = (connection, transaction);
        }

        public async Task CommitTransactionAsync ( string connectionString ) {
            if ( string.IsNullOrEmpty ( connectionString ) ) throw new ArgumentNullException ( nameof ( connectionString ) );
            if ( !m_connections.ContainsKey ( connectionString ) ) throw new ArgumentException ( $"Transaction for connection string: {connectionString} not was created!" );

            var (connection, transaction) = m_connections[connectionString];
            await transaction.CommitAsync ();
            await connection.CloseAsync ();
            m_connections.Remove ( connectionString );
        }

        private static async Task CreateMigrationRecord ( int migrationId, string description, string issue, NpgsqlConnection connection, NpgsqlTransaction transaction ) {
            await using var cmd = new NpgsqlCommand ( $"INSERT INTO {MigrationTable} (timestamp, description, issue) VALUES (@_param1, @_param2, @_param3)", connection, transaction );

            cmd.Parameters.AddWithValue ( "@_param1", migrationId );
            cmd.Parameters.AddWithValue ( "@_param2", description );
            cmd.Parameters.AddWithValue ( "@_param3", issue );

            await cmd.ExecuteNonQueryAsync ();
        }

        private static async Task DeleteMigrationRecord ( int migrationId, NpgsqlConnection connection, NpgsqlTransaction transaction ) {
            await using var cmd = new NpgsqlCommand ( $"DELETE FROM {MigrationTable} WHERE timestamp = @_param1", connection, transaction );

            cmd.Parameters.AddWithValue ( "@_param1", migrationId );

            await cmd.ExecuteNonQueryAsync ();
        }

        public async Task ApplyMigrationAsync ( string connectionString, AvailableMigration migration ) {
            if ( string.IsNullOrEmpty ( connectionString ) ) throw new ArgumentNullException ( nameof ( connectionString ) );
            if ( !m_connections.ContainsKey ( connectionString ) ) throw new ArgumentException ( $"Transaction for connection string: {connectionString} not was created! Use BeginTransactionAsync method to create it." );

            var (connection, transaction) = m_connections[connectionString];

            try {
                await using var cmd = new NpgsqlCommand ( migration.UpScript, connection, transaction );
                await cmd.ExecuteNonQueryAsync ();
            } catch ( Exception ex ) {
                throw new Exception ( $"Error while run UP migration with number {migration.MigrationNumber}!", ex );
            }

            await CreateMigrationRecord ( migration.MigrationNumber, migration.Description, migration.Issue, connection, transaction );
        }

        public async Task RevertMigrationAsync ( string connectionString, AvailableMigration migration ) {
            if ( string.IsNullOrEmpty ( connectionString ) ) throw new ArgumentNullException ( nameof ( connectionString ) );
            if ( !m_connections.ContainsKey ( connectionString ) ) throw new ArgumentException ( $"Transaction for connection string: {connectionString} not was created! Use BeginTransactionAsync method to create it." );

            var (connection, transaction) = m_connections[connectionString];

            try {
                await using var cmd = new NpgsqlCommand ( migration.DownScript, connection, transaction );
                await cmd.ExecuteNonQueryAsync ();
            } catch ( Exception ex ) {
                throw new Exception ( $"Error while run DOWN migration with number {migration.MigrationNumber}!", ex );
            }

            await DeleteMigrationRecord ( migration.MigrationNumber, connection, transaction );
        }

        public async Task<IEnumerable<int>> GetAppliedMigrations ( string connectionString ) {
            if ( !m_connections.ContainsKey ( connectionString ) ) throw new ArgumentException ( $"Transaction for connection string: {connectionString} not was created! Use BeginTransactionAsync method to create it." );

            var (connection, transaction) = m_connections[connectionString];

            await using var cmd = new NpgsqlCommand ( $"SELECT timestamp FROM {MigrationTable} ORDER BY timestamp", connection, transaction );

            using var reader = await cmd.ExecuteReaderAsync ();
            var result = new List<int> ();
            while ( await reader.ReadAsync () ) {
                result.Add ( reader.GetInt32 ( 0 ) );
            }

            return result;
        }

    }
}
