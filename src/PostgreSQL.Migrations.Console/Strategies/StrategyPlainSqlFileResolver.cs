using Database.Migrations;

namespace Migrations.Console.Strategies {

	public static class StrategyPlainSqlFileResolver {

		public static Task<IEnumerable<IMigrationsAsyncResolver>> Run ( IEnumerable<string> files, string group, IEnumerable<string> parameters ) {
			var resolver = new PlainSqlFileResolver ();
			resolver.SetGroup ( group );
			resolver.SetParameters ( parameters );

			return Task.FromResult ( new List<IMigrationsAsyncResolver> { resolver }.AsEnumerable () );
		}

	}

}
