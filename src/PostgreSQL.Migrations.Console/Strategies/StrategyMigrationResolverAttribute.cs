using PostgreSQL.Migrations.Client;
using PostgreSQL.Migrations.Runner;
using System.Reflection;
using System.Runtime.Loader;

namespace PostgreSQL.Migrations.Console.Strategies {

    public class StrategyMigrationResolverAttribute {

        public Task<IEnumerable<AvailableMigration>> Run ( IEnumerable<string> files, string group ) {
            var assemblies = new List<Assembly> ();
            foreach ( string file in files ) {
                var loadedAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath ( file );
                assemblies.Add ( loadedAssembly );
            }

            var resolver = new MigrationResolverAttribute ();
            resolver.AddAssemblies ( assemblies );

            return Task.FromResult ( resolver.GetMigrations() );
        }

    }

}
