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
using Sannel.House.Devices.Models;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sannel.House.Devices.Interfaces
{
	public interface IDeviceRepository
	{
		/// <summary>
		/// Gets the devices list asynchronous.
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <returns></returns>
		Task<PagedResults<Device>> GetDevicesListAsync(int pageIndex, int pageSize);

		/// <summary>
		/// Gets the device by identifier asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		Task<Device> GetDeviceByIdAsync(int deviceId);

		/// <summary>
		/// Gets the device by mac address asynchronous.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		Task<Device> GetDeviceByMacAddressAsync(long macAddress);

		/// <summary>
		/// Gets the device by UUID/Guid asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID/Guid.</param>
		/// <returns></returns>
		Task<Device> GetDeviceByUuidAsync(Guid uuid);

		/// <summary>
		/// Gets the device by manufacture identifier asynchronous.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns></returns>
		Task<Device> GetDeviceByManufactureIdAsync(string manufacture, string manufactureId);

		/// <summary>
		/// Creates the device asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		Task<Device> AddDeviceAsync(Device device);

		/// <summary>
		/// Updates the device asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <exception cref="ReadOnlyException">The device is marked as read only and cannot be updated</exception>
		/// <returns></returns>
		Task<Device> UpdateDeviceAsync(Device device);

		/// <summary>
		/// Adds the alternate mac address asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="macAddress">The mac address.</param>
		/// <exception cref="AlternateDeviceIdException">The alternate id is already connected to another device</exception>
		/// <returns>The device or null if there is no device with <paramref name="deviceId"/></returns>
		Task<Device> AddAlternateMacAddressAsync(int deviceId, long macAddress);

		/// <summary>
		/// Adds the alternate UUID asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="uuid">The UUID.</param>
		/// <exception cref="AlternateDeviceIdException">The alternate id is already connected to another device</exception>
		/// <returns>The device or null if there is no device with <paramref name="deviceId"/></returns>
		Task<Device> AddAlternateUuidAsync(int deviceId, Guid uuid);

		/// <summary>
		/// Adds the alternate device identifier asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns></returns>
		Task<Device> AddAlternateManufactureIdAsync(int deviceId, string manufacture, string manufactureId);

		/// <summary>
		/// Removes the alternate mac address asynchronous.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns>
		/// The device or null if the macAddress is not found
		/// </returns>
		Task<Device> RemoveAlternateMacAddressAsync(long macAddress);

		/// <summary>
		/// Removes the alternate UUID asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns>
		/// The device or null if the uuid is not found
		/// </returns>
		Task<Device> RemoveAlternateUuidAsync(Guid uuid);

		/// <summary>
		/// Removes the alternate manufacture identifier asynchronous.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns></returns>
		Task<Device> RemoveAlternateManufactureIdAsync(string manufacture, string manufactureId);

		/// <summary>
		/// Gets the alternate ids for device asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		Task<IEnumerable<AlternateDeviceId>> GetAlternateIdsForDeviceAsync(int deviceId);
	}
}
