using Npgsql;
using Database.Migrations;
using PostgreSQL.Migrations.SqlRunner;
using Xunit;

[assembly: CollectionBehavior ( DisableTestParallelization = true )]

namespace PostgreSQL.Migrations.UnitTests.IntegrationTests {

    [TestCaseOrderer( "PostgreSQL.Migrations.UnitTests.IntegrationTests.PriorityOrderer", "PostgreSQL.Migrations.UnitTests" )]
    public class PostgresSqlRunnerIntegrationTests {

        private const string m_ConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=migrationtests";

        private static async Task CheckRawRequest ( string query, Action<NpgsqlDataReader> readRow ) {
            var connection = new NpgsqlConnection ( m_ConnectionString );

            await connection.OpenAsync ();

            await using var cmd = new NpgsqlCommand ( query, connection );

            using var reader = await cmd.ExecuteReaderAsync ();
            var result = new List<int> ();
            while ( await reader.ReadAsync () ) readRow.Invoke ( reader );

            await reader.CloseAsync ();

            await connection.CloseAsync ();
        }

        private static async Task<bool> CheckRawRequestException ( string query, Action<Exception> exceptionCallback ) {
            var connection = new NpgsqlConnection ( m_ConnectionString );

            await connection.OpenAsync ();

            await using var cmd = new NpgsqlCommand ( query, connection );

            var hasException = false;

            try {
                using var reader = await cmd.ExecuteReaderAsync ();
                await reader.CloseAsync ();
            } catch ( Exception ex ) {
                hasException = true;
                exceptionCallback ( ex );
            }

            await connection.CloseAsync ();

            return hasException;
        }

        [Fact, Trait ( "Category", "Integration" ), TestPriority (1)]
        public async Task ApplyMigrationAsync_Complete () {
            //arrange
            var runner = new PostgresSqlRunner ();

            //act
            await runner.BeginTransactionAsync ( m_ConnectionString );
            await runner.ApplyMigrationAsync (
                m_ConnectionString,
                new AvailableMigration {
                    Description = "description",
                    MigrationNumber = 1,
                    UpScript = "CREATE TABLE test (field bool);",
                    DownScript = ""
                }
            );

            await runner.CommitTransactionAsync ( m_ConnectionString );

            //assert
            await CheckRawRequest (
                "SELECT timestamp FROM postgresmigrations ORDER BY timestamp",
                ( reader ) => {
                    var timestamp = reader.GetInt32 ( 0 );
                    Assert.Equal ( 1, timestamp );
                }
            );
            await CheckRawRequest (
                "SELECT COUNT(*) FROM test",
                ( reader ) => {
                    var timestamp = reader.GetInt32 ( 0 );
                    Assert.Equal ( 0, timestamp );
                }
            );
        }

        [Fact, Trait ( "Category", "Integration" ), TestPriority ( 2 )]
        public async Task GetAppliedMigrations_Complete () {
            //arrange
            var runner = new PostgresSqlRunner ();

            //act
            await runner.BeginTransactionAsync ( m_ConnectionString );
            var migrations = await runner.GetAppliedMigrations ( m_ConnectionString );
            await runner.CommitTransactionAsync ( m_ConnectionString );

            //assert
            Assert.Equal ( 1, migrations.First () );
        }

        [Fact, Trait ( "Category", "Integration" ), TestPriority ( 3 )]
        public async Task RevertMigrationAsync_Complete () {
            //arrange
            var runner = new PostgresSqlRunner ();

            //act
            await runner.BeginTransactionAsync ( m_ConnectionString );
            await runner.RevertMigrationAsync (
                m_ConnectionString,
                new AvailableMigration {
                    Description = "description",
                    MigrationNumber = 1,
                    UpScript = "",
                    DownScript = "DROP TABLE test"
                }
            );
            await runner.CommitTransactionAsync ( m_ConnectionString );

            //assert
            await CheckRawRequest (
                "SELECT timestamp FROM postgresmigrations WHERE timestamp = 1",
                ( reader ) => {
                    //if it have result it mean is have error
                    Assert.True ( false );
                }
            );
            var tableNotExists = await CheckRawRequestException ("SELECT * FROM test", (_) => { } );
            Assert.True( tableNotExists );
        }


    }

}
