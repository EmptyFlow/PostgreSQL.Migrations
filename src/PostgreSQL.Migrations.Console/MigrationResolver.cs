using PostgreSQL.Migrations.Client;
using PostgreSQL.Migrations.Console.Strategies;
using PostgreSQL.Migrations.Runner;
using SystemConsole = System.Console;

namespace PostgreSQL.Migrations.Console {

    public static class MigrationResolver {

        private const string MigrationResolverAttributeStrategy = "MigrationResolverAttribute";

        public static IMigrationsAsyncResolver GetResolver ( string strategy ) {
            return strategy switch {
                MigrationResolverAttributeStrategy => new MigrationNumberAttributeResolver (),
                _ => throw new NotSupportedException ( $"Strategy {strategy} not supported!" )
            };
        }

        public static async Task<List<IMigrationsAsyncResolver>> GetResolvers ( IEnumerable<string> files, string group, string strategy ) {
            SystemConsole.WriteLine ( $"Trying to use a strategy: {strategy}..." );
            var migrationResolvers = new List<IMigrationsAsyncResolver> ();

            switch ( strategy ) {
                case MigrationResolverAttributeStrategy:
                    migrationResolvers.AddRange ( await StrategyMigrationResolverAttribute.Run ( files, group ) );
                    break;
                default:
                    break;
            }

            SystemConsole.WriteLine ( $"Strategy {strategy} applied!" );

            return migrationResolvers;
        }

    }
}
