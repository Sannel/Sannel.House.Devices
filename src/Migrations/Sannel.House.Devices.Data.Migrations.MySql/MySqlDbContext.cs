using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Sannel.House.Devices.Data.Migrations.MySql
{
	public class MySqlDbContext : DevicesDbContext
	{
		public MySqlDbContext([NotNull] DbContextOptions options) : base(options)
		{
		}
	}
}
