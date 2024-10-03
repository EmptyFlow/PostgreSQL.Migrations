namespace Database.Migrations {

    /// <summary>
    /// Available migration.
    /// </summary>
    public record AvailableMigration {

		/// <summary>
		/// Sequential migration number.
		/// </summary>
		public int MigrationNumber { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Issue number or link.
        /// </summary>
        public string Issue { get; set; } = "";

        /// <summary>
        /// Migration group(s).
        /// </summary>
        public string Group { get; set; } = "";

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