namespace PostgreSQL.Migrations.Runner {

    /// <summary>
    /// A logger implementation that writes log messages to the console.
    /// </summary>
    public class ConsoleMigrationRunnerLogger : IMigrationRunnerLogger {

        public void Log ( string message ) => Console.WriteLine ( message );

    }

}
