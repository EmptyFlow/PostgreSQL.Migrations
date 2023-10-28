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
        Task GenerateNewMigrationAsync ( List<string> parameters, int migrationNumber );

    }

}
