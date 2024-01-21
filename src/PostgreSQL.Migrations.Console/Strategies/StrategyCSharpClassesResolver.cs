using Database.Migrations;

namespace PostgreSQL.Migrations.Console.Strategies {

	public static class StrategyCSharpClassesResolver {

		public static Task<IEnumerable<IMigrationsAsyncResolver>> Run ( IEnumerable<string> files, string group ) {
			var resolver = new MigrationNumberAttributeResolver ();
			resolver.AddFiles ( files );
			resolver.SetGroup ( group );

			return Task.FromResult ( new List<IMigrationsAsyncResolver> { resolver }.AsEnumerable () );
		}

	}

}
