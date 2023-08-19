using FakeItEasy;
using Npgsql;
using PostgreSQL.Migrations.Runner;
using PostgreSQL.Migrations.SqlRunner;
using Xunit;

namespace PostgreSQL.Migrations.UnitTests.IntegrationTests {

    [TestCaseOrderer ( "PostgreSQL.Migrations.UnitTests.IntegrationTests.PriorityOrderer", "PostgreSQL.Migrations.UnitTests" )]
    public class MigrationRunnerIntegrationTests {

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

        [Fact, Trait ( "Category", "Integration" ), TestPriority ( 1 )]
        public async Task ApplyMigrationsAsync_Complete_InitialRun () {
            //arrange
            var migrationsResolver = A.Fake<IMigrationsAsyncResolver> ();
            A.CallTo ( () => migrationsResolver.GetMigrationsAsync () ).Returns (
                new List<AvailableMigration> {
                    new AvailableMigration {
                        MigrationNumber = 1,
                        UpScript = "CREATE TABLE test1(id integer NOT NULL);"
                    },
                    new AvailableMigration {
                        MigrationNumber = 2,
                        UpScript = "CREATE TABLE test2(id integer NOT NULL);"
                    }
                }
            );
            var runner = new MigrationRunner ();
            runner.ConnectionString ( m_ConnectionString );
            await runner.LoadMigrationsAsync ( migrationsResolver );

            //act
            await runner.ApplyMigrationsAsync ( new PostgresSqlRunner () );

            //assert
            await CheckRawRequest (
                "SELECT COUNT(*) FROM postgresmigrations",
                ( reader ) => {
                    var count = reader.GetInt32 ( 0 );
                    Assert.Equal ( 2, count );
                }
            );
            await CheckRawRequest (
                "SELECT COUNT(*) FROM test1",
                ( reader ) => {
                    var countInTest = reader.GetInt32 ( 0 );
                    Assert.Equal ( 0, countInTest );
                }
            );
            await CheckRawRequest (
                "SELECT COUNT(*) FROM test2",
                ( reader ) => {
                    var countInTest = reader.GetInt32 ( 0 );
                    Assert.Equal ( 0, countInTest );
                }
            );
        }

        [Fact, Trait ( "Category", "Integration" ), TestPriority ( 2 )]
        public async Task ApplyMigrationsAsync_Complete_AdditionalMigration () {
            //arrange
            var migrationsResolver = A.Fake<IMigrationsAsyncResolver> ();
            A.CallTo ( () => migrationsResolver.GetMigrationsAsync () ).Returns ( new List<AvailableMigration> {
                new AvailableMigration {
                    MigrationNumber = 3,
                    UpScript = "CREATE TABLE test3(id integer NOT NULL);"
                }
            } );
            var runner = new MigrationRunner ();
            runner.ConnectionString ( m_ConnectionString );
            await runner.LoadMigrationsAsync ( migrationsResolver );

            //act
            await runner.ApplyMigrationsAsync ( new PostgresSqlRunner () );

            //assert
            await CheckRawRequest (
                "SELECT COUNT(*) FROM postgresmigrations",
                ( reader ) => {
                    var count = reader.GetInt32 ( 0 );
                    Assert.Equal ( 3, count );
                }
            );
            await CheckRawRequest (
                "SELECT COUNT(*) FROM test3",
                ( reader ) => {
                    var timestamp = reader.GetInt32 ( 0 );
                    Assert.Equal ( 0, timestamp );
                }
            );
        }

        [Fact, Trait ( "Category", "Integration" ), TestPriority ( 3 )]
        public async Task MigrationRunner_RevertMigrationAsync_Complete () {
            //arrange
            var migrationsResolver = A.Fake<IMigrationsAsyncResolver> ();
            A.CallTo ( () => migrationsResolver.GetMigrationsAsync () ).Returns ( new List<AvailableMigration> {
                new AvailableMigration {
                    MigrationNumber = 1,
                    DownScript = "DROP TABLE test1;"
                },
                new AvailableMigration {
                    MigrationNumber = 2,
                    DownScript = "DROP TABLE test2;"
                },
                new AvailableMigration {
                    MigrationNumber = 3,
                    DownScript = "DROP TABLE test3;"
                }
            } );
            var runner = new MigrationRunner ();
            runner.ConnectionString ( m_ConnectionString );
            await runner.LoadMigrationsAsync ( migrationsResolver );

            //act
            await runner.RevertMigrationAsync ( new PostgresSqlRunner (), 2 );

            //assert
            await CheckRawRequest (
                "SELECT COUNT(*) FROM postgresmigrations",
                ( reader ) => {
                    var count = reader.GetInt32 ( 0 );
                    Assert.Equal ( 1, count );
                }
            );
            await CheckRawRequest (
                "SELECT COUNT(*) FROM test1",
                ( reader ) => {
                    var timestamp = reader.GetInt32 ( 0 );
                    Assert.Equal ( 0, timestamp );
                }
            );
            var tableNotExists1 = await CheckRawRequestException ( "SELECT * FROM test2", ( _ ) => { } );
            Assert.True ( tableNotExists1 );

            var tableNotExists2 = await CheckRawRequestException ( "SELECT * FROM test3", ( _ ) => { } );
            Assert.True ( tableNotExists2 );
        }

        [Fact, Trait ( "Category", "Integration" ), TestPriority ( 4 )]
        public async Task MigrationRunner_RevertAllMigrationsAsync_Complete () {
            //arrange
            var migrationsResolver = A.Fake<IMigrationsAsyncResolver> ();
            A.CallTo ( () => migrationsResolver.GetMigrationsAsync () ).Returns ( new List<AvailableMigration> {
                new AvailableMigration {
                    MigrationNumber = 1,
                    DownScript = "DROP TABLE test1;"
                },
                new AvailableMigration {
                    MigrationNumber = 2,
                    DownScript = "DROP TABLE test2;"
                },
                new AvailableMigration {
                    MigrationNumber = 3,
                    DownScript = "DROP TABLE test3;"
                }
            } );
            var runner = new MigrationRunner ();
            runner.ConnectionString ( m_ConnectionString );
            await runner.LoadMigrationsAsync ( migrationsResolver );

            //act
            await runner.RevertAllMigrationsAsync ( new PostgresSqlRunner () );

            //assert
            await CheckRawRequest (
                "SELECT COUNT(*) FROM postgresmigrations",
                ( reader ) => {
                    var count = reader.GetInt32 ( 0 );
                    Assert.Equal ( 0, count );
                }
            );

            var tableNotExists1 = await CheckRawRequestException ( "SELECT * FROM test2", ( _ ) => { } );
            Assert.True ( tableNotExists1 );

            var tableNotExists2 = await CheckRawRequestException ( "SELECT * FROM test3", ( _ ) => { } );
            Assert.True ( tableNotExists2 );

            var tableNotExists3 = await CheckRawRequestException ( "SELECT * FROM test1", ( _ ) => { } );
            Assert.True ( tableNotExists3 );
        }

    }

}
