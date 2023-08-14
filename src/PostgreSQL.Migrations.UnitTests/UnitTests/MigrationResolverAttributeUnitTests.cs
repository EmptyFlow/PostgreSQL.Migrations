using PostgreSQL.Migrations.Client;
using System.Reflection;
using Xunit;

namespace PostgreSQL.Migrations.UnitTests.UnitTests {

    [MigrationNumber ( 1, "http://issue/1" )]
    public class InitialMigration : MigrationScript {

        public override string Down () => "initial migration down script";

        public override string Up () => "initial migration up script";

    }

    [MigrationNumber ( 2, "http://issue/2" )]
    public class SecondMigration : MigrationScript {

        public override string Down () => "second migration down script";

        public override string Up () => "second migration up script";

    }

    public class MigrationResolverAttributeUnitTests {

        [Fact, Trait ( "Category", "Unit" )]
        public void GetMigrations_Completed () {
            //arrange
            var service = new MigrationResolverAttribute ();

            service.AddAssemblies ( new List<Assembly> { typeof ( MigrationResolverAttributeUnitTests ).Assembly } );

            //act
            var migrations = service.GetMigrations ();

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

    }

}
