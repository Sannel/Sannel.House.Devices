using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sannel.House.Devices.Tests.Repositories
{
	public class MockRepository : IDeviceRepository
	{

		public Func<int, Device> GetDeviceById { get; set; }
		public Task<Device> GetDeviceByIdAsync(int deviceId)
			=> Task.Run(() => GetDeviceById(deviceId));

		public Func<int, int, PagedResults<Device>> GetDeviceList { get; set; }
		public Task<PagedResults<Device>> GetDevicesListAsync(int pageIndex, int pageSize)
			=> Task.Run(() => GetDeviceList(pageIndex, pageSize));

		public Func<long, Device> GetDeviceByMacAddress { get; set; }

		public Task<Device> GetDeviceByMacAddressAsync(long macAddress)
			=> Task.Run(() => GetDeviceByMacAddress(macAddress));

		public Func<Guid, Device> GetDeviceByUuid { get; set; }

		public Task<Device> GetDeviceByUuidAsync(Guid uuid)
			=> Task.Run(() => GetDeviceByUuid(uuid));

		public Task<Device> CreateDeviceAsync(Device device)
		{
			throw new NotImplementedException();
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

		public Task<Device> AddDeviceAsync(Device device)
		{
			throw new NotImplementedException();
		}
	}
}
