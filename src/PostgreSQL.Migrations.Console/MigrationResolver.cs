﻿using PostgreSQL.Migrations.Console.Strategies;
using Database.Migrations;
using SystemConsole = System.Console;
using Migrations.Console.Strategies;

namespace PostgreSQL.Migrations.Console {

	public static class MigrationResolver {

		private const string CSharpClassesStrategy = "CSharpClasses";

		private const string PlainSqlStrategy = "PlainSql";

		private const string PlainPackStrategy = "Packed";

		public const string DefaultStrategy = CSharpClassesStrategy;

		public static IMigrationsAsyncResolver GetResolver ( string strategy ) {
			return strategy switch {
				CSharpClassesStrategy => new MigrationNumberAttributeResolver (),
				PlainSqlStrategy => new PlainSqlFileResolver (),
				PlainPackStrategy => new PackFileResolver(),
				_ => throw new NotSupportedException ( $"Strategy {strategy} not supported!" )
			};
		}

		public static async Task<List<IMigrationsAsyncResolver>> GetResolvers ( IEnumerable<string> files, string group, string strategy ) {
			SystemConsole.WriteLine ( $"Trying to use a strategy: {strategy}..." );
			var migrationResolvers = new List<IMigrationsAsyncResolver> ();

			switch ( strategy ) {
				case CSharpClassesStrategy:
					migrationResolvers.AddRange ( await StrategyCSharpClassesResolver.Run ( files, group ) );
					break;
				case PlainSqlStrategy:
					migrationResolvers.AddRange ( await StrategyPlainSqlFileResolver.Run ( files, group ) );
					break;
				case PlainPackStrategy:
					migrationResolvers.AddRange ( await StrategyPackedFileResolver.Run ( files, group ) );
					break;
				default: throw new NotSupportedException ( $"Strategy {strategy} not supported!" );
			}

			SystemConsole.WriteLine ( $"Strategy {strategy} applied!" );

			return migrationResolvers;
		}

	}
}
