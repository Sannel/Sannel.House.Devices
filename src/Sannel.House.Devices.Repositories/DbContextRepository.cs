/* Copyright 2018 Sannel Software, L.L.C.
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
			=> context.Devices.FirstOrDefaultAsync(i => i.DeviceId == deviceId);

		/// <summary>
		/// Gets the device by mac address asynchronous.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		public async Task<Device> GetDeviceByMacAddressAsync(long macAddress)
		{
			var alt = await context.AlternateDeviceIds
				.Include(nameof(AlternateDeviceId.Device))
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
				Data = context.Devices
							.OrderBy(i => i.DisplayOrder)
							.Skip((pageIndex - 1) * pageSize)
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

			var displayOrder = await context.Devices.MaxAsync(i => i.DisplayOrder);
			device.DisplayOrder = displayOrder + 1;
			var result = await context.Devices.AddAsync(device);
			await context.SaveChangesAsync();

			return result.Entity;
		}

		public Task<Device> UpdateDeviceAsync(Device device)
		{
			throw new NotImplementedException();
		}

		public Task<Device> AddAlternateMacAddressAsync(int deviceId, long macAddress)
		{
			throw new NotImplementedException();
		}

		public Task<Device> AddAlternateUuidAsync(int deviceId, Guid uuid)
		{
			throw new NotImplementedException();
		}

		public Task<Device> RemoveAlternateMacAddressAsync(long macAddress)
		{
			throw new NotImplementedException();
		}

		public Task<Device> RemoveAlternateUuidAsync(Guid uuid)
		{
			throw new NotImplementedException();
		}
	}
}
