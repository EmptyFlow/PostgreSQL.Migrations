using System.Text.Json.Serialization;

namespace Database.Migrations.JsonSerializers {

	[JsonSerializable ( typeof ( List<AvailableMigration> ) )]
	[JsonSerializable ( typeof ( AvailableMigration ) )]
	public partial class PackSerializer : JsonSerializerContext {

	}

}
