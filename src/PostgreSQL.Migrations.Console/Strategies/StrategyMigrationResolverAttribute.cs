using PostgreSQL.Migrations.Client;
using PostgreSQL.Migrations.Runner;
using System.Reflection;
using System.Runtime.Loader;

namespace PostgreSQL.Migrations.Console.Strategies {

    public static class StrategyMigrationResolverAttribute {

        public static Task<IEnumerable<IMigrationsAsyncResolver>> Run ( IEnumerable<string> files, string group ) {
            var assemblies = new List<Assembly> ();
            foreach ( string file in files ) {
                var loadedAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath ( file );
                assemblies.Add ( loadedAssembly );
            }

            var resolver = new MigrationNumberAttributeResolver ();
            resolver.AddAssemblies ( assemblies );
            resolver.SetGroup ( group );

            return Task.FromResult ( new List<IMigrationsAsyncResolver> { resolver }.AsEnumerable() );
        }

    }

}
