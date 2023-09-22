using PostgreSQL.Migrations.Runner;
using System.ComponentModel;
using System.Reflection;

namespace PostgreSQL.Migrations.Client {

    /// <summary>
    /// Migration resolve based on decorating classes with <see cref="MigrationNumberAttribute"/> attribute and <see cref="DescriptionAttribute"/> for specify field issue.
    /// </summary>
    public class MigrationResolverAttribute : IMigrationsAsyncResolver {

        private readonly List<Assembly> m_assemblies = new ();

        private string m_group = "";

        public void AddAssemblies ( IEnumerable<Assembly> assemblies ) => m_assemblies.AddRange ( assemblies );

        public void SetGroup ( string group ) => m_group = group;

        /// <summary>
        /// Get all available migrations from assemblies specified using the AddAssemblies method.
        /// </summary>
        /// <returns>List of available migrations found in the specified assemblies.</returns>
        public Task<IEnumerable<AvailableMigration>> GetMigrationsAsync () {
            var result = new List<AvailableMigration> ();

            foreach ( Assembly assembly in m_assemblies ) {
                var types = assembly.GetTypes ()
                    .Where ( a => a.GetCustomAttribute<MigrationNumberAttribute> () != null )
                    .ToList ();

                foreach ( Type type in types ) {
                    var migrationAttribute = type.GetCustomAttribute<MigrationNumberAttribute> () ?? throw new ArgumentNullException ();
                    var description = type.GetCustomAttribute<DescriptionAttribute> ()?.Description ?? "";

                    var script = Activator.CreateInstance ( type ) as MigrationScript;
                    if ( script == null ) continue;

                    if ( !CheckInGroup ( migrationAttribute.Group ) ) continue;

                    result.Add (
                        new AvailableMigration {
                            MigrationNumber = migrationAttribute.MigrationNumber,
                            Issue = migrationAttribute.Issue,
                            Description = description,
                            Group = migrationAttribute.Group,
                            UpScript = script.Up (),
                            DownScript = script.Down (),
                        }
                    );
                }
            }

            return Task.FromResult ( result.AsEnumerable () );
        }

        private bool CheckInGroup ( string group ) {
            if ( string.IsNullOrEmpty ( m_group ) ) return true;

            var groups = group.Split ( "," ).Select ( a => a.Trim ().ToLowerInvariant () );
            var searchGroups = m_group.Split ( "," ).Select ( a => a.Trim ().ToLowerInvariant () );
            return groups.Intersect ( searchGroups ).Any ();
        }

    }

}
