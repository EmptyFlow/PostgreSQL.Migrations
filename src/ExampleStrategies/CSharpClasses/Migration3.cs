using PostgreSQL.Migrations.Client;

namespace MyProject.Migrations
{

    [MigrationNumber(3,"https://bugtracker.com/project/3","group3")]
    public class Migration3 : MigrationScript
    {

        /// <summary>
        /// The Down method must return SQL commands to return the database to the state before the SQL commands from the Up method were executed.
        /// </summary>
        public override string Down () => "DROP TABLE IF EXISTS test3;";

        /// <summary>
        /// The Up method should return SQL commands to change the structure or data in the database.
        /// </summary>
        public override string Up () => "CREATE TABLE test3(id date DEFAULT now());";

    }

}
