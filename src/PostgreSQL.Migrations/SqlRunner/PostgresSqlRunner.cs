using Npgsql;
using PostgreSQL.Migrations.Runner;

namespace PostgreSQL.Migrations.SqlRunner {

    public class PostgresSqlRunner : ISqlRunner {

        private readonly Dictionary<string, (NpgsqlConnection, NpgsqlTransaction)> m_connections = new ();

        public async Task BeginTransactionAsync ( string connectionString ) {
            if ( string.IsNullOrEmpty ( connectionString ) ) throw new ArgumentNullException ( nameof ( connectionString ) );
            if ( m_connections.ContainsKey ( connectionString ) ) throw new ArgumentException ( $"Transaction for connection string: {connectionString} already created! If you want to start it again need commit current." );

            var connection = new NpgsqlConnection ( connectionString );

            await connection.OpenAsync ();
            var transaction = await connection.BeginTransactionAsync ();
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

        private static async Task CreateMigrationRecord ( int migrationId, string description, NpgsqlConnection connection, NpgsqlTransaction transaction ) {
            await using var cmd = new NpgsqlCommand ( "INSERT INTO migrations (timestamp, description, issue) VALUES (@_param1, @_param2, @_param3)", connection, transaction );

            cmd.Parameters.AddWithValue ( "@_param1", migrationId );
            cmd.Parameters.AddWithValue ( "@_param2", description );

            await cmd.ExecuteNonQueryAsync ();
        }

        private static async Task DeleteMigrationRecord ( int migrationId, NpgsqlConnection connection, NpgsqlTransaction transaction ) {
            await using var cmd = new NpgsqlCommand ( "DELETE FROM migrations WHERE timestamp = @_param1", connection, transaction );

            cmd.Parameters.AddWithValue ( "@_param1", migrationId );

            await cmd.ExecuteNonQueryAsync ();
        }

        public async Task ApplyMigrationAsync ( string connectionString, AvailableMigration migration ) {
            if ( string.IsNullOrEmpty ( connectionString ) ) throw new ArgumentNullException ( nameof ( connectionString ) );
            if ( !m_connections.ContainsKey ( connectionString ) ) throw new ArgumentException ( $"Transaction for connection string: {connectionString} not was created! Use BeginTransactionAsync method to create it." );

            var (connection, transaction) = m_connections[connectionString];

            await using var cmd = new NpgsqlCommand ( migration.UpScript, connection, transaction );
            await cmd.ExecuteNonQueryAsync ();

            await CreateMigrationRecord ( migration.MigrationNumber, migration.Description, connection, transaction );
        }

        public async Task RevertMigration ( string connectionString, AvailableMigration migration ) {
            if ( string.IsNullOrEmpty ( connectionString ) ) throw new ArgumentNullException ( nameof ( connectionString ) );
            if ( !m_connections.ContainsKey ( connectionString ) ) throw new ArgumentException ( $"Transaction for connection string: {connectionString} not was created! Use BeginTransactionAsync method to create it." );

            var (connection, transaction) = m_connections[connectionString];

            await using var cmd = new NpgsqlCommand ( migration.DownScript, connection, transaction );
            await cmd.ExecuteNonQueryAsync ();

            await DeleteMigrationRecord ( migration.MigrationNumber, connection, transaction );
        }

        public async Task<IEnumerable<int>> GetAppliedMigrations ( string connectionString ) {
            if ( !m_connections.ContainsKey ( connectionString ) ) throw new ArgumentException ( $"Transaction for connection string: {connectionString} not was created! Use BeginTransactionAsync method to create it." );

            var (connection, transaction) = m_connections[connectionString];

            await using var cmd = new NpgsqlCommand ( "SELECT timestamp FROM migrations ORDER BY timestamp", connection, transaction );

            using var reader = await cmd.ExecuteReaderAsync ();
            var result = new List<int> ();
            while ( await reader.ReadAsync () ) {
                result.Add ( reader.GetInt32 ( 0 ) );
            }

            return result;
        }

    }
}
