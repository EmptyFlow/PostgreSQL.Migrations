using PostgreSQL.Migrations.Console.Options;
using SystemConsole = System.Console;

namespace PostgreSQL.Migrations.Console {

    public class AddMigrationOperations {

        public static async Task<int> AddMigration ( AddMigrationOptions options ) {
            SystemConsole.WriteLine ( $"Started add migration..." );

            var resolver = MigrationResolver.GetResolver ( options.Strategy );
            if ( resolver == null ) return 1;

            SystemConsole.WriteLine ( $"Trying to use a strategy: {options.Strategy}..." );

            await resolver.GenerateNewMigrationAsync ( options.Parameters.ToList(), options.MigrationNumber );

            SystemConsole.WriteLine ( $"Migration with number {options.MigrationNumber} created." );

            return 0;
        }

    }

}
