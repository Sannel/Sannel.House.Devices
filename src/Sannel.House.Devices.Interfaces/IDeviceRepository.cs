/* Copyright 2019-2020 Sannel Software, L.L.C.
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
using System.Threading.Tasks;
using Sannel.House.Base.Models;
using System.Diagnostics.CodeAnalysis;

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
		Task<PagedResponseModel<Device>> GetDevicesListAsync(int pageIndex, int pageSize);

		/// <summary>
		/// Gets the device by identifier asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		Task<Device?> GetDeviceByIdAsync(int deviceId);

		/// <summary>
		/// Gets the device by mac address asynchronous.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns>
		/// The Device or null if no device is found with the passed <paramref name="macAddress"/>
		/// </returns>
		Task<Device?> GetDeviceByMacAddressAsync(long macAddress);

		/// <summary>
		/// Gets the device by UUID/Guid asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID/Guid.</param>
		/// <returns>
		/// The Device or null if no device is found with the passed <paramref name="uuid"/>
		/// </returns>
		Task<Device?> GetDeviceByUuidAsync(Guid uuid);

		/// <summary>
		/// Gets the device by manufacture identifier asynchronous.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns>
		/// The Device or null if no device is found with the passed <paramref name="manufacture"/> <paramref name="manufactureId"/> combination
		/// </returns>
		Task<Device?> GetDeviceByManufactureIdAsync([NotNull]string manufacture, [NotNull]string manufactureId);

		/// <summary>
		/// Creates the device asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		[return: NotNull]
		Task<Device> AddDeviceAsync([NotNull]Device device);

		/// <summary>
		/// Updates the device asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns>
		/// The device thats update or null if the passed device is not found in the database
		/// </returns>
		/// <exception cref="ReadOnlyException">The device is marked as read only and cannot be updated</exception>
		Task<Device?> UpdateDeviceAsync([NotNull]Device device);

		/// <summary>
		/// Adds the alternate mac address asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="macAddress">The mac address.</param>
		/// <returns>
		/// The device or null if there is no device with <paramref name="deviceId" /> is found
		/// </returns>
		/// <exception cref="AlternateDeviceIdException">The alternate id is already connected to another device</exception>
		Task<Device?> AddAlternateMacAddressAsync(int deviceId, long macAddress);

		/// <summary>
		/// Adds the alternate UUID asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="uuid">The UUID.</param>
		/// <returns>
		/// The device or null if there is no device with <paramref name="deviceId" /> is found
		/// </returns>
		/// <exception cref="AlternateDeviceIdException">The alternate id is already connected to another device</exception>
		Task<Device?> AddAlternateUuidAsync(int deviceId, Guid uuid);

		/// <summary>
		/// Adds the alternate device identifier asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns>
		/// The device or null if there is no device with <paramref name="deviceId"/> is found
		/// </returns>
		/// <exception cref="AlternateDeviceIdException">The alternate id is already connected to another device</exception>
		Task<Device?> AddAlternateManufactureIdAsync(int deviceId, [NotNull]string manufacture, [NotNull]string manufactureId);

		/// <summary>
		/// Removes the alternate mac address asynchronous.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns>
		/// The device associated with <paramref name="macAddress"/> or null if the <paramref name="macAddress"/> is not found
		/// </returns>
		Task<Device?> RemoveAlternateMacAddressAsync(long macAddress);

		/// <summary>
		/// Removes the alternate UUID asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns>
		/// The device associated with the <paramref name="uuid"/> or null if the <paramref name="uuid"/> is not found
		/// </returns>
		Task<Device?> RemoveAlternateUuidAsync(Guid uuid);

		/// <summary>
		/// Removes the alternate manufacture identifier asynchronous.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns>
		/// The device associated with the <paramref name="manufacture"/> <paramref name="manufactureId"/> combo or null if its not found
		/// </returns>
		Task<Device?> RemoveAlternateManufactureIdAsync([NotNull]string manufacture, [NotNull]string manufactureId);

		/// <summary>
		/// Gets the alternate ids for device asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns>
		/// The AlternateDeviceIds or empty if <paramref name="deviceId"/> is not found
		/// </returns>
		Task<IEnumerable<AlternateDeviceId>> GetAlternateIdsForDeviceAsync(int deviceId);
	}
}
