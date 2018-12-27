using Microsoft.AspNetCore.Mvc;
using Moq;
using Sannel.House.Devices.Controllers;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using Sannel.House.Models;
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
			Assert.Throws<ArgumentNullException>("repository",
				() => new AlternateIdController(null, null));
			Assert.Throws<ArgumentNullException>("logger",
				() => new AlternateIdController(new Mock<IDeviceRepository>().Object, null));
		}

		[Fact]
		public async Task GetAlternateIdsTest()
		{
			var repo = new Mock<IDeviceRepository>();
			using (var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>()))
			{
				var result = await controller.Get(-1);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.NotNull(error);
				Assert.Single(error.Errors);
				var err = error.Errors.First();
				Assert.Equal("deviceId", err.Key);
				Assert.Equal(HttpStatusCode.BadRequest, error.StatusCode);
				Assert.Equal("Device Id must be greater then or equal to 0", err.Value.First());

				repo.Setup(i => i.GetAlternateIdsForDeviceAsync(1)).ReturnsAsync(() => null);

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
				var okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
				var devices =
					Assert.IsAssignableFrom<ResponseModel<IEnumerable<AlternateDeviceId>>>(okor.Value);

				Assert.NotNull(devices);
				Assert.Empty(devices.Data);

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
				okor = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
				devices =
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
		}

		[Fact]
		public async Task PostMacAddressTest()
		{

			var repo = new Mock<IDeviceRepository>();
			using (var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>()))
			{
				var result = await controller.Post(-1, -1);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.NotNull(error);
				Assert.Single(error.Errors);
				var err = error.Errors.First();
				Assert.Equal("macAddress", err.Key);
				Assert.Equal("Invalid MacAddress it must be greater then or equal to 0", err.Value.First());

				result = await controller.Post(0xD5A6E4539A1F, -1);
				bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.NotNull(error);
				Assert.Single(error.Errors);
				err = error.Errors.First();
				Assert.Equal("deviceId", err.Key);
				Assert.Equal("Device Id must be greater then or equal to 0", err.Value.First());

				repo.Setup(i => i.AddAlternateMacAddressAsync(20, 0xD5A6E4539A1F)).ThrowsAsync(new AlternateDeviceIdException("Device already connected"));


				result = await controller.Post(0xD5A6E4539A1F, 20);
				bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.NotNull(error);
				Assert.Single(error.Errors);
				err = error.Errors.First();
				Assert.Equal("macAddress", err.Key);
				Assert.Equal("That mac address is already connected to a device", err.Value.First());

				repo.Setup(i => i.AddAlternateMacAddressAsync(20, 0xD5A6E4539A1F)).ReturnsAsync((Device)null);

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
		}

		[Fact]
		public async Task PostUuidTest()
		{

			var repo = new Mock<IDeviceRepository>();
			using (var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>()))
			{
				var result = await controller.Post(Guid.Empty, -1);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.NotNull(error);
				Assert.Single(error.Errors);
				var err = error.Errors.First();
				Assert.Equal("uuid", err.Key);
				Assert.Equal("Uuid must be a valid Guid and not Guid.Empty",err.Value.First());

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

				repo.Setup(i => i.AddAlternateUuidAsync(20, id)).ReturnsAsync((Device)null);

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
		}

		[Fact]
		public async Task DeleteMacAddressAsync()
		{

			var repo = new Mock<IDeviceRepository>();
			using (var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>()))
			{
				var result = await controller.Delete(-1);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.NotNull(error);
				Assert.Single(error.Errors);
				var err = error.Errors.First();
				Assert.Equal("macAddress", err.Key);
				Assert.Equal("Invalid MacAddress it must be greater then or equal to 0", err.Value.First());

				repo.Setup(i => i.RemoveAlternateMacAddressAsync(0xD5A6E4539A1F)).ReturnsAsync((Device)null);

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
		}

		[Fact]
		public async Task DeleteUuidAsync()
		{

			var repo = new Mock<IDeviceRepository>();
			using (var controller = new AlternateIdController(repo.Object, CreateLogger<AlternateIdController>()))
			{
				var result = await controller.Delete(Guid.Empty);
				var bror = Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
				var error = Assert.IsAssignableFrom<ErrorResponseModel>(bror.Value);
				Assert.NotNull(error);
				Assert.Single(error.Errors);
				var err = error.Errors.First();
				Assert.Equal("uuid", err.Key);
				Assert.Equal("Uuid must be a valid Guid and not Guid.Empty",err.Value.First());

				var id = Guid.NewGuid();

				repo.Setup(i => i.RemoveAlternateUuidAsync(id)).ReturnsAsync((Device)null);

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
		}
	}
}
