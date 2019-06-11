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
using Newtonsoft.Json;
using Sannel.House.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security;
using Microsoft.Extensions.Logging;

namespace Sannel.House.Devices.Client
{
	public class DevicesClient : ClientBase
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="DevicesClient" /> class.
		/// </summary>
		/// <param name="factory">The Http Client factory.</param>
		/// <param name="baseUrl">The base URL i.e. http://gateway or http://gateway:8080</param>
		/// <param name="logger">The logger.</param>
		public DevicesClient(IHttpClientFactory factory, Uri baseUrl, ILogger logger) 
			: base(factory, 
				$"{baseUrl.Scheme}://{baseUrl.Host}:{baseUrl.Port}/api/v1/", 
				logger)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="DevicesClient" /> class.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="baseUrl">The base URL i.e. http://gateway or http://gateway:8080</param>
		/// <param name="logger">The logger.</param>
		public DevicesClient(HttpClient client, Uri baseUrl, ILogger logger) 
			: base(client, 
				$"{baseUrl.Scheme}://{baseUrl.Host}:{baseUrl.Port}/api/v1/", 
				logger)
		{ }

		/// <summary>Gets the client.</summary>
		/// <returns></returns>
		protected override HttpClient GetClient()
			=> client ?? factory.CreateClient(nameof(DevicesClient));

		/// <summary>
		/// Gets a paged list of Devices asynchronous.
		/// </summary>
		/// <returns></returns>
		public virtual Task<PagedResults<Device>> GetPagedAsync()
			=> GetPagedAsync(0, 25);

		/// <summary>
		/// Gets a paged list of Devices asynchronous.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <returns></returns>
		public virtual Task<PagedResults<Device>> GetPagedAsync(long page)
			=> GetPagedAsync(page, 25);

		/// <summary>
		/// Gets a paged list of Devices asynchronous.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <returns></returns>
		public virtual async Task<PagedResults<Device>> GetPagedAsync(long page, int pageSize)
		{ 
			var client = factory.CreateClient(nameof(DevicesClient));
			try
			{
				using (var message = new HttpRequestMessage(HttpMethod.Get, $"Devices/GetPaged/{page}/{pageSize}"))
				{
					AddAuthorizationHeader(message);

					if(logger.IsEnabled(LogLevel.Debug))
					{
						logger.LogDebug("RequestUri: {0}", message.RequestUri);
						logger.LogDebug("AuthHeader: {0}", message.Headers.Authorization);
					}
					var response = await client.SendAsync(message);
					var obj = await DeserializeIfSupportedCodeAsync<PagedResults<Device>>(response);
					return obj;
				}
			}
			catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
			{
				return new PagedResults<Device>()
				{
					Status = 444,
					Title = "Exception",
					Success = false,
					Exception = ex
				};
			}
		}

		/// <summary>
		/// Gets the device by the provided device identifier.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public virtual Task<Results<Device>> GetDeviceAsync(int deviceId)
			=> GetAsync($"Devices/{deviceId}");

		/// <summary>
		/// Does a get call to the provided url
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		protected virtual async Task<Results<Device>> GetAsync(string url)
		{
			var client = factory.CreateClient(nameof(DevicesClient));
			try
			{
				using (var message = new HttpRequestMessage(HttpMethod.Get, url))
				{
					AddAuthorizationHeader(message);
					if(logger.IsEnabled(LogLevel.Debug))
					{
						logger.LogDebug("RequestUri: {0}", message.RequestUri);
						logger.LogDebug("AuthHeader: {0}", message.Headers.Authorization);
					}
					var response = await client.SendAsync(message);
					return await DeserializeIfSupportedCodeAsync<Results<Device>>(response);
				}
			}
			catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
			{
				return new Results<Device>()
				{
					Status = 444,
					Title = "Exception",
					Success = false,
					Exception = ex
				};
			}
		}

		/// <summary>
		/// Gets a device by its registered mac address asynchronous.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		public virtual Task<Results<Device>> GetByMacAddressAsync(long macAddress)
			=> GetAsync($"Devices/GetByMac/{macAddress}");

		/// <summary>
		/// Gets a device by its registered UUID asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns></returns>
		public virtual Task<Results<Device>> GetByUuidAsync(Guid uuid)
			=> GetAsync($"Devices/GetByUuid/{uuid}");

		/// <summary>
		/// Gets the by manufacture identifier which is <paramref name="manufacture"/>+<paramref name="manufactureId"/> asynchronous.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns></returns>
		public virtual Task<Results<Device>> GetByManufactureIdAsync(string manufacture, string manufactureId)
			=> GetAsync($"Devices/GetByManufactureId/{Uri.EscapeUriString(manufacture)}/{Uri.EscapeUriString(manufactureId)}");

		/// <summary>
		/// Adds the device asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		public virtual Task<Results<int>> AddDeviceAsync(Device device)
			=> PostAsync<Results<int>>("Devices", device);

		/// <summary>
		/// Updates the device.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		public virtual Task<Results<int>> UpdateDeviceAsync(Device device)
			=> PutAsync<Results<int>>("Devices", device);

