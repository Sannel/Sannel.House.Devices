using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sannel.House.Devices.Data;
using Sannel.House.Devices.Data.Migrations.Sqlite;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sannel.House.Devices.Tests
{
	public abstract class BaseTests : IDisposable
	{
		private ILoggerFactory loggerFactory;

		/// <summary>
		/// Creates the logger.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public ILogger<T> CreateLogger<T>()
		{
			var l = loggerFactory ?? new LoggerFactory();
			return l.CreateLogger<T>();
		}

		/// <summary>
		/// Asserts that Devices are equal
		/// </summary>
		/// <param name="expected">The expected.</param>
		/// <param name="actual">The actual.</param>
		protected void AssertEqual(Device expected, Device actual)
		{
			if(expected == null)
			{
				Assert.Null(actual);
			}
			else
			{
				Assert.NotNull(actual);
			}

			Assert.Equal(expected.DeviceId, actual.DeviceId);
			Assert.Equal(expected.Name, actual.Name);
			Assert.Equal(expected.IsReadOnly, actual.IsReadOnly);
			Assert.Equal(expected.Description, actual.Description);
			Assert.Equal(expected.DateCreated, actual.DateCreated);
			Assert.Equal(expected.DisplayOrder, actual.DisplayOrder);
		}

		/// <summary>
		/// Opens the connection be sure to dispose it.
		/// </summary>
		/// <returns></returns>
		protected SqliteConnection OpenConnection()
		{
			var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();
			return connection;
		}

		/// <summary>
		/// Gets the test dbcontext be sure to dispose it.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <returns></returns>
		public DevicesDbContext GetTestDB(SqliteConnection connection)
		{
			var d = new DbContextOptionsBuilder();
			d.ConfigureSqlite(connection);

			var context = new DevicesDbContext(d.Options);
			context.Database.Migrate();
			return context;
		}

		public void Dispose()
		{
		}
	}
}
