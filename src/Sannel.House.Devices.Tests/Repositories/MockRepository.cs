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
		public Func<long, int, PagedResults<Device>> GetDeviceList { get; set; }

		public Task<PagedResults<Device>> GetDevicesListAsync(long pageIndex, int pageSize)
			=> Task.Run(() => GetDeviceList(pageIndex, pageSize));


	}
}
