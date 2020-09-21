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
	public abstract class BaseTests : Sannel.House.Base.Tests.BaseTests<DevicesDbContext>
	{
		/// <summary>
		/// Asserts that Devices are equal
		/// </summary>
		/// <param name="expected">The expected.</param>
		/// <param name="actual">The actual.</param>
		protected void AssertEqual(Device? expected, Device? actual)
		{
			if(expected is null)
			{
				Assert.Null(actual);
				return;
			}
			else
			{
				Assert.NotNull(actual);
			}

			// fix compiler warning
			if(actual is null)
			{
				throw new NullReferenceException("Not sure how we got here should not have been null at this point");
			}

			Assert.Equal(expected.DeviceId, actual.DeviceId);
			Assert.Equal(expected.Name, actual.Name);
			Assert.Equal(expected.IsReadOnly, actual.IsReadOnly);
			Assert.Equal(expected.Description, actual.Description);
			Assert.Equal(expected.DateCreated, actual.DateCreated);
			Assert.Equal(expected.DisplayOrder, actual.DisplayOrder);
		}

		public override DevicesDbContext CreateDbContext(DbContextOptions options) 
			=> new DevicesDbContext(options);

		public override Type MigrationAssemblyType => typeof(DevicesDesignTimeFactory);
	}
}
