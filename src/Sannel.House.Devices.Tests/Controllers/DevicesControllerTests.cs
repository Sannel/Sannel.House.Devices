using Sannel.House.Devices.Controllers;
using Sannel.House.Devices.Models;
using Sannel.House.Devices.Tests.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sannel.House.Devices.Tests.Controllers
{
	public class DevicesControllerTests : BaseTests
	{
		[Fact]
		public void ConstructorArgumentsTest()
		{
			Assert.Throws<ArgumentNullException>("repository",
				() => new DevicesController(null, null));
			Assert.Throws<ArgumentNullException>("logger",
				() => new DevicesController(new MockRepository(), null));
		}
		
		[Fact]
		public async Task GetDevicesTest()
		{
			var repo = new MockRepository();
			using (var controller = new DevicesController(repo, CreateLogger<DevicesController>()))
			{
				var called = false;
				repo.GetDeviceList = (index, size) =>
				{
					called = true;
					Assert.Equal(0, index);
					Assert.Equal(25, size);

					return new Models.PagedResults<Device>()
					{
						Data = new List<Device>(),
						Page = -1,
						PageSize = -2
					};
				};

				var result = await controller.Get();
				Assert.True(called, "Repository Not called");
				Assert.NotNull(result.Data);
				Assert.Empty(result.Data);
				Assert.Equal(-1, result.Page);
				Assert.Equal(-2, result.PageSize);
			}


		}
	}
}
