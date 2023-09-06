namespace PostgreSQL.Migrations.Client {


    [AttributeUsage ( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public class MigrationNumberAttribute : Attribute {

        public int MigrationNumber { get; init; }

        public string Issue { get; init; }

        public string Group { get; init; }

        public MigrationNumberAttribute ( int migrationNumber, string issue, string group = "" ) {
            MigrationNumber = migrationNumber;
            Issue = issue;
            Group = group;
        }

    }

}
