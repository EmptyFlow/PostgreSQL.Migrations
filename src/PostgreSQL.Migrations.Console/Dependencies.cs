﻿using Database.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PostgreSQL.Migrations.SqlRunner;

namespace PostgreSQL.Migrations.Console {

	public static class Dependencies {

		private static ServiceProvider? m_serviceCollection;

		public static void RegisterDependencies () {
			m_serviceCollection = new ServiceCollection ()
				.AddSingleton<ISqlRunner, PostgresSqlRunner> ()
				.BuildServiceProvider ();
		}

		public static T GetService<T> () {
			var service = m_serviceCollection?.GetService ( typeof ( T ) );
			return service == null ? throw new ArgumentException ( $"Dependency {typeof ( T ).FullName} can't resolved" ) : (T) service;
		}

	}

}
