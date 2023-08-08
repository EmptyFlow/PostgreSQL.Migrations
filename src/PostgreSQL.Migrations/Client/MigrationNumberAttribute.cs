namespace PostgreSQL.Migrations.Client {


    [AttributeUsage ( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public class MigrationNumberAttribute : Attribute {

        public int MigrationNumber { get; set; }

        public MigrationNumberAttribute ( int migrationNumber ) => MigrationNumber = migrationNumber;

    }

}
