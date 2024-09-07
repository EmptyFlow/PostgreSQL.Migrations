using System.ComponentModel;
using System.Reflection;
using System.Runtime.Loader;

namespace Database.Migrations {

	/// <summary>
	/// Migration resolve based on decorating classes with <see cref="MigrationNumberAttribute"/> attribute and <see cref="DescriptionAttribute"/> for specify field issue.
	/// </summary>
	public class MigrationNumberAttributeResolver : IMigrationsAsyncResolver {

		private const string RequiredPropertyName = "MigrationNumber";

		private readonly List<string> m_files = new ();

		private string m_group = "";

		private const string m_failedMessage = nameof ( MigrationNumberAttributeResolver ) + ": Failed get migrations";

		public void AddFiles ( IEnumerable<string> files ) => m_files.AddRange ( files );

		public void SetGroup ( string group ) => m_group = group;

		/// <summary>
		/// Get all available migrations from assemblies specified using the AddAssemblies method.
		/// </summary>
		/// <returns>List of available migrations found in the specified assemblies.</returns>
		public Task<IEnumerable<AvailableMigration>> GetMigrationsAsync () {
			var result = new List<AvailableMigration> ();

			foreach ( var file in m_files ) {
				var loadContext = new AssemblyLoadContext ( "MigrationLoadContext" + Path.GetFileName ( file ) );
				var fullPath = Path.GetFullPath ( file );
				Console.WriteLine ( $"Try to load dll {fullPath}" );
				var pathToAssembly = Path.GetDirectoryName ( fullPath ) ?? "";
				loadContext.Resolving += ( AssemblyLoadContext context, AssemblyName assemblyName ) => {
					return context.LoadFromAssemblyPath ( Path.Combine ( pathToAssembly, $"{assemblyName.Name}.dll" ) );
				};
				var assembly = loadContext.LoadFromAssemblyPath ( fullPath );
				var types = assembly.GetTypes ()
					.Where ( IsHasMigrationNumberAttribute )
					.ToList ();

				foreach ( Type type in types ) {
					var migration = Activator.CreateInstance ( type );
					if ( migration == null ) continue;

					var (number, issue, group) = GetMigrationDataFromType ( migration, type );
					var description = type.GetCustomAttribute<DescriptionAttribute> ()?.Description ?? "";

					if ( !CheckInGroup ( group ) ) continue;

					result.Add (
						new AvailableMigration {
							MigrationNumber = number,
							Issue = issue,
							Description = description,
							Group = group,
							UpScript = InvokeMigrationMethod ( "Up", migration ),
							DownScript = InvokeMigrationMethod ( "Down", migration ),
						}
					);
				}
			}

			return Task.FromResult ( result.AsEnumerable () );
		}

		private static bool IsHasMigrationNumberAttribute ( Type type ) => type.GetProperties ().Any ( b => b.Name == RequiredPropertyName );

		private static (int number, string issue, string group) GetMigrationDataFromType ( object script, Type type ) {
			var migrationNumber = GetValueFromProperty<int> ( "MigrationNumber", script, type, required: true );
			var issue = GetValueFromProperty<string?> ( "Issue", script, type ) ?? "";
			var group = GetValueFromProperty<string?> ( "Group", script, type ) ?? "";

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

		private static T? GetValueFromProperty<T> ( string propertyName, object instance, Type type, bool required = false ) {
			var property = type.GetProperty ( propertyName, BindingFlags.Instance | BindingFlags.Public ); // first step instance property
			if ( property == null ) property = type.GetProperty ( propertyName, BindingFlags.Static | BindingFlags.Public ); // second step static property
			if ( property == null && required ) {
				Console.WriteLine ( $"Can't read property '{propertyName}' from migration attribute {type.FullName}!" );
				throw new Exception ( m_failedMessage );
			}
			if ( property == null && !required ) return default;

			var result = property!.GetGetMethod ()!.Invoke ( instance, null );
			if ( result == null ) {
				Console.WriteLine ( $"Value from property '{propertyName}' in class {type.FullName} is null that is not compatible!" );
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

		private static string? GetStringValueFromParameters ( string name, List<string> parameters, bool isRequired, string valueDescription = "" ) {
			var value = parameters.FirstOrDefault ( a => a.StartsWith ( $"{name}=" ) );
			if ( value == null && isRequired ) throw new Exception ( $"Parameter {name} is required! You need specify it as `{name}=<{valueDescription}>`" );

			return value?.Replace ( $"{name}=", "" ) ?? "";
		}

		public async Task GenerateNewMigrationAsync ( List<string> parameters, int migrationNumber, string issue, string groups, string description ) {
			var folder = GetStringValueFromParameters ( "folder", parameters, true, "path to folder where migrations will be generated" );
			var fileNamespace = GetStringValueFromParameters ( "namespace", parameters, false );
			var customClassName = GetStringValueFromParameters ( "classname", parameters, false );

			if ( !Directory.Exists ( folder ) ) throw new Exception ( $"Folder {folder} don't exists!" );

			var assembly = GetType ().Assembly;

			Stream? templateStream;

			if ( string.IsNullOrEmpty ( fileNamespace ) ) {
				templateStream = assembly.GetManifestResourceStream ( "Database.Migrations.Client.Templates.WithoutNamespaceTemplate.template" );
			} else {
				templateStream = assembly.GetManifestResourceStream ( "Database.Migrations.Client.Templates.WithNamespaceTemplate.template" );
			}
			if ( templateStream == null ) return;

			using var streamReader = new StreamReader ( templateStream );
			var template = await streamReader.ReadToEndAsync ();

			var migrationsClassName = customClassName?.Replace ( "{MigrationNumber}", migrationNumber.ToString () ) ?? $"Migration{migrationNumber}";

			template = template
				.Replace ( "{ClassName}", migrationsClassName )
				.Replace ( "{Namespace}", fileNamespace )
				.Replace ( "{MigrationNumber}", migrationNumber.ToString () );
			template = ReplaceOptionalAttribute ( "{Issue}", template, issue );
			template = ReplaceOptionalAttribute ( "{Group}", template, groups );
			template = ReplaceOptionalAttribute ( "{Description}", template, description );

			var filePath = Path.Combine ( folder, migrationsClassName + ".cs" );
			await File.WriteAllTextAsync ( filePath, template );

			Console.WriteLine ( $"Migration file {filePath} created." );
		}

		private static string ReplaceOptionalAttribute ( string pattern, string template, string? value ) {
			if ( !string.IsNullOrEmpty ( value ) ) return template.Replace ( pattern, $",\"{value}\"" );

			return template.Replace ( pattern, "" );
		}

	}

}
