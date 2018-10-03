using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Devices.Data.Migrations.SqlServer
{
	public class DevicesDesignTimeFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
	{
		public SqlServerDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<SqlServerDbContext>();


			builder.UseSqlServer("server=localhost");

			return new SqlServerDbContext(builder.Options);

		}
	}
}
