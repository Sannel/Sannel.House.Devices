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
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sannel.House.Base.Models;
using Sannel.House.Devices.Controllers;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sannel.House.Devices.Tests.Controllers
{
	public class AlternateIdControllerTest : BaseTests
	{
		[Fact]
		public void ConstructorArgumentsTest()
		{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			Assert.Throws<ArgumentNullException>("service",
				() => new AlternateIdController(null, null));
			Assert.Throws<ArgumentNullException>("logger",
				() => new AlternateIdController(new Mock<IDeviceService>().Object, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
		}

		[Fact]
		public async Task GetAlternateIdsTest()
		{
			var repo = new Mock<IDeviceService>();
			using var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>());
			var result = await controller.Get(-1);
			var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			var err = error.Errors.First();
			Assert.Equal("deviceId", err.Key);
			Assert.Equal(HttpStatusCode.BadRequest, error.StatusCode);
			Assert.Equal("Device Id must be greater then or equal to 0", err.Value.First());

			repo.Setup(i => i.GetAlternateIdsForDeviceAsync(1)).ReturnsAsync(() => new List<AlternateDeviceId>());

			result = await controller.Get(1);
			var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			Assert.Equal(HttpStatusCode.NotFound, error.StatusCode);
			err = error.Errors.First();
			Assert.Equal("device", err.Key);
			Assert.Equal("Device not found", err.Value.First());

			repo.Setup(i => i.GetAlternateIdsForDeviceAsync(1)).ReturnsAsync(() =>
				new List<AlternateDeviceId>()
				{

				}
			);

			result = await controller.Get(1);
			nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
			var deviceResponse =
				Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);

			Assert.NotNull(deviceResponse);

			var altId1 = new AlternateDeviceId()
			{
				DeviceId = 2,
				Uuid = Guid.NewGuid(),
				MacAddress = 2,
				DateCreated = DateTime.UtcNow
			};
			var altId2 = new AlternateDeviceId
			{
				DeviceId = 2,
				Uuid = Guid.NewGuid(),
				MacAddress = 3,
				DateCreated = DateTime.UtcNow
			};

			repo.Setup(i => i.GetAlternateIdsForDeviceAsync(2)).ReturnsAsync(() =>
				new List<AlternateDeviceId>()
				{
						altId1,
						altId2
				}
			);

			result = await controller.Get(2);
			var okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
			var devices =
				Assert.IsAssignableFrom<ResponseModel<IEnumerable<AlternateDeviceId>>>(okor.Value);

			Assert.NotNull(devices);
			Assert.Equal(2, devices.Data.Count());
			var alt = devices.Data.ElementAt(0);
			Assert.Equal(altId1.DeviceId, alt.DeviceId);
			Assert.Equal(altId1.Uuid, alt.Uuid);
			Assert.Equal(altId1.MacAddress, alt.MacAddress);
			Assert.Equal(altId1.DateCreated, alt.DateCreated);
			alt = devices.Data.ElementAt(1);
			Assert.Equal(altId2.DeviceId, alt.DeviceId);
			Assert.Equal(altId2.Uuid, alt.Uuid);
			Assert.Equal(altId2.MacAddress, alt.MacAddress);
			Assert.Equal(altId2.DateCreated, alt.DateCreated);
		}

		[Fact]
		public async Task PostMacAddressTest()
		{

			var repo = new Mock<IDeviceService>();
			using var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>());
			var result = await controller.Post(-1, -1);
			var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			var err = error.Errors.First();
			Assert.Equal("macAddress", err.Key);
			Assert.Equal("Invalid MacAddress it must be greater then 0", err.Value.First());

			result = await controller.Post(0xD5A6E4539A1F, -1);
			bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("deviceId", err.Key);
			Assert.Equal("Device Id must be greater then 0", err.Value.First());

			repo.Setup(i => i.AddAlternateMacAddressAsync(20, 0xD5A6E4539A1F)).ThrowsAsync(new AlternateDeviceIdException("Device already connected"));


			result = await controller.Post(0xD5A6E4539A1F, 20);
			bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("macAddress", err.Key);
			Assert.Equal("That mac address is already connected to a device", err.Value.First());

			repo.Setup(i => i.AddAlternateMacAddressAsync(20, 0xD5A6E4539A1F)).ReturnsAsync((Device?)null);

			result = await controller.Post(0xD5A6E4539A1F, 20);
			var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("notFound", err.Key);
			Assert.Equal("No device found with that id", err.Value.First());

			var device1 = new Device()
			{
				DeviceId = 20,
				Name = "Test Name",
				IsReadOnly = true,
				Description = "Dest Description",
				DateCreated = DateTime.UtcNow,
				DisplayOrder = 2
			};

			repo.Setup(i => i.AddAlternateMacAddressAsync(20, 0xD5A6E4539A1F)).ReturnsAsync(device1);

			result = await controller.Post(0xD5A6E4539A1F, 20);
			var okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
			var device = Assert.IsAssignableFrom<ResponseModel<Device>>(okor.Value);
			AssertEqual(device1, device.Data);
		}

		[Fact]
		public async Task PostUuidTest()
		{

			var repo = new Mock<IDeviceService>();
			using var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>());
			var result = await controller.Post(Guid.Empty, -1);
			var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			var err = error.Errors.First();
			Assert.Equal("uuid", err.Key);
			Assert.Equal("Uuid must be a valid Guid and not Guid.Empty", err.Value.First());

			result = await controller.Post(Guid.NewGuid(), -1);
			bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("deviceId", err.Key);
			Assert.Equal("Device Id must be greater then or equal to 0", err.Value.First());

			var id = Guid.NewGuid();

			repo.Setup(i => i.AddAlternateUuidAsync(20, id)).ThrowsAsync(new AlternateDeviceIdException("Device already connected"));


			result = await controller.Post(id, 20);
			bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("uuid", err.Key);
			Assert.Equal("That Uuid is already connected to a device", err.Value.First());

			repo.Setup(i => i.AddAlternateUuidAsync(20, id)).ReturnsAsync((Device?)null);

			result = await controller.Post(id, 20);
			var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("notFound", err.Key);
			Assert.Equal("No device found with that id", err.Value.First());

			var device1 = new Device()
			{
				DeviceId = 20,
				Name = "Test Name",
				IsReadOnly = true,
				Description = "Dest Description",
				DateCreated = DateTime.UtcNow,
				DisplayOrder = 2
			};

			repo.Setup(i => i.AddAlternateUuidAsync(20, id)).ReturnsAsync(device1);

			result = await controller.Post(id, 20);
			var okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
			var device = Assert.IsAssignableFrom<ResponseModel<Device>>(okor.Value);
			AssertEqual(device1, device.Data);
		}

		[Fact]
		public async Task PostManufactureIdTest()
		{

			var repo = new Mock<IDeviceService>();
			using var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>());
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			var result = await controller.Post("", null, -1);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
			var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			var err = error.Errors.First();
			Assert.Equal("manufacture", err.Key);
			Assert.Equal("manufacture must not be null or whitespace", err.Value.First());

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			result = await controller.Post("Particle", null, -1);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
			bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("manufactureId", err.Key);
			Assert.Equal("manufactureId must not be null or whitespace", err.Value.First());

			result = await controller.Post("Particle", "Tinker", -1);
			bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("deviceId", err.Key);
			Assert.Equal("Device Id must be greater then or equal to 0", err.Value.First());

			var manufacture = "Particle";
			var manufactureId = "Photon";

			repo.Setup(i => i.AddAlternateManufactureIdAsync(20, manufacture, manufactureId)).ThrowsAsync(new AlternateDeviceIdException("Device already connected"));


			result = await controller.Post(manufacture, manufactureId, 20);
			bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("manufactureId", err.Key);
			Assert.Equal("That Manufacture and ManufactureId is already connected to a device", err.Value.First());

			repo.Setup(i => i.AddAlternateManufactureIdAsync(20, manufacture, manufactureId)).ReturnsAsync((Device?)null);

			result = await controller.Post(manufacture, manufactureId, 20);
			var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("notFound", err.Key);
			Assert.Equal("No device found with that id", err.Value.First());

			var device1 = new Device()
			{
				DeviceId = 20,
				Name = "Test Name",
				IsReadOnly = true,
				Description = "Dest Description",
				DateCreated = DateTime.UtcNow,
				DisplayOrder = 2
			};

			repo.Setup(i => i.AddAlternateManufactureIdAsync(20, manufacture, manufactureId)).ReturnsAsync(device1);

			result = await controller.Post(manufacture, manufactureId, 20);
			var okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
			var device = Assert.IsAssignableFrom<ResponseModel<Device>>(okor.Value);
			AssertEqual(device1, device.Data);
		}

		[Fact]
		public async Task DeleteMacAddressAsync()
		{

			var repo = new Mock<IDeviceService>();
			using var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>());
			var result = await controller.Delete(-1);
			var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			var err = error.Errors.First();
			Assert.Equal("macAddress", err.Key);
			Assert.Equal("Invalid MacAddress it must be greater then or equal to 0", err.Value.First());

			repo.Setup(i => i.RemoveAlternateMacAddressAsync(0xD5A6E4539A1F)).ReturnsAsync((Device?)null);

			result = await controller.Delete(0xD5A6E4539A1F);
			var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("macAddress", err.Key);
			Assert.Equal("Mac Address not found", err.Value.First());

			var device1 = new Device()
			{
				DeviceId = 20,
				Name = "Test Name",
				IsReadOnly = true,
				Description = "Dest Description",
				DateCreated = DateTime.UtcNow,
				DisplayOrder = 2
			};

			repo.Setup(i => i.RemoveAlternateMacAddressAsync(0xD5A6E4539A1F)).ReturnsAsync(device1);

			result = await controller.Delete(0xD5A6E4539A1F);
			var okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
			var device = Assert.IsAssignableFrom<ResponseModel<Device>>(okor.Value);
			AssertEqual(device1, device.Data);
		}

		[Fact]
		public async Task DeleteUuidAsync()
		{

			var repo = new Mock<IDeviceService>();
			using var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>());
			var result = await controller.Delete(Guid.Empty);
			var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			var err = error.Errors.First();
			Assert.Equal("uuid", err.Key);
			Assert.Equal("Uuid must be a valid Guid and not Guid.Empty", err.Value.First());

			var id = Guid.NewGuid();

			repo.Setup(i => i.RemoveAlternateUuidAsync(id)).ReturnsAsync((Device?)null);

			result = await controller.Delete(id);
			var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("uuid", err.Key);
			Assert.Equal("Uuid not found", err.Value.First());

			var device1 = new Device()
			{
				DeviceId = 20,
				Name = "Test Name",
				IsReadOnly = true,
				Description = "Dest Description",
				DateCreated = DateTime.UtcNow,
				DisplayOrder = 2
			};

			repo.Setup(i => i.RemoveAlternateUuidAsync(id)).ReturnsAsync(device1);

			result = await controller.Delete(id);
			var okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
			var device = Assert.IsAssignableFrom<ResponseModel<Device>>(okor.Value);
			AssertEqual(device1, device.Data);
		}

		[Fact]
		public async Task DeleteManufactureIdAsync()
		{

			var repo = new Mock<IDeviceService>();
			using var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>());
			var result = await controller.Delete("", "Photon");
			var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			var err = error.Errors.First();
			Assert.Equal("manufacture", err.Key);
			Assert.Equal("Manufacture must not be null or whitespace", err.Value.First());

			result = await controller.Delete("Particle", "");
			bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("manufactureId", err.Key);
			Assert.Equal("ManufactureId must not be null or whitespace", err.Value.First());

			var manufacture = "Particle";
			var manufactureId = "Photon";

			repo.Setup(i => i.RemoveAlternateManufactureIdAsync(manufacture, manufactureId)).ReturnsAsync((Device?)null);

			result = await controller.Delete(manufacture, manufactureId);
			var nfor = Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
			error = Assert.IsAssignableFrom<ErrorResponseModel>(nfor.Value);
			Assert.NotNull(error);
			Assert.Single(error.Errors);
			err = error.Errors.First();
			Assert.Equal("manufactureid", err.Key);
			Assert.Equal("Manufacture/ManufactureId not found", err.Value.First());

			var device1 = new Device()
			{
				DeviceId = 20,
				Name = "Test Name",
				IsReadOnly = true,
				Description = "Dest Description",
				DateCreated = DateTime.UtcNow,
				DisplayOrder = 2
			};

			repo.Setup(i => i.RemoveAlternateManufactureIdAsync(manufacture, manufactureId)).ReturnsAsync(device1);

			result = await controller.Delete(manufacture, manufactureId);
			var okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
			var device = Assert.IsAssignableFrom<ResponseModel<Device>>(okor.Value);
			AssertEqual(device1, device.Data);
		}
	}
}
