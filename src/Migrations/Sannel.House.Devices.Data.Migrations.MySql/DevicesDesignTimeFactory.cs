using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;
using Pomelo.EntityFrameworkCore.MySql;

namespace Sannel.House.Devices.Data.Migrations.MySql
{
	public class DevicesDesignTimeFactory : IDesignTimeDbContextFactory<MySqlDbContext>
	{
		public MySqlDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<MySqlDbContext>();


			builder.UseMySql("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;");

			return new MySqlDbContext(builder.Options);

		}
	}
}
