using Database.Migrations;

namespace Migrations.Console.Strategies {

	public static class StarategyPlainSqlFileResolver {

		public static Task<IEnumerable<IMigrationsAsyncResolver>> Run ( IEnumerable<string> files, string group ) {
			var resolver = new PlainSqlFileResolver ();
			resolver.SetConfigFiles ( files );
			resolver.SetGroup ( group );

			return Task.FromResult ( new List<IMigrationsAsyncResolver> { resolver }.AsEnumerable () );
		}

	}

}
