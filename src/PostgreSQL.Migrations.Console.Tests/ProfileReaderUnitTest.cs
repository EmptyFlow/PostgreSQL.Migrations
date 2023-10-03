using PostgreSQL.Migrations.Console.Options;

namespace PostgreSQL.Migrations.Console.Tests {

    public class ProfileReaderUnitTest {

        [Fact]
        public void Read_Completed_SingleConnectionString () {
            //arrange
            var content = "constring Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=pooldatabase";

            //act
            var result = ProfileReader.Read<ApplyOptions> ( content );

            //assert
            Assert.NotNull ( result );
            Assert.NotEmpty ( result.ConnectionStrings );
            Assert.Equal ( "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=pooldatabase", result.ConnectionStrings.First () );
            Assert.True ( result.ConnectionStrings.Count () == 1 );
        }

        [Fact]
        public void Read_Completed_MiltipleConnectionString () {
            //arrange
            var content = """
constring Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=pooldatabase
constring Host=localhost2;Port=5433;Username=postgres;Password=postgres;Database=pooldatabase
constring Host=localhost3;Port=5434;Username=postgres1;Password=postgres2;Database=pooldatabase
""";

            //act
            var result = ProfileReader.Read<ApplyOptions> ( content );

            //assert
            Assert.NotNull ( result );
            Assert.NotEmpty ( result.ConnectionStrings );
            Assert.Equal ( "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=pooldatabase", result.ConnectionStrings.First () );
            Assert.Equal ( "Host=localhost2;Port=5433;Username=postgres;Password=postgres;Database=pooldatabase", result.ConnectionStrings.ElementAt ( 1 ) );
            Assert.Equal ( "Host=localhost3;Port=5434;Username=postgres1;Password=postgres2;Database=pooldatabase", result.ConnectionStrings.Last () );
            Assert.True ( result.ConnectionStrings.Count () == 3 );
        }

        [Fact]
        public void Read_Completed_SingleFile () {
            //arrange
            var content = @"file C:\lalalal\bububub\mumumumu\migration.dll";

            //act
            var result = ProfileReader.Read<ApplyOptions> ( content );

            //assert
            Assert.NotNull ( result );
            Assert.NotEmpty ( result.Files );
            Assert.Equal ( @"C:\lalalal\bububub\mumumumu\migration.dll", result.Files.First () );
            Assert.True ( result.Files.Count () == 1 );
        }

        [Fact]
        public void Read_Completed_MultipleFiles () {
            //arrange
            var content = """
file C:\lalalal\bububub\mumumumu\migration.dll
file C:\lalalal1\bububub1\mumumumu\migration2.dll
file C:\lalalal2\bububub2\mumumumu\migration3.dll
""";

            //act
            var result = ProfileReader.Read<ApplyOptions> ( content );

            //assert
            Assert.NotNull ( result );
            Assert.NotEmpty ( result.Files );
            Assert.Equal ( @"C:\lalalal\bububub\mumumumu\migration.dll", result.Files.First () );
            Assert.Equal ( @"C:\lalalal1\bububub1\mumumumu\migration2.dll", result.Files.ElementAt ( 1 ) );
            Assert.Equal ( @"C:\lalalal2\bububub2\mumumumu\migration3.dll", result.Files.Last () );
            Assert.True ( result.Files.Count () == 3 );
        }

        [Fact]
        public void Read_Completed_Strategy () {
            //arrange
            var content = "strategy TestStrategy";

            //act
            var result = ProfileReader.Read<ApplyOptions> ( content );

            //assert
            Assert.NotNull ( result );
            Assert.NotEmpty ( result.Strategy );
            Assert.Equal ( "TestStrategy", result.Strategy );
        }

        [Fact]
        public void Read_Completed_Group () {
            //arrange
            var content = "group TestGroup";

            //act
            var result = ProfileReader.Read<ApplyOptions> ( content );

            //assert
            Assert.NotNull ( result );
            Assert.NotEmpty ( result.Group );
            Assert.Equal ( "TestGroup", result.Group );
        }

    }

}