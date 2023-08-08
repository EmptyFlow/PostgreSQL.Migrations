﻿namespace PostgreSQL.Migrations.Runner {

    /// <summary>
    /// An interface to implement a asynchronize method for getting available migrations.
    /// </summary>
    public interface IMigrationsAsyncResolver {


        /// <summary>
        /// Get all available migrations.
        /// </summary>
        Task<IEnumerable<AvailableMigration>> GetMigrationsAsync ();

    }

}
