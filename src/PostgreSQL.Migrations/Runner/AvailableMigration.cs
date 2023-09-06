namespace PostgreSQL.Migrations.Runner {

    /// <summary>
    /// Available migration.
    /// </summary>
    public record AvailableMigration {

        /// <summary>
        /// Sequential migration number.
        /// </summary>
        public int MigrationNumber { get; init; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; init; } = "";

        /// <summary>
        /// Issue number or link.
        /// </summary>
        public string Issue { get; init; } = "";

        /// <summary>
        /// Migration group(s).
        /// </summary>
        public string Group { get; init; } = "";

        /// <summary>
        /// Fields.
        /// </summary>
        public Dictionary<string, object> Fields { get; init; } = new Dictionary<string, object>();

        /// <summary>
        /// Script for apply migration.
        /// </summary>
        public string UpScript { get; init; } = "";

        /// <summary>
        /// Script for revert migration.
        /// </summary>
        public string DownScript { get; init; } = "";

    }

}