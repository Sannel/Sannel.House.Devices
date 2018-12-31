using Newtonsoft.Json;
using Sannel.House.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sannel.House.Devices.Client
{
	public class DevicesClient
	{
		private readonly IHttpClientFactory factory;

		public event EventHandler<AuthenticationTokenArgs> GetAuthenticationToken;

		/// <summary>
		/// Initializes a new instance of the <see cref="DevicesClient"/> class.
		/// </summary>
		/// <param name="factory">The Http Client factory.</param>
		public DevicesClient(IHttpClientFactory factory) 
			=> this.factory = factory;

		protected void UpdateAuthenticationToken(HttpClient client)
		{
			var args = new AuthenticationTokenArgs();
			GetAuthenticationToken?.Invoke(this, args);

			client.DefaultRequestHeaders.Authorization 
				= new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", args.Token);
		}

		/// <summary>
		/// Gets a paged list of Devices asynchronous.
		/// </summary>
		/// <returns></returns>
		public Task<PagedResults<Device>> GetPagedAsync()
			=> GetPagedAsync(0, 25);

		/// <summary>
		/// Gets a paged list of Devices asynchronous.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <returns></returns>
		public Task<PagedResults<Device>> GetPagedAsync(long page)
			=> GetPagedAsync(page, 25);

		/// <summary>
		/// Gets a paged list of Devices asynchronous.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <returns></returns>
		public async Task<PagedResults<Device>> GetPagedAsync(long page, int pageSize)
		{ 
			var client = factory.CreateClient(nameof(DevicesClient));
			UpdateAuthenticationToken(client);
			try
			{
				var response = await client.GetAsync($"Devices/GetPaged/{page}/{pageSize}");
				var data = await response.Content.ReadAsStringAsync();
				var obj = await Task.Run(() => JsonConvert.DeserializeObject<PagedResults<Device>>(data));
				obj.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
				return obj;
			}
			catch(Exception ex)
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
		public Task<Results<Device>> GetDeviceAsync(int deviceId)
			=> GetAsync($"Devices/{deviceId}");

		/// <summary>
		/// Does a get call to the provided url
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		protected async Task<Results<Device>> GetAsync(string url)
		{
			var client = factory.CreateClient(nameof(DevicesClient));
			UpdateAuthenticationToken(client);
			try
			{
				var response = await client.GetAsync(url);
				var data = await response.Content.ReadAsStringAsync();
				var obj = await Task.Run(() => JsonConvert.DeserializeObject<Results<Device>>(data));
				obj.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
				return obj;
			}
			catch(Exception ex)
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
		public Task<Results<Device>> GetByMacAddressAsync(long macAddress)
			=> GetAsync($"Devices/GetByMac/{macAddress}");

		/// <summary>
		/// Gets a device by its registered UUID asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns></returns>
		public Task<Results<Device>> GetByUuidAsync(Guid uuid)
			=> GetAsync($"Devices/GetByUuid/{uuid}");

		/// <summary>
		/// Adds the device asynchronous.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		public async Task<Results<int>> AddDeviceAsync(Device device)
		{
			var client = factory.CreateClient(nameof(DevicesClient));
			UpdateAuthenticationToken(client);
			try
			{
				var response = await client.PostAsync($"Devices",
					new StringContent(
						await Task.Run(() =>JsonConvert.SerializeObject(device)),
						System.Text.Encoding.UTF8,
						"application/json"));
				var data = await response.Content.ReadAsStringAsync();
				var obj = await Task.Run(() => JsonConvert.DeserializeObject<Results<int>>(data));
				obj.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
				return obj;
			}
			catch(Exception ex)
			{
				return new Results<int>()
				{
					Status = 444,
					Title = "Exception",
					Success = false,
					Exception = ex
				};
			}
		}

		/// <summary>
		/// Updates the device.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		public async Task<Results<int>> UpdateDeviceAsync(Device device)
		{
			var client = factory.CreateClient(nameof(DevicesClient));
			UpdateAuthenticationToken(client);
			try
			{
				var response = await client.PutAsync($"Devices",
					new StringContent(
						await Task.Run(() =>JsonConvert.SerializeObject(device)),
						System.Text.Encoding.UTF8,
						"application/json"));
				var data = await response.Content.ReadAsStringAsync();
				var obj = await Task.Run(() => JsonConvert.DeserializeObject<Results<int>>(data));
				obj.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
				return obj;
			}
			catch(Exception ex)
			{
				return new Results<int>()
				{
					Status = 444,
					Title = "Exception",
					Success = false,
					Exception = ex
				};
			}
		}

		/// <summary>
		/// Gets the alternate ids asynchronous.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public async Task<Results<List<AlternateDeviceId>>> GetAlernateIdsAsync(int deviceId)
		{ 
			var client = factory.CreateClient(nameof(DevicesClient));
			UpdateAuthenticationToken(client);
			try
			{
				var response = await client.GetAsync($"AlternateId/{deviceId}");
				var data = await response.Content.ReadAsStringAsync();
				var obj = await Task.Run(() => JsonConvert.DeserializeObject<Results<List<AlternateDeviceId>>>(data));
				obj.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
				return obj;
			}
			catch(Exception ex)
			{
				return new Results<List<AlternateDeviceId>>()
				{
					Status = 444,
					Title = "Exception",
					Success = false,
					Exception = ex
				};
			}
		}

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
		public Task<Results<Device>> AddAlternateIdAsync(byte[] macAddress, int deviceId)
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
		public async Task<Results<Device>> AddAlternateIdAsync(long macAddress, int deviceId)
		{
			var client = factory.CreateClient(nameof(DevicesClient));
			UpdateAuthenticationToken(client);
			try
			{
				var response = await client.PostAsync($"AlternateId/mac/{macAddress}/{deviceId}", new StringContent(string.Empty));
				var data = await response.Content.ReadAsStringAsync();
				var obj = await Task.Run(() => JsonConvert.DeserializeObject<Results<Device>>(data));
				obj.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
				return obj;
			}
			catch(Exception ex)
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
		/// Adds the alternate identifier asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		public async Task<Results<Device>> AddAlternateIdAsync(Guid uuid, int deviceId)
		{
			var client = factory.CreateClient(nameof(DevicesClient));
			UpdateAuthenticationToken(client);
			try
			{
				var response = await client.PostAsync($"AlternateId/uuid/{uuid}/{deviceId}", new StringContent(string.Empty));
				var data = await response.Content.ReadAsStringAsync();
				var obj = await Task.Run(() => JsonConvert.DeserializeObject<Results<Device>>(data));
				obj.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
				return obj;
			}
			catch(Exception ex)
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
		public Task<Results<Device>> DeleteAlternateIdAsync(byte[] macAddress)
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
		public async Task<Results<Device>> DeleteAlternateIdAsync(long macAddress)
		{
			var client = factory.CreateClient(nameof(DevicesClient));
			UpdateAuthenticationToken(client);
			try
			{
				var response = await client.DeleteAsync($"AlternateId/mac/{macAddress}");
				var data = await response.Content.ReadAsStringAsync();
				var obj = await Task.Run(() => JsonConvert.DeserializeObject<Results<Device>>(data));
				obj.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
				return obj;
			}
			catch(Exception ex)
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
		/// Deletes the alternate identifier asynchronous.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns></returns>
		public async Task<Results<Device>> DeleteAlternateIdAsync(Guid uuid)
		{
			var client = factory.CreateClient(nameof(DevicesClient));
			UpdateAuthenticationToken(client);
			try
			{
				var response = await client.DeleteAsync($"AlternateId/uuid/{uuid}");
				var data = await response.Content.ReadAsStringAsync();
				var obj = await Task.Run(() => JsonConvert.DeserializeObject<Results<Device>>(data));
				obj.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
				return obj;
			}
			catch(Exception ex)
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
	}
}
