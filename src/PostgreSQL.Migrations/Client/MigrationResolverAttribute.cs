using PostgreSQL.Migrations.Runner;
using System.ComponentModel;
using System.Reflection;

namespace PostgreSQL.Migrations.Client {

    /// <summary>
    /// Migration resolve based on decorating classes with <see cref="MigrationNumberAttribute"/> attribute and <see cref="DescriptionAttribute"/> for specify field issue.
    /// </summary>
    public class MigrationResolverAttribute : IMigrationsAsyncResolver {

        private const string AttributeName = "MigrationNumberAttribute";

        private readonly List<Assembly> m_assemblies = new ();

        private string m_group = "";

        private Type? m_attributeType;

        private const string m_failedMessage = nameof ( MigrationResolverAttribute ) + ": Failed get migrations";

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
                    .Where ( IsHasMigrationNumberAttribute )
                    .ToList ();

                foreach ( Type type in types ) {
                    var (number, issue, group) = GetMigrationDataFromType ( type );
                    var description = type.GetCustomAttribute<DescriptionAttribute> ()?.Description ?? "";

                    var script = Activator.CreateInstance ( type );
                    if ( script == null ) continue;

                    if ( !CheckInGroup ( group ) ) continue;

                    result.Add (
                        new AvailableMigration {
                            MigrationNumber = number,
                            Issue = issue,
                            Description = description,
                            Group = group,
                            UpScript = InvokeMigrationMethod ( "Up", script ),
                            DownScript = InvokeMigrationMethod ( "Down", script ),
                        }
                    );
                }
            }

            return Task.FromResult ( result.AsEnumerable () );
        }

        private static bool IsHasMigrationNumberAttribute ( Type type ) => type.GetCustomAttributes ().Any ( b => b.GetType ().Name == AttributeName );

        private (int number, string issue, string group) GetMigrationDataFromType ( Type type ) {
            var attribute = type.GetCustomAttributes ().First ( b => b.GetType ().Name == AttributeName );

            var migrationNumber = GetValueFromProperty<int> ( "MigrationNumber", attribute );
            var issue = GetValueFromProperty<string> ( "Issue", attribute );
            var group = GetValueFromProperty<string> ( "Group", attribute );

            return (migrationNumber, issue, group);
        }

        private static string InvokeMigrationMethod ( string methodName, object instance ) {
            var instanceType = instance.GetType ();

            var method = instanceType.GetMethod ( methodName, BindingFlags.Instance | BindingFlags.Public );
            if ( method == null ) {
                Console.WriteLine ( $"Can't read method '{methodName}' from migration class {instanceType.FullName}!" );
                throw new Exception ( m_failedMessage );
            }

            var result = method.Invoke ( instance, null );
            if ( result == null ) {
                Console.WriteLine ( $"The method '{methodName}' from the migration class {instanceType.FullName} returns null, which is not compatible!" );
                throw new Exception ( m_failedMessage );
            }
            return (string) result;
        }

        private T GetValueFromProperty<T> ( string propertyName, Attribute attribute ) {
            if ( m_attributeType == null ) m_attributeType = attribute.GetType ();

            var property = m_attributeType.GetProperty ( propertyName, BindingFlags.Instance | BindingFlags.Public );
            if ( property == null ) {
                Console.WriteLine ( $"Can't read property '{propertyName}' from migration attribute {m_attributeType.FullName}!" );
                throw new Exception ( m_failedMessage );
            }

            var result = property.GetGetMethod ()!.Invoke ( attribute, null );
            if ( result == null ) {
                Console.WriteLine ( $"Value from property '{propertyName}' in class {m_attributeType.FullName} is null that is not compatible!" );
                throw new Exception ( m_failedMessage );
            }

            return (T) result;
        }

        private bool CheckInGroup ( string group ) {
            if ( string.IsNullOrEmpty ( m_group ) ) return true;

            var groups = group.Split ( "," ).Select ( a => a.Trim ().ToLowerInvariant () );
            var searchGroups = m_group.Split ( "," ).Select ( a => a.Trim ().ToLowerInvariant () );
            return groups.Intersect ( searchGroups ).Any ();
        }

    }

}
