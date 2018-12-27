/* Copyright 2018 Sannel Software, L.L.C.
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
      http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sannel.House.Devices.Controllers;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using Sannel.House.Devices.Tests.Repositories;
using Sannel.House.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
				() => new DevicesController(new Mock<IDeviceRepository>().Object, null));
		}
		
		[Fact]
		public async Task GetPageTest()
		{
			var repo = new Mock<IDeviceRepository>();
			using (var controller = new DevicesController(repo.Object, CreateLogger<DevicesController>()))
			{
				repo.Setup(i => i.GetDevicesListAsync(1, 25))
					.ReturnsAsync(new PagedResults<Device>()
				{
					Data = new List<Device>(),
					Page = -1,
					PageSize = -2
				});

				var result = await controller.GetPaged();
				var scr = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
				Assert.Equal(200, scr.StatusCode);
				var v = Assert.IsAssignableFrom<PagedResponseModel<Device>>(scr.Value);
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
			var repo = new Mock<IDeviceRepository>();
			using (var controller = new DevicesController(repo.Object, CreateLogger<DevicesController>()))
			{

				repo.Setup(i => i.GetDevicesListAsync(5, 25))
					.ReturnsAsync(new PagedResults<Device>()
					{
						Data = new List<Device>(),
						Page = -1,
						PageSize = -2
					});


				var result = await controller.GetPaged(-1);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				var first = em.Errors.First();
				Assert.Equal("pageIndex", first.Key);
				Assert.Equal("Page Index must be 0 or greater", first.Value.First());
				Assert.Equal("Invalid Page Index", em.Title);
				Assert.Equal(400, em.Status);

				result = await controller.GetPaged(5);
				var scr = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
				Assert.Equal(200, scr.StatusCode);

				var v = Assert.IsAssignableFrom<PagedResponseModel<Device>>(scr.Value);
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
			var repo = new Mock<IDeviceRepository>();
			using (var controller = new DevicesController(repo.Object, CreateLogger<DevicesController>()))
			{
				repo.Setup(i => i.GetDevicesListAsync(5, 3))
					.ReturnsAsync(new PagedResults<Device>()
					{
						Data = new List<Device>(),
						Page = -1,
						PageSize = -2
					});

				var result = await controller.GetPaged(-1, 3);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				var first = em.Errors.First();
				Assert.Equal("pageIndex", first.Key);
				Assert.Equal("Page Index must be 0 or greater", first.Value.First());
				Assert.Equal("Invalid Page Index", em.Title);
				Assert.Equal(400, em.Status);

				result = await controller.GetPaged(5, 1);
				bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				first = em.Errors.First();
				Assert.Equal("pageSize", first.Key);
				Assert.Equal("Page Size must be greater then 2", first.Value.First());
				Assert.Equal("Invalid Page Size", em.Title);
				Assert.Equal(400, em.Status);

				result = await controller.GetPaged(5, 3);
				var scr = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
				Assert.Equal(200, scr.StatusCode);

				var v = Assert.IsAssignableFrom<PagedResponseModel<Device>>(scr.Value);
				Assert.NotNull(v);
				Assert.NotNull(v.Data);
				Assert.Empty(v.Data);
				Assert.Equal(-1, v.Page);
				Assert.Equal(-2, v.PageSize);
			}
		}

		[Fact]
		public async Task GetDeviceByIdAsynctest()
		{
			var repo = new Mock<IDeviceRepository>();
			using (var controller = new DevicesController(repo.Object, CreateLogger<DevicesController>()))
			{


				var result = await controller.Get(-1);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				var first = em.Errors.First();
				Assert.Equal("deviceId", first.Key);
				Assert.Equal("Device Id must be greater then or equal to 0", first.Value.First());
				Assert.Equal("Invalid Device Id", em.Title);
				Assert.Equal(400, em.Status);


				repo.Setup(i => i.GetDeviceByIdAsync(0)).ReturnsAsync((Device)null);

				result = await controller.Get(0);
				var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
				em = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
				first = em.Errors.First();
				Assert.Equal("device", first.Key);
				Assert.Equal("Device Not Found", first.Value.First());
				Assert.Equal("Device Not Found", em.Title);
				Assert.Equal(404, em.Status);

				var device = new Device()
				{
					DeviceId = 1,
					Name = "Test Device",
					Description = "Test Description",
					DisplayOrder = 9,
					IsReadOnly = true,
					DateCreated = DateTime.Now
				};

				repo.Setup(i => i.GetDeviceByIdAsync(device.DeviceId))
					.ReturnsAsync(device);

				result = await controller.Get(device.DeviceId);
				var oor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
				Assert.NotNull(oor.Value);
				var actual = Assert.IsAssignableFrom<ResponseModel<Device>>(oor.Value);
				Assert.Equal(200, actual.Status);
				Assert.Equal("The Device", actual.Title);
				AssertEqual(device, actual.Data);
			}
		}

		[Fact]
		public async Task GetDeviceByAlternateIdMacAddressAsynctest()
		{
			var repo = new Mock<IDeviceRepository>();
			using (var controller = new DevicesController(repo.Object, CreateLogger<DevicesController>()))
			{


				var result = await controller.GetByAltId(-1);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				var first = em.Errors.First();
				Assert.Equal("macAddress", first.Key);
				Assert.Equal("Mac Address must be greater then or equal to 0", first.Value.First());
				Assert.Equal("Invalid Mac Address", em.Title);
				Assert.Equal(400, em.Status);

				repo.Setup(i => i.GetDeviceByMacAddressAsync(0)).ReturnsAsync((Device)null);

				result = await controller.GetByAltId(0);
				var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
				em = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
				first = em.Errors.First();
				Assert.Equal("device", first.Key);
				Assert.Equal("Device Not Found", first.Value.First());
				Assert.Equal("Device Not Found", em.Title);
				Assert.Equal(404, em.Status);

				var device = new Device()
				{
					DeviceId = 1,
					Name = "Test Device",
					Description = "Test Description",
					DisplayOrder = 9,
					IsReadOnly = true,
					DateCreated = DateTime.Now
				};

				repo.Setup(i => i.GetDeviceByMacAddressAsync(20))
					.ReturnsAsync(device);

				result = await controller.GetByAltId(20);
				var oor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
				Assert.NotNull(oor.Value);
				var actual = Assert.IsAssignableFrom<ResponseModel<Device>>(oor.Value);
				Assert.Equal("The Device", actual.Title);
				Assert.Equal(200, actual.Status);
				AssertEqual(device, actual.Data);
			}
		}

		[Fact]
		public async Task GetDeviceByAlternateIdUuidAsynctest()
		{
			var repo = new Mock<IDeviceRepository>();
			using (var controller = new DevicesController(repo.Object, CreateLogger<DevicesController>()))
			{


				var result = await controller.GetByAltId(Guid.Empty);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				var first = em.Errors.First();
				Assert.Equal("uuid", first.Key);
				Assert.Equal($"Uuid cannot be Empty {Guid.Empty}", first.Value.First());
				Assert.Equal("Invalid Uuid", em.Title);
				Assert.Equal(400, em.Status);

				var guid1 = Guid.NewGuid();

				repo.Setup(i => i.GetDeviceByUuidAsync(guid1)).ReturnsAsync((Device)null);

				result = await controller.GetByAltId(guid1);
				var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
				em = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
				first = em.Errors.First();
				Assert.Equal("device", first.Key);
				Assert.Equal("Device Not Found", first.Value.First());
				Assert.Equal("Device Not Found", em.Title);
				Assert.Equal(404, em.Status);

				var device = new Device()
				{
					DeviceId = 1,
					Name = "Test Device",
					Description = "Test Description",
					DisplayOrder = 9,
					IsReadOnly = true,
					DateCreated = DateTime.Now
				};

				guid1 = Guid.NewGuid();

				repo.Setup(i => i.GetDeviceByUuidAsync(guid1))
					.ReturnsAsync(device);

				result = await controller.GetByAltId(guid1);
				var oor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
				Assert.NotNull(oor.Value);
				var actual = Assert.IsAssignableFrom<ResponseModel<Device>>(oor.Value);
				Assert.Equal(200, actual.Status);
				Assert.Equal("The Device", actual.Title);
				AssertEqual(device, actual.Data);
			}
		}

		[Fact]
		public async Task PostTest()
		{
			var repo = new Mock<IDeviceRepository>();
			using (var controller = new DevicesController(repo.Object, CreateLogger<DevicesController>()))
			{

				controller.ViewData.ModelState.Clear();

				var result = await controller.Post(null);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				var first = em.Errors.First();
				Assert.Equal("device", first.Key);
				Assert.Equal("No device info provided", first.Value.First());
				Assert.Equal("Invalid Device Object", em.Title);
				Assert.Equal(400, em.Status);

				var device = new Device()
				{
					DeviceId = 200,
					Name = "Test Name",
					Description = "20",
					DisplayOrder = 22,
					IsReadOnly = true,
					DateCreated = DateTime.Now
				};

				controller.ViewData.ModelState.Clear();
				controller.ModelState.AddModelError("validationError", "Error Validating");
				result = await controller.Post(device);
				bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				first = em.Errors.First();
				Assert.Equal("validationError", first.Key);
				Assert.Equal("Error Validating", first.Value.First());
				Assert.Equal("Invalid Model", em.Title);
				Assert.Equal(400, em.Status);

				repo.Setup(i => i.AddDeviceAsync(device)).ReturnsAsync(device);

				controller.ModelState.Clear();
				result = await controller.Post(device);
				var okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
				var d = Assert.IsAssignableFrom<ResponseModel<int>>(okor.Value);
				Assert.Equal(200, d.Status);
				Assert.Equal("The Device Id", d.Title);
				Assert.Equal(device.DeviceId, d.Data);
			}
		}

		[Fact]
		public async Task PutTest()
		{
			var repo = new Mock<IDeviceRepository>();
			using (var controller = new DevicesController(repo.Object, CreateLogger<DevicesController>()))
			{

				controller.ViewData.ModelState.Clear();

				var result = await controller.Put(null);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				var first = em.Errors.First();
				Assert.Equal("device", first.Key);
				Assert.Equal("No device info provided", first.Value.First());
				Assert.Equal("Invalid Device Object", em.Title);
				Assert.Equal(400, em.Status);

				var device = new Device()
				{
					DeviceId = -1,
					Name = "Test Name",
					Description = "20",
					DisplayOrder = 22,
					IsReadOnly = true,
					DateCreated = DateTime.Now
				};

				controller.ViewData.ModelState.Clear();
				result = await controller.Put(device);
				bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				first = em.Errors.First();
				Assert.Equal("DeviceId", first.Key);
				Assert.Equal("Device Id must be 0 or greater", first.Value.First());
				Assert.Equal("Invalid Device Id", em.Title);
				Assert.Equal(400, em.Status);

				device.DeviceId = 200;


				controller.ViewData.ModelState.Clear();
				controller.ModelState.AddModelError("validationError", "Error Validating");
				result = await controller.Put(device);
				bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				first = em.Errors.First();
				Assert.Equal("validationError", first.Key);
				Assert.Equal("Error Validating", first.Value.First());
				Assert.Equal("Invalid Device Object", em.Title);
				Assert.Equal(400, em.Status);

				repo.Setup(i => i.UpdateDeviceAsync(device)).ThrowsAsync(new ReadOnlyException($"Device {device.DeviceId} is Read Only"));

				controller.ModelState.Clear();

				result = await controller.Put(device);
				bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				em = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.Single(em.Errors);
				first = em.Errors.First();
				Assert.Equal("readonly", first.Key);
				Assert.Equal("Device is read-only and cannot be updated.", first.Value.First());
				Assert.Equal("Device is Read-only", em.Title);
				Assert.Equal(400, em.Status);

				repo.Setup(i => i.UpdateDeviceAsync(device)).ReturnsAsync((Device)null);


				result = await controller.Put(device);
				var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
				em = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
				Assert.Single(em.Errors);
				first = em.Errors.First();
				Assert.Equal("notfound", first.Key);
				Assert.Equal("Device not found to update", first.Value.First());
				Assert.Equal("Device Not Found", em.Title);
				Assert.Equal(404, em.Status);

				repo.Setup(i => i.UpdateDeviceAsync(device)).ReturnsAsync(device);

				result = await controller.Put(device);
				var okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
				var d = Assert.IsAssignableFrom<ResponseModel<int>>(okor.Value);
				Assert.Equal(200, d.Status);
				Assert.Equal("Device Id", d.Title);
				Assert.Equal(device.DeviceId, d.Data);
			}
		}
	}
}
