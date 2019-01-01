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
using Sannel.House.Devices.Data;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sannel.House.Devices.Repositories
{
	public class DbContextRepository : IDeviceRepository
	{
		private DevicesDbContext context;
		/// <summary>
		/// Initializes a new instance of the <see cref="DbContextRepository"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <exception cref="ArgumentNullException">context</exception>
		public DbContextRepository([NotNull]DevicesDbContext context)
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Gets the device by identifier asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public Task<Device> GetDeviceByIdAsync(int deviceId)
			=> context.Devices.AsNoTracking().FirstOrDefaultAsync(i => i.DeviceId == deviceId);

		/// <summary>
		/// Gets the device by mac address asynchronous.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		public async Task<Device> GetDeviceByMacAddressAsync(long macAddress)
		{
			var alt = await context.AlternateDeviceIds
				.Include(nameof(AlternateDeviceId.Device)).AsNoTracking()
				.FirstOrDefaultAsync(i => i.MacAddress == macAddress);
			return alt?.Device;
		}



		/// <summary>
		/// Gets the devices list asynchronous.
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <returns></returns>
		public async Task<PagedResults<Device>> GetDevicesListAsync(int pageIndex, int pageSize)
		{
			var result = await Task.Run(() => new PagedResults<Device>
			{
				Page = pageIndex,
				PageSize = pageSize,
				TotalCount = context.Devices.LongCount(),
				Data = context.Devices.AsNoTracking()
							.OrderBy(i => i.DisplayOrder)
							.Skip(pageIndex * pageSize)
							.Take(pageSize)
			});

			return result;
		}

		/// <summary>
		/// Gets the device by UUID/Guid asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID/Guid.</param>
		/// <returns></returns>
		public async Task<Device> GetDeviceByUuidAsync(Guid uuid)
		{
			var alt = await context.AlternateDeviceIds
				.Include(nameof(AlternateDeviceId.Device))
				.AsNoTracking()
				.FirstOrDefaultAsync(i => i.Uuid == uuid);
			return alt?.Device;
		}

		/// <summary>
		/// Creates the device asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">device</exception>
		public async Task<Device> AddDeviceAsync(Device device)
		{
			if(device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}

			device.DeviceId = 0; // reset deviceId to 1 so its auto generated.
			var count = await context.Devices.CountAsync();

			var displayOrder = (count > 0)?await context.Devices.MaxAsync(i => i.DisplayOrder):0;
			device.DisplayOrder = displayOrder + 1;
			var result = await context.Devices.AddAsync(device);
			await context.SaveChangesAsync();

			var id = result.Entity.DeviceId;

			return await context.Devices.AsNoTracking().FirstOrDefaultAsync(i => i.DeviceId == id);
		}

		/// <summary>
		/// Updates the device asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">device is null</exception>
		/// <exception cref="ReadOnlyException">The device is marked read only</exception>
		public async Task<Device> UpdateDeviceAsync(Device device)
		{
			if(device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}

			var d = await context.Devices.FirstOrDefaultAsync(i => i.DeviceId == device.DeviceId);

			if(d == null)
			{
				return null;
			}

			if(d.IsReadOnly)
			{
				throw new ReadOnlyException($"Device {d.DeviceId} is Read Only");
			}

			d.Name = device.Name;
			d.Description = device.Description;
			d.DisplayOrder = device.DisplayOrder;
			await context.SaveChangesAsync();

			return await context.Devices.AsNoTracking().FirstOrDefaultAsync(i => i.DeviceId == d.DeviceId);
		}

		/// <summary>
		/// Adds the alternate mac address asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="macAddress">The mac address.</param>
		/// <returns>
		/// The device or null if there is no device with <paramref name="deviceId" />
		/// </returns>
		/// <exception cref="AlternateDeviceIdException">If the macAddress is already connected to another device</exception>
		public async Task<Device> AddAlternateMacAddressAsync(int deviceId, long macAddress)
		{
			var device = await context.Devices.AsNoTracking().FirstOrDefaultAsync(i => i.DeviceId == deviceId);
			if (device == null)
			{
				return null;
			}

			var altId = await context.AlternateDeviceIds.AsNoTracking().FirstOrDefaultAsync(i => i.MacAddress == macAddress);
			if (altId != null)
			{
				throw new AlternateDeviceIdException($"The macAddress {macAddress} is already associated with a device {altId.DeviceId}");
			}

			altId = new AlternateDeviceId()
			{
				DeviceId = device.DeviceId,
				DateCreated = DateTime.UtcNow,
				MacAddress = macAddress
			};

			await context.AlternateDeviceIds.AddAsync(altId);
			await context.SaveChangesAsync();

			return device;
		}

		/// <summary>
		/// Adds the alternate UUID asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="uuid">The UUID.</param>
		/// <returns>
		/// The device or null if there is no device with <paramref name="deviceId" />
		/// </returns>
		/// <exception cref="AlternateDeviceIdException">If the Uuid is already associated with a device</exception>
		public async Task<Device> AddAlternateUuidAsync(int deviceId, Guid uuid)
		{
			var device = await context.Devices.AsNoTracking().FirstOrDefaultAsync(i => i.DeviceId == deviceId);
			if (device == null)
			{
				return null;
			}

			var altId = await context.AlternateDeviceIds.AsNoTracking().FirstOrDefaultAsync(i => i.Uuid == uuid);
			if (altId != null)
			{
				throw new AlternateDeviceIdException($"The Uuid {uuid} is already associated with a device {altId.DeviceId}");
			}

			altId = new AlternateDeviceId()
			{
				DeviceId = device.DeviceId,
				DateCreated = DateTime.UtcNow,
				Uuid = uuid
			};

			await context.AlternateDeviceIds.AddAsync(altId);
			await context.SaveChangesAsync();

			return device;
		}

		/// <summary>
		/// Removes the alternate mac address asynchronous.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns>
		/// The device or null if the macAddress is not found
		/// </returns>
		public async Task<Device> RemoveAlternateMacAddressAsync(long macAddress)
		{
			var altId = await context.AlternateDeviceIds.Include(i => i.Device)
				.AsNoTracking().FirstOrDefaultAsync(i => i.MacAddress == macAddress);
			if (altId == null)
			{
				return null;
			}

			context.AlternateDeviceIds.Remove(altId);
			await context.SaveChangesAsync();

			return altId.Device;
		}

		/// <summary>
		/// Removes the alternate UUID asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns>
		/// The device or null if the uuid is not found
		/// </returns>
		public async Task<Device> RemoveAlternateUuidAsync(Guid uuid)
		{
			var altId = await context.AlternateDeviceIds.Include(i => i.Device)
				.AsNoTracking().FirstOrDefaultAsync(i => i.Uuid == uuid);
			if (altId == null)
			{
				return null;
			}

			context.AlternateDeviceIds.Remove(altId);
			await context.SaveChangesAsync();

			return altId.Device;
		}

		/// <summary>
		/// Gets the alternate ids for the device asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public async Task<IEnumerable<AlternateDeviceId>> GetAlternateIdsForDeviceAsync(int deviceId)
		{
			if(await context.Devices.CountAsync() == 0)
			{
				return null;
			}

			return await Task.Run(() => context.AlternateDeviceIds
					.AsNoTracking().Where(i => i.DeviceId == deviceId));
		}
	}
}
