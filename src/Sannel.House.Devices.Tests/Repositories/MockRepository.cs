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
	}
}
