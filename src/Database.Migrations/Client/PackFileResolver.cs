using Database.Migrations.Client;
using Database.Migrations.JsonSerializers;
using System.Text.Json;

namespace Database.Migrations {

	public class PackFileResolver : IMigrationsAsyncResolver {

		private string m_group = "";

		private readonly List<string> m_fileNames = new ();

		public void SetGroup ( string group ) => m_group = group;

		public void SetFiles ( IEnumerable<string> files ) => m_fileNames.AddRange ( files );

		public Task GenerateNewMigrationAsync ( List<string> parameters, int migrationNumber, string issue, string groups, string description ) {
			Console.WriteLine ( "Packed migration strategy not support generation operation!" );
			throw new NotImplementedException ();
		}

		public async Task<IEnumerable<AvailableMigration>> GetMigrationsAsync () {
			var result = new List<AvailableMigration> ();

			foreach ( string configFile in m_fileNames ) {
				var content = await File.ReadAllTextAsync ( configFile );
				var migrations = JsonSerializer.Deserialize ( content, typeof ( List<AvailableMigration> ), PackSerializer.Default ) as List<AvailableMigration>;
				if ( migrations == null ) continue;

				result.AddRange ( migrations.Where ( a => ResolverHelpers.CheckInGroup ( a.Group, m_group ) ) );
			}

			return result;
		}

	}

}
