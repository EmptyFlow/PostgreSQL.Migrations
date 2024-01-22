using Database.Migrations;

namespace Migrations.Console.Strategies {


	public static class StrategyPackedFileResolver {

		public static Task<IEnumerable<IMigrationsAsyncResolver>> Run ( IEnumerable<string> files, string group ) {
			var resolver = new PackFileResolver ();
			resolver.SetFiles ( files );
			resolver.SetGroup ( group );

			return Task.FromResult ( new List<IMigrationsAsyncResolver> { resolver }.AsEnumerable () );
		}

	}

}
