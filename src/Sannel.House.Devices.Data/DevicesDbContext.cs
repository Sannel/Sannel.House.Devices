/* Copyright 2019 Sannel Software, L.L.C.
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
      http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Devices.Data
{
	public class DevicesDbContext : DbContext
	{
		public DbSet<Device> Devices { get; set; }

		public DbSet<AlternateDeviceId> AlternateDeviceIds { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public DevicesDbContext([NotNull] DbContextOptions options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			if(modelBuilder is null)
			{
				throw new ArgumentNullException(nameof(modelBuilder));
			}

			var altids = modelBuilder.Entity<AlternateDeviceId>();
			altids.HasIndex(i => i.MacAddress).IsUnique();
			altids.HasIndex(i => i.Uuid).IsUnique();
			altids.HasIndex(nameof(AlternateDeviceId.Manufacture), nameof(AlternateDeviceId.ManufactureId)).IsUnique();

			var devices = modelBuilder.Entity<Device>();
			devices.HasIndex(i => i.DisplayOrder);
		}
	}
}
