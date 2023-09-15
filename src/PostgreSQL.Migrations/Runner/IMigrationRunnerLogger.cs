namespace PostgreSQL.Migrations.Runner {

    /// <summary>
    /// Interface for logging operations and any other messages during the migration process.
    /// </summary>
    public interface IMigrationRunnerLogger {

        /// <summary>
        /// Write message to log.
        /// </summary>
        /// <param name="message">Message.</param>
        void Log ( string message );

    }

}
