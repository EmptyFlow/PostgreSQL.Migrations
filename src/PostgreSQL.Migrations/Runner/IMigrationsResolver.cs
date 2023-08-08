namespace PostgreSQL.Migrations.Runner {

    /// <summary>
    /// An interface to implement a method for getting available migrations.
    /// </summary>
    public interface IMigrationsResolver {

        /// <summary>
        /// Get all available migrations.
        /// </summary>
        IEnumerable<AvailableMigration> GetMigrations ();

    }

}