		/// <summary>
		/// Gets the alternate ids asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public virtual Task<Results<List<AlternateDeviceId>>> GetAlernateIdsAsync(int deviceId)
			=> GetAsync<Results<List<AlternateDeviceId>>>($"AlternateId/{deviceId}");

		/// <summary>
		/// Adds the alternate identifier asynchronous.
		/// The alternateId is a mac address
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		/// <remarks>
		/// You can pass in a 6 byte array that represents your mac address this method will 
		/// fix the array depending on if this system is Little or Big Endian and convert it
		/// to a long.
		/// 
		/// You can also pass in an 8 byte array but this method will only try to convert it
		/// to a long not deal with Little or Big Endian conversion of the array.
		/// </remarks>
		/// <exception cref="ArgumentNullException">macAddress</exception>
		/// <exception cref="ArgumentOutOfRangeException">macAddress - macAddress must be 6 or 8 bytes long. Also if the array of bytes cannot be converted to a long</exception>
		public virtual Task<Results<Device>> AddAlternateIdAsync(byte[] macAddress, int deviceId)
		{
			var fixedMacAddress = new byte[8];
			if(macAddress == null)
			{
				throw new ArgumentNullException(nameof(macAddress));
			}
			if(macAddress.Length == 6)
			{
				Array.Clear(fixedMacAddress, 0, fixedMacAddress.Length);
				Array.Copy(macAddress, 0, fixedMacAddress, 2, macAddress.Length);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(fixedMacAddress);
				}
			}
			else if(macAddress.Length == 8)
			{
				Array.Copy(macAddress, fixedMacAddress, macAddress.Length);
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(macAddress), "macAddress must be 6 or 8 bytes long");
			}

			return AddAlternateIdAsync(BitConverter.ToInt64(fixedMacAddress, 0), deviceId);
		}

		/// <summary>
		/// Adds the alternate identifier asynchronous.
		/// The alternateId is a mac address
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public virtual Task<Results<Device>> AddAlternateIdAsync(long macAddress, int deviceId)
			=> PostAsync<Results<Device>>($"AlternateId/mac/{macAddress}/{deviceId}",new object() { });

		/// <summary>
		/// Adds the alternate identifier asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public virtual Task<Results<Device>> AddAlternateIdAsync(Guid uuid, int deviceId)
			=> PostAsync<Results<Device>>($"AlternateId/uuid/{uuid}/{deviceId}",new object() { });

		/// <summary>
		/// Adds the manufacture alternate identifier asynchronous.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public virtual Task<Results<Device>> AddAlternateIdAsync(string manufacture, string manufactureId, int deviceId)
			=> PostAsync<Results<Device>>($"AlternateId/manufactureid/{Uri.EscapeUriString(manufacture)}/{Uri.EscapeUriString(manufactureId)}/{deviceId}", new object() { });

		/// <summary>
		/// Deletes the alternate identifier asynchronous.
		/// The alternateId is a mac address
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		/// <remarks>
		/// You can pass in a 6 byte array that represents your mac address this method will 
		/// fix the array depending on if this system is Little or Big Endian and convert it
		/// to a long.
		/// 
		/// You can also pass in an 8 byte array but this method will only try to convert it
		/// to a long not deal with Little or Big Endian conversion of the array.
		/// </remarks>
		/// <exception cref="ArgumentNullException">macAddress</exception>
		/// <exception cref="ArgumentOutOfRangeException">macAddress - macAddress must be 6 or 8 bytes long. Also if the array of bytes cannot be converted to a long</exception>
		public virtual Task<Results<Device>> DeleteAlternateIdAsync(byte[] macAddress)
		{
			var fixedMacAddress = new byte[8];
			if(macAddress == null)
			{
				throw new ArgumentNullException(nameof(macAddress));
			}
			if(macAddress.Length == 6)
			{
				Array.Clear(fixedMacAddress, 0, fixedMacAddress.Length);
				Array.Copy(macAddress, 0, fixedMacAddress, 2, macAddress.Length);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(fixedMacAddress);
				}
			}
			else if(macAddress.Length == 8)
			{
				Array.Copy(macAddress, fixedMacAddress, macAddress.Length);
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(macAddress), "macAddress must be 6 or 8 bytes long");
			}

			return DeleteAlternateIdAsync(BitConverter.ToInt64(fixedMacAddress, 0));
		}

		/// <summary>
		/// Deletes the alternate identifier asynchronous.
		/// The alternateId is a mac address
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		public virtual Task<Results<Device>> DeleteAlternateIdAsync(long macAddress)
			=> DeleteAsync<Results<Device>>($"AlternateId/mac/{macAddress}");

		/// <summary>
		/// Deletes the Uuid alternate identifier asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns></returns>
		public virtual Task<Results<Device>> DeleteAlternateIdAsync(Guid uuid)
			=> DeleteAsync<Results<Device>>($"AlternateId/uuid/{uuid}");

		/// <summary>
		/// Deletes the manufacture alternate identifier asynchronous.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns></returns>
		public virtual Task<Results<Device>> DeleteAlternateIdAsync(string manufacture, string manufactureId)
			=> DeleteAsync<Results<Device>>($"AlternateId/manufacture/{Uri.EscapeUriString(manufacture)}/{Uri.EscapeUriString(manufactureId)}");
	}
}
