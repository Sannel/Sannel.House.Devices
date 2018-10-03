using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Devices.Data.Migrations.Sqlite
{
	public class DevicesDesignTimeFactory : IDesignTimeDbContextFactory<SqliteDbContext>
	{
		public SqliteDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<SqliteDbContext>();


			builder.UseSqlite("data source=db.db");

			return new SqliteDbContext(builder.Options);

		}
	}
}
