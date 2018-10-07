using Microsoft.AspNetCore.Mvc;
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
		public async Task GetPageTest()
		{
			var repo = new MockRepository();
			using (var controller = new DevicesController(repo, CreateLogger<DevicesController>()))
			{
				var called = false;
				repo.GetDeviceList = (index, size) =>
				{
					called = true;
					Assert.Equal(1, index);
					Assert.Equal(25, size);

					return new Models.PagedResults<Device>()
					{
						Data = new List<Device>(),
						Page = -1,
						PageSize = -2
					};
				};

				var result = await controller.GetPaged();
				Assert.True(called, "Repository Not called");
				Assert.IsType<OkObjectResult>(result.Result);
				var scr = (ObjectResult)result.Result;
				Assert.Equal(200, scr.StatusCode);

				Assert.IsType<PagedResults<Device>>(scr.Value);
				var v = (PagedResults<Device>)scr.Value;
				Assert.NotNull(v);
				Assert.NotNull(v.Data);
				Assert.Empty(v.Data);
				Assert.Equal(-1, v.Page);
				Assert.Equal(-2, v.PageSize);
			}
		}

		[Fact]
		public async Task GetPagedTest2()
		{
			var repo = new MockRepository();
			using (var controller = new DevicesController(repo, CreateLogger<DevicesController>()))
			{
				var called = false;
				repo.GetDeviceList = (index, size) =>
				{
					called = true;
					Assert.Equal(5, index);
					Assert.Equal(25, size);

					return new Models.PagedResults<Device>()
					{
						Data = new List<Device>(),
						Page = -1,
						PageSize = -2
					};
				};

				var result = await controller.GetPaged(0);
				Assert.IsType<BadRequestObjectResult>(result.Result);
				var bror = (BadRequestObjectResult)result.Result;
				Assert.Equal("Page Index must be 1 or greater", bror.Value);

				called = false;
				result = await controller.GetPaged(5);
				Assert.True(called, "Repository Not called");
				Assert.IsType<OkObjectResult>(result.Result);
				var scr = (ObjectResult)result.Result;
				Assert.Equal(200, scr.StatusCode);

				Assert.IsType<PagedResults<Device>>(scr.Value);
				var v = (PagedResults<Device>)scr.Value;
				Assert.NotNull(v);
				Assert.NotNull(v.Data);
				Assert.Empty(v.Data);
				Assert.Equal(-1, v.Page);
				Assert.Equal(-2, v.PageSize);
			}
		}

		[Fact]
		public async Task GetPagedTest3()
		{
			var repo = new MockRepository();
			using (var controller = new DevicesController(repo, CreateLogger<DevicesController>()))
			{
				var called = false;
				repo.GetDeviceList = (index, size) =>
				{
					called = true;
					Assert.Equal(5, index);
					Assert.Equal(3, size);

					return new Models.PagedResults<Device>()
					{
						Data = new List<Device>(),
						Page = -1,
						PageSize = -2
					};
				};

				var result = await controller.GetPaged(0, 3);
				Assert.IsType<BadRequestObjectResult>(result.Result);
				var bror = (BadRequestObjectResult)result.Result;
				Assert.Equal("Page Index must be 1 or greater", bror.Value);

				result = await controller.GetPaged(5, 1);
				Assert.IsType<BadRequestObjectResult>(result.Result);
				bror = (BadRequestObjectResult)result.Result;
				Assert.Equal("Page Size must be greater then 2", bror.Value);

				called = false;
				result = await controller.GetPaged(5, 3);
				Assert.True(called, "Repository Not called");
				Assert.IsType<OkObjectResult>(result.Result);
				var scr = (ObjectResult)result.Result;
				Assert.Equal(200, scr.StatusCode);

				Assert.IsType<PagedResults<Device>>(scr.Value);
				var v = (PagedResults<Device>)scr.Value;
				Assert.NotNull(v);
				Assert.NotNull(v.Data);
				Assert.Empty(v.Data);
				Assert.Equal(-1, v.Page);
				Assert.Equal(-2, v.PageSize);
			}
		}
	}
}
