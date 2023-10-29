namespace Database.Migrations.Client {

	public static class ResolverHelpers {

		public static bool CheckInGroup ( string source, string pattern ) {
			if ( string.IsNullOrEmpty ( pattern ) ) return true;

			var groups = source.Split ( "," ).Select ( a => a.Trim ().ToLowerInvariant () );
			var searchGroups = pattern.Split ( "," ).Select ( a => a.Trim ().ToLowerInvariant () );
			return groups.Intersect ( searchGroups ).Any ();
		}

	}

}
