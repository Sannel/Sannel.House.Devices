using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Sannel.House.Devices.Data.Migrations.MySql
{
	public class SqlServerDbContext : DevicesDbContext
	{
		public SqlServerDbContext([NotNull] DbContextOptions options) : base(options)
		{
		}
	}
}