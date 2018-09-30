using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Sannel.House.Devices.Data.Migrations.MySql
{
	public class SqliteDbContext : DevicesDbContext
	{
		public SqliteDbContext([NotNull] DbContextOptions options) : base(options)
		{
		}
	}
}