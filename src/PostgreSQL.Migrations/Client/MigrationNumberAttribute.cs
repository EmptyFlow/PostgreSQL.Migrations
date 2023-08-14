namespace PostgreSQL.Migrations.Client {


    [AttributeUsage ( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public class MigrationNumberAttribute : Attribute {

        public int MigrationNumber { get; set; }

        public string Issue { get; set; }

        public MigrationNumberAttribute ( int migrationNumber, string issue ) {
            MigrationNumber = migrationNumber;
            Issue = issue;
        }

    }

}
