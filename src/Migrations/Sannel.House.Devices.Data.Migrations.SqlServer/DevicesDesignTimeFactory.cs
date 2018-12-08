using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Devices.Data.Migrations.SqlServer
{
	public class DevicesDesignTimeFactory : IDesignTimeDbContextFactory<DevicesDbContext>
	{
		public DevicesDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<DevicesDbContext>();


			builder.UseSqlServer("server=localhost", o => o.MigrationsAssembly(GetType().Assembly.GetName().FullName));

			return new DevicesDbContext(builder.Options);

		}
	}
}
