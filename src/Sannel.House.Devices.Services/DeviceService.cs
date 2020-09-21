/* Copyright 2020-2020 Sannel Software, L.L.C.
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
      http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sannel.House.Base.Messages.Device;
using Sannel.House.Base.MQTT.Interfaces;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannel.House.Devices.Services
{
	public class DeviceService : IDeviceService
	{
		private readonly IDeviceRepository repository;
		private readonly IMqttClientPublishService mqttClient;
		private readonly ILogger logger;

		public DeviceService(IDeviceRepository repository, IMqttClientPublishService mqttClient, ILogger<DeviceService> logger)
		{
			this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
			this.mqttClient = mqttClient ?? throw new ArgumentNullException(nameof(mqttClient));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Publishes the device to the MQTT Service asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public Task PublishDeviceAsync(int deviceId)
			=> sendDeviceUpdateAsync(deviceId);

		private async Task sendDeviceUpdateAsync(int deviceId)
		{
			var device = await repository.GetDeviceByIdAsync(deviceId).ConfigureAwait(false);

			if(device is null)
			{
				return;
			}

			await sendDeviceUpdateAsync(device);
		}

		private async Task sendDeviceUpdateAsync(Device device)
		{
			if(device is null)
			{
				return;
			}

			var message = new DeviceMessage()
			{
				DeviceId = device.DeviceId,
				DateCreated = device.DateCreated,
				Verified = device.Verified
			};

			var alternatIds = await repository.GetAlternateIdsForDeviceAsync(device.DeviceId);

			message.AlternateIds = alternatIds.Select(i => new AlternateIdMessage()
			{
				AlternateId = i.AlternateId,
				DateCreated = i.DateCreated,
				Uuid = i.Uuid,
				MacAddress = i.MacAddress,
				Manufacture = i.Manufacture,
				ManufactureId = i.ManufactureId
			}).ToList();

			mqttClient.Publish(message);
		}

		/// <summary>
		/// Adds the alternate mac address asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="macAddress">The mac address.</param>
		/// <returns>
		/// The device or null if there is no device with <paramref name="deviceId" />
		/// </returns>
		public async Task<Device> AddAlternateMacAddressAsync(int deviceId, long macAddress)
		{
			var result = await repository.AddAlternateMacAddressAsync(deviceId, macAddress);

			if(result != null)
			{
				await sendDeviceUpdateAsync(deviceId);
			}

			return result;
		}

		/// <summary>
		/// Adds the alternate device identifier asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns></returns>
		public async Task<Device> AddAlternateManufactureIdAsync(int deviceId, string manufacture, string manufactureId)
		{
			var result = await repository.AddAlternateManufactureIdAsync(deviceId, manufacture, manufactureId);

			if(result != null)
			{
				await sendDeviceUpdateAsync(deviceId);
			}

			return result;
		}

		/// <summary>
		/// Adds the alternate UUID asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="uuid">The UUID.</param>
		/// <returns>
		/// The device or null if there is no device with <paramref name="deviceId" />
		/// </returns>
		public async Task<Device> AddAlternateUuidAsync(int deviceId, Guid uuid)
		{

			var result = await repository.AddAlternateUuidAsync(deviceId, uuid);

			if(result != null)
			{
				await sendDeviceUpdateAsync(deviceId);
			}

			return result;
		}

		/// <summary>
		/// Creates the device asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		public async Task<Device> AddDeviceAsync(Device device)
		{
			var result = await repository.AddDeviceAsync(device);

			if(result != null)
			{
				await sendDeviceUpdateAsync(device.DeviceId);
			}

			return result;
		}

		/// <summary>
		/// Gets the alternate ids for device asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public Task<IEnumerable<AlternateDeviceId>> GetAlternateIdsForDeviceAsync(int deviceId) 
			=> repository.GetAlternateIdsForDeviceAsync(deviceId);

		/// <summary>
		/// Gets the device by identifier asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public Task<Device> GetDeviceByIdAsync(int deviceId)
			=> repository.GetDeviceByIdAsync(deviceId);

		/// <summary>
		/// Gets the device by mac address asynchronous.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		public Task<Device> GetDeviceByMacAddressAsync(long macAddress)
			=> repository.GetDeviceByMacAddressAsync(macAddress);

		/// <summary>
		/// Gets the device by manufacture identifier asynchronous.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns></returns>
		public Task<Device> GetDeviceByManufactureIdAsync(string manufacture, string manufactureId)
			=> repository.GetDeviceByManufactureIdAsync(manufacture, manufactureId);

		/// <summary>
		/// Gets the device by UUID/Guid asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID/Guid.</param>
		/// <returns></returns>
		public Task<Device> GetDeviceByUuidAsync(Guid uuid)
			=> repository.GetDeviceByUuidAsync(uuid);

		public Task<PagedResults<Device>> GetDevicesListAsync(int pageIndex, int pageSize)
			=> repository.GetDevicesListAsync(pageIndex, pageSize);

		/// <summary>
		/// Removes the alternate mac address asynchronous.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns>
		/// The device or null if the macAddress is not found
		/// </returns>
		public async Task<Device> RemoveAlternateMacAddressAsync(long macAddress)
		{
			var result = await repository.RemoveAlternateMacAddressAsync(macAddress);

			if(result != null)
			{
				await sendDeviceUpdateAsync(result.DeviceId);
			}

			return result;
		}

		/// <summary>
		/// Removes the alternate manufacture identifier asynchronous.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns></returns>
		public async Task<Device> RemoveAlternateManufactureIdAsync(string manufacture, string manufactureId)
		{
			var result = await repository.RemoveAlternateManufactureIdAsync(manufacture, manufactureId);

			if(result != null)
			{
				await sendDeviceUpdateAsync(result.DeviceId);
			}

			return result;
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
			var result = await repository.RemoveAlternateUuidAsync(uuid);

			if(result != null)
			{
				await sendDeviceUpdateAsync(result.DeviceId);
			}

			return result;
		}

		/// <summary>
		/// Updates the device asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		public async Task<Device> UpdateDeviceAsync(Device device)
		{
			var result = await repository.UpdateDeviceAsync(device);

			if(result != null)
			{
				await sendDeviceUpdateAsync(result.DeviceId);
			}

			return result;
		}

		/// <summary>
		/// Publishes the device to the MQTT Service asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <exception cref="System.ArgumentNullException">device</exception>
		public async Task PublishDeviceAsync(Device device)
		{
			if(device is null)
			{
				throw new ArgumentNullException(nameof(device));
			}

			await sendDeviceUpdateAsync(device);
		}
	}
}
