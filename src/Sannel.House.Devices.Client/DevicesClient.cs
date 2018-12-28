using Newtonsoft.Json;
using Sannel.House.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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
		public Task<Results<Device>> GetAsync(int deviceId)
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
	}
}
