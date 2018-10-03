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
		public Func<int, int, PagedResults<Device>> GetDeviceList { get; set; }

		public Task<PagedResults<Device>> GetDevicesListAsync(int pageIndex, int pageSize)
			=> Task.Run(() => GetDeviceList(pageIndex, pageSize));


	}
}
