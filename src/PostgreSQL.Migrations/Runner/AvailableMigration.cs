namespace PostgreSQL.Migrations.Runner {

    /// <summary>
    /// Available migration.
    /// </summary>
    public class AvailableMigration {

        /// <summary>
        /// Sequential migration number.
        /// </summary>
        public int MigrationNumber { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Fields.
        /// </summary>
        public Dictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Script for apply migration.
        /// </summary>
        public string UpScript { get; set; } = "";

        /// <summary>
        /// Script for revert migration.
        /// </summary>
        public string DownScript { get; set; } = "";

    }

}