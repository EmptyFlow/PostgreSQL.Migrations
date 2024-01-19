using Database.Migrations;
using System.Reflection;
using Xunit;

namespace PostgreSQL.Migrations.UnitTests.UnitTests {

	public class InitialMigration {

		public int MigrationNumber => 1;

		public string Issue => "http://issue/1";

		public string Group => "firstGroup";

		public string Down () => "initial migration down script";

        public string Up () => "initial migration up script";

    }

    public class SecondMigration {

		public static int MigrationNumber => 2;

		public static string Issue => "http://issue/2";

		public static string Group => "lastGroup";


		public string Down () => "second migration down script";

        public string Up () => "second migration up script";

    }

    public class MigrationResolverAttributeUnitTests {

        [Fact, Trait ( "Category", "Unit" )]
        public async Task GetMigrations_Completed () {
            //arrange
            var service = new MigrationNumberAttributeResolver ();

            service.AddAssemblies ( new List<Assembly> { typeof ( MigrationResolverAttributeUnitTests ).Assembly } );

            //act
            var migrations = await service.GetMigrationsAsync ();

            //assert
            Assert.True ( migrations.Count () == 2 );
            var firstMigration = migrations.FirstOrDefault ( a => a.MigrationNumber == 1 );
            Assert.NotNull ( firstMigration );
            Assert.Equal ( 1, firstMigration.MigrationNumber );
            Assert.Equal ( "http://issue/1", firstMigration.Issue );
            Assert.Equal ( "initial migration up script", firstMigration.UpScript );
            Assert.Equal ( "initial migration down script", firstMigration.DownScript );
            var secondMigration = migrations.FirstOrDefault ( a => a.MigrationNumber == 2 );
            Assert.NotNull ( secondMigration );
            Assert.Equal ( 2, secondMigration.MigrationNumber );
            Assert.Equal ( "http://issue/2", secondMigration.Issue );
            Assert.Equal ( "second migration up script", secondMigration.UpScript );
            Assert.Equal ( "second migration down script", secondMigration.DownScript );
        }

        [Fact, Trait ( "Category", "Unit" )]
        public async Task GetMigrations_GroupFilter_SingleGroup_Completed () {
            //arrange
            var service = new MigrationNumberAttributeResolver ();

            service.AddAssemblies ( new List<Assembly> { typeof ( MigrationResolverAttributeUnitTests ).Assembly } );
            service.SetGroup ( "lASTgrouP" );

            //act
            var migrations = await service.GetMigrationsAsync ();

            //assert
            Assert.True ( migrations.Count () == 1 );
            var firstMigration = migrations.FirstOrDefault ( a => a.MigrationNumber == 2 );
            Assert.NotNull ( firstMigration );
            Assert.Equal ( 2, firstMigration.MigrationNumber );
        }

        [Fact, Trait ( "Category", "Unit" )]
        public async Task GetMigrations_GroupFilter_MultipleGroups_Completed () {
            //arrange
            var service = new MigrationNumberAttributeResolver ();

            service.AddAssemblies ( new List<Assembly> { typeof ( MigrationResolverAttributeUnitTests ).Assembly } );
            service.SetGroup ( "lAsTgrouP, Firstgroup" );

            //act
            var migrations = await service.GetMigrationsAsync ();

            //assert
            Assert.True ( migrations.Count () == 2 );
            var firstMigration = migrations.FirstOrDefault ( a => a.MigrationNumber == 1 );
            Assert.NotNull ( firstMigration );
            Assert.Equal ( 1, firstMigration.MigrationNumber );
            var secondMigration = migrations.FirstOrDefault ( a => a.MigrationNumber == 2 );
            Assert.NotNull ( secondMigration );
            Assert.Equal ( 2, secondMigration.MigrationNumber );
        }

        [Fact, Trait ( "Category", "Unit" )]
        public async Task GetMigrations_GroupFilter_MultipleGroups_SecondGroupInAttribute_Completed () {
            //arrange
            var service = new MigrationNumberAttributeResolver ();

            service.AddAssemblies ( new List<Assembly> { typeof ( MigrationResolverAttributeUnitTests ).Assembly } );
            service.SetGroup ( "lalala, Firstgroup" );

            //act
            var migrations = await service.GetMigrationsAsync ();

            //assert
            Assert.True ( migrations.Count () == 2 );
            var firstMigration = migrations.FirstOrDefault ( a => a.MigrationNumber == 1 );
            Assert.NotNull ( firstMigration );
            Assert.Equal ( 1, firstMigration.MigrationNumber );
            var secondMigration = migrations.FirstOrDefault ( a => a.MigrationNumber == 2 );
            Assert.NotNull ( secondMigration );
            Assert.Equal ( 2, secondMigration.MigrationNumber );
        }

    }

}
