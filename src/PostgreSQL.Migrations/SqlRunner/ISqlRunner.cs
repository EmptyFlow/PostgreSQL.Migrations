﻿using PostgreSQL.Migrations.Runner;

namespace PostgreSQL.Migrations.SqlRunner {

    /// <summary>
    /// Common interface for interacting with SQL providers.
    /// </summary>
    public interface ISqlRunner {

        /// <summary>
        /// Begin database transaction.
        /// </summary>
        /// <returns></returns>
        Task BeginTransactionAsync ( string connectionString );

        /// <summary>
        /// Commit database transaction.
        /// </summary>
        /// <returns></returns>
        Task CommitTransactionAsync ( string connectionString );

        /// <summary>
        /// Get number of already applied migrations for connection string passed via parameter.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns>Collection of numbers.</returns>
        Task<IEnumerable<int>> GetAppliedMigrations ( string connectionString );

        /// <summary>
        /// Apply migration.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="migration">Migrations for apply.</param>
        Task ApplyMigrationAsync ( string connectionString, AvailableMigration migration );

        /// <summary>
        /// Apply migration.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="migration">Migrations for apply.</param>
        Task RevertMigration ( string connectionString, AvailableMigration migration );

    }

}
