
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Sannel.House.Devices.Data.Migrations.MySql
{
	public class MySqlDbContext : Data.DevicesDbContext
	{
		public MySqlDbContext([NotNull] DbContextOptions options) : base(options)
		{
		}
	}
}
