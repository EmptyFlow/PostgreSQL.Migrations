using Database.Migrations;
using System.Text.Json.Serialization;

namespace Migrations.Console.JsonSerializers {

	[JsonSerializable ( typeof ( List<AvailableMigration> ) )]
	[JsonSerializable ( typeof ( AvailableMigration ) )]
	public partial class PackSerializer : JsonSerializerContext {

	}

}
