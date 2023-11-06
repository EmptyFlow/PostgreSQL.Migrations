namespace Database.Migrations {

    /// <summary>
    /// An interface to implement a asynchronize method for getting available migrations.
    /// </summary>
    public interface IMigrationsAsyncResolver {


        /// <summary>
        /// Get all available migrations.
        /// </summary>
        Task<IEnumerable<AvailableMigration>> GetMigrationsAsync ();

        /// <summary>
        /// Generate new migration.
        /// </summary>
        /// <param name="parameters">Parameters required for generation new migration.</param>
        /// <param name="migrationNumber">Migration number.</param>
		/// <param name="groups">Groups for migration.</param>
		/// <param name="issue">Link to issue related with migration.</param>
		/// <param name="description">Description.</param>
        Task GenerateNewMigrationAsync ( List<string> parameters, int migrationNumber, string issue, string groups, string description );

    }

}
