namespace PostgreSQL.Migrations.Client {

    /// <summary>
    /// Base class for classes containing Up and Down SQL scripts.
    /// </summary>
    public abstract class MigrationScript {

        /// <summary>
        /// Script to perform the "apply" part of the migration.
        /// </summary>
        /// <returns>Result SQL script.</returns>
        public abstract string Up ();

        /// <summary>
        /// Script to perform the "revert" part of the migration.
        /// </summary>
        /// <returns>Result SQL script.</returns>
        public abstract string Down ();

    }

}
