namespace TestMigrationAssembly {

	public class InitialMigration {

		public int MigrationNumber => 1;

		public string Issue => "http://issue/1";

		public string Group => "firstGroup";

		public string Down () => "initial migration down script";

		public string Up () => "initial migration up script";

	}

	public class SecondMigration {

		public static int MigrationNumber => 2;

		public static string Issue => "http://issue/2";

		public static string Group => "lastGroup";


		public string Down () => "second migration down script";

		public string Up () => "second migration up script";

	}

}
