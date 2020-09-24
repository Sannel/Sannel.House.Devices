using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Listener;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sannel.House.Devices.Tests.Subscribers
{
	public class UnknownDeviceSubscriberTests : BaseTests
	{
		[Fact]
		public async Task InvalidJsonTestAsync()
		{
			var deviceService = new Mock<IDeviceService>();
			var config = new Mock<IConfiguration>();

			var builder = new ServiceCollection();
			builder.AddSingleton(deviceService.Object);

			var unknownDeviceSubscriber = new UnknownDeviceSubscriber(builder.BuildServiceProvider(),
				config.Object,
				CreateLogger<UnknownDeviceSubscriber>());

			var message = new Sannel.House.Base.Messages.Device.UnknownDeviceMessage()
			{
			};

			var device = new Device()
			{
				DeviceId = Random.Next(50, 20000),
				Name = Guid.NewGuid().ToString(),
				Description = Guid.NewGuid().ToString(),
				DateCreated = DateTimeOffset.Now,
				DisplayOrder = Random.Next(20, 1111),
				IsReadOnly = Random.Next(0, 1) == 1,
				Verified = false
			};

			var getDeviceByUuidCalled = 0;
			deviceService.Setup(i => i.GetDeviceByUuidAsync(It.IsAny<Guid>()))
				.Callback((Guid uuid) =>
				{
					getDeviceByUuidCalled++;
					Assert.Equal(message.Uuid, uuid);
				})
				.ReturnsAsync(device);

			var getDeviceByMacAddressCalled = 0;
			deviceService.Setup(i => i.GetDeviceByMacAddressAsync(It.IsAny<long>()))
				.Callback((long macAddress) =>
				{
					getDeviceByMacAddressCalled++;
				}).ReturnsAsync(device);

			var getDeviceByManufactureIdCalled = 0;
			deviceService.Setup(i => i.GetDeviceByManufactureIdAsync(It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string manufacture, string manufactureId) =>
				{
					getDeviceByManufactureIdCalled++;
				}).ReturnsAsync(device);

			var publishDeviceAsyncCalled = 0;
			deviceService.Setup(i => i.PublishDeviceAsync(It.IsAny<int>()))
				.Callback((int deviceId) =>
				{
					publishDeviceAsyncCalled++;
					Assert.Equal(device.DeviceId, deviceId);
				});

			var addDeviceCalled = 0;
			deviceService.Setup(i => i.AddDeviceAsync(It.IsAny<Device>()))
				.Callback((Device device) =>
				{
					addDeviceCalled++;
				});

			var addAlternateUuidCalled = 0;
			deviceService.Setup(i => i.AddAlternateUuidAsync(It.IsAny<int>(), It.IsAny<Guid>()))
				.Callback((int deviceId, Guid uuid) =>
				{
					addAlternateUuidCalled++;
				});

			await unknownDeviceSubscriber.MessageAsync(Guid.NewGuid().ToString(), "{");
			Assert.Equal(0, getDeviceByUuidCalled);
			Assert.Equal(0, getDeviceByMacAddressCalled);
			Assert.Equal(0, getDeviceByManufactureIdCalled);
			Assert.Equal(0, publishDeviceAsyncCalled);
			Assert.Equal(0, addDeviceCalled);
			Assert.Equal(0, addAlternateUuidCalled);

		}
		[Fact]
		public async Task NothingSetTestAsync()
		{
			var deviceService = new Mock<IDeviceService>();
			var config = new Mock<IConfiguration>();

			var builder = new ServiceCollection();
			builder.AddSingleton(deviceService.Object);

			var unknownDeviceSubscriber = new UnknownDeviceSubscriber(builder.BuildServiceProvider(),
				config.Object,
				CreateLogger<UnknownDeviceSubscriber>());

			var message = new Sannel.House.Base.Messages.Device.UnknownDeviceMessage()
			{
			};

			var device = new Device()
			{
				DeviceId = Random.Next(50, 20000),
				Name = Guid.NewGuid().ToString(),
				Description = Guid.NewGuid().ToString(),
				DateCreated = DateTimeOffset.Now,
				DisplayOrder = Random.Next(20, 1111),
				IsReadOnly = Random.Next(0, 1) == 1,
				Verified = false
			};

			var getDeviceByUuidCalled = 0;
			deviceService.Setup(i => i.GetDeviceByUuidAsync(It.IsAny<Guid>()))
				.Callback((Guid uuid) =>
				{
					getDeviceByUuidCalled++;
					Assert.Equal(message.Uuid, uuid);
				})
				.ReturnsAsync(device);

			var getDeviceByMacAddressCalled = 0;
			deviceService.Setup(i => i.GetDeviceByMacAddressAsync(It.IsAny<long>()))
				.Callback((long macAddress) =>
				{
					getDeviceByMacAddressCalled++;
				}).ReturnsAsync(device);

			var getDeviceByManufactureIdCalled = 0;
			deviceService.Setup(i => i.GetDeviceByManufactureIdAsync(It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string manufacture, string manufactureId) =>
				{
					getDeviceByManufactureIdCalled++;
				}).ReturnsAsync(device);

			var publishDeviceAsyncCalled = 0;
			deviceService.Setup(i => i.PublishDeviceAsync(It.IsAny<int>()))
				.Callback((int deviceId) =>
				{
					publishDeviceAsyncCalled++;
					Assert.Equal(device.DeviceId, deviceId);
				});

			var addDeviceCalled = 0;
			deviceService.Setup(i => i.AddDeviceAsync(It.IsAny<Device>()))
				.Callback((Device device) =>
				{
					addDeviceCalled++;
				});

			var addAlternateUuidCalled = 0;
			deviceService.Setup(i => i.AddAlternateUuidAsync(It.IsAny<int>(), It.IsAny<Guid>()))
				.Callback((int deviceId, Guid uuid) =>
				{
					addAlternateUuidCalled++;
				});

			await unknownDeviceSubscriber.MessageAsync(Guid.NewGuid().ToString(), System.Text.Json.JsonSerializer.Serialize(message));
			Assert.Equal(0, getDeviceByUuidCalled);
			Assert.Equal(0, getDeviceByMacAddressCalled);
			Assert.Equal(0, getDeviceByManufactureIdCalled);
			Assert.Equal(0, publishDeviceAsyncCalled);
			Assert.Equal(0, addDeviceCalled);
			Assert.Equal(0, addAlternateUuidCalled);

		}
		[Fact]
		public async Task ExistingUuidTestAsync()
		{
			var deviceService = new Mock<IDeviceService>();
			var config = new Mock<IConfiguration>();

			var builder = new ServiceCollection();
			builder.AddSingleton(deviceService.Object);

			var unknownDeviceSubscriber = new UnknownDeviceSubscriber(builder.BuildServiceProvider(),
				config.Object,
				CreateLogger<UnknownDeviceSubscriber>());

			var message = new Sannel.House.Base.Messages.Device.UnknownDeviceMessage()
			{
				Uuid = Guid.NewGuid()
			};

			var device = new Device()
			{
				DeviceId = Random.Next(50, 20000),
				Name = Guid.NewGuid().ToString(),
				Description = Guid.NewGuid().ToString(),
				DateCreated = DateTimeOffset.Now,
				DisplayOrder = Random.Next(20, 1111),
				IsReadOnly = Random.Next(0, 1) == 1,
				Verified = false
			};

			var getDeviceByUuidCalled = 0;
			deviceService.Setup(i => i.GetDeviceByUuidAsync(It.IsAny<Guid>()))
				.Callback((Guid uuid) =>
				{
					getDeviceByUuidCalled++;
					Assert.Equal(message.Uuid, uuid);
				})
				.ReturnsAsync(device);

			var publishDeviceAsyncCalled = 0;
			deviceService.Setup(i => i.PublishDeviceAsync(It.IsAny<int>()))
				.Callback((int deviceId) =>
				{
					publishDeviceAsyncCalled++;
					Assert.Equal(device.DeviceId, deviceId);
				});

			var addDeviceCalled = 0;
			deviceService.Setup(i => i.AddDeviceAsync(It.IsAny<Device>()))
				.Callback((Device device) =>
				{
					addDeviceCalled++;
				});

			var addAlternateUuidCalled = 0;
			deviceService.Setup(i => i.AddAlternateUuidAsync(It.IsAny<int>(), It.IsAny<Guid>()))
				.Callback((int deviceId, Guid uuid) =>
				{
					addAlternateUuidCalled++;
				});

			await unknownDeviceSubscriber.MessageAsync(Guid.NewGuid().ToString(), System.Text.Json.JsonSerializer.Serialize(message));
			Assert.Equal(1, getDeviceByUuidCalled);
			Assert.Equal(1, publishDeviceAsyncCalled);
			Assert.Equal(0, addDeviceCalled);
			Assert.Equal(0, addAlternateUuidCalled);

		}

		[Fact]
		public async Task NewUuidTestAsync()
		{
			var deviceService = new Mock<IDeviceService>();
			var config = new Mock<IConfiguration>();

			var builder = new ServiceCollection();
			builder.AddSingleton(deviceService.Object);

			var unknownDeviceSubscriber = new UnknownDeviceSubscriber(builder.BuildServiceProvider(),
				config.Object,
				CreateLogger<UnknownDeviceSubscriber>());

			var message = new Sannel.House.Base.Messages.Device.UnknownDeviceMessage()
			{
				Uuid = Guid.NewGuid()
			};

			var device = new Device();

			var getDeviceByUuidCalled = 0;
			deviceService.Setup(i => i.GetDeviceByUuidAsync(It.IsAny<Guid>()))
				.Callback((Guid uuid) =>
				{
					getDeviceByUuidCalled++;
					Assert.Equal(message.Uuid, uuid);
				})
				.ReturnsAsync((Device?)null);

			var publishDeviceAsyncCalled = 0;
			deviceService.Setup(i => i.PublishDeviceAsync(It.IsAny<int>()))
				.Callback((int deviceId) =>
				{
					publishDeviceAsyncCalled++;
					Assert.Equal(device.DeviceId, deviceId);
				});

			var addDeviceCalled = 0;
			deviceService.Setup(i => i.AddDeviceAsync(It.IsAny<Device>()))
				.ReturnsAsync((Device sentDevice) =>
				{
					addDeviceCalled++;
					Assert.NotNull(sentDevice);
					Assert.Equal("Unknown Device", sentDevice.Name);
					Assert.Equal("Unknown Device", sentDevice.Description);
					Assert.Equal(default, sentDevice.DeviceId);
					Assert.Equal(DateTimeOffset.Now.UtcDateTime, sentDevice.DateCreated.UtcDateTime, TimeSpan.FromSeconds(5));
					Assert.False(sentDevice.Verified);
					Assert.False(sentDevice.IsReadOnly);
					Assert.Equal(default, sentDevice.DisplayOrder);

					device = new Device()
					{
						DeviceId = Random.Next(50, 10000),
						Name = sentDevice.Name,
						Description = sentDevice.Description,
						DateCreated = sentDevice.DateCreated,
						Verified = sentDevice.Verified,
						IsReadOnly = false,
						DisplayOrder = Random.Next(5000, 10000)
					};
					return device;
				});

			var addAlternateUuidCalled = 0;
			deviceService.Setup(i => i.AddAlternateUuidAsync(It.IsAny<int>(), It.IsAny<Guid>()))
				.Callback((int deviceId, Guid uuid) =>
				{
					addAlternateUuidCalled++;
					Assert.Equal(device.DeviceId, deviceId);
					Assert.Equal(message.Uuid, uuid);
				});

			await unknownDeviceSubscriber.MessageAsync(Guid.NewGuid().ToString(), System.Text.Json.JsonSerializer.Serialize(message));
			Assert.Equal(1, getDeviceByUuidCalled);
			Assert.Equal(0, publishDeviceAsyncCalled);
			Assert.Equal(1, addDeviceCalled);
			Assert.Equal(1, addAlternateUuidCalled);

		}

		[Fact]
		public async Task ExistingMacAddressTestAsync()
		{
			var deviceService = new Mock<IDeviceService>();
			var config = new Mock<IConfiguration>();

			var builder = new ServiceCollection();
			builder.AddSingleton(deviceService.Object);

			var unknownDeviceSubscriber = new UnknownDeviceSubscriber(builder.BuildServiceProvider(),
				config.Object,
				CreateLogger<UnknownDeviceSubscriber>());

			var message = new Sannel.House.Base.Messages.Device.UnknownDeviceMessage()
			{
				MacAddress = (long)Math.Truncate(Random.NextDouble() * int.MaxValue)
			};

			var device = new Device()
			{
				DeviceId = Random.Next(50, 20000),
				Name = Guid.NewGuid().ToString(),
				Description = Guid.NewGuid().ToString(),
				DateCreated = DateTimeOffset.Now,
				DisplayOrder = Random.Next(20, 1111),
				IsReadOnly = Random.Next(0, 1) == 1,
				Verified = false
			};

			var getDeviceByMacAddressCalled = 0;
			deviceService.Setup(i => i.GetDeviceByMacAddressAsync(It.IsAny<long>()))
				.Callback((long macAddress) =>
				{
					getDeviceByMacAddressCalled++;
					Assert.Equal(message.MacAddress, macAddress);
				})
				.ReturnsAsync(device);

			var publishDeviceAsyncCalled = 0;
			deviceService.Setup(i => i.PublishDeviceAsync(It.IsAny<int>()))
				.Callback((int deviceId) =>
				{
					publishDeviceAsyncCalled++;
					Assert.Equal(device.DeviceId, deviceId);
				});

			var addDeviceCalled = 0;
			deviceService.Setup(i => i.AddDeviceAsync(It.IsAny<Device>()))
				.Callback((Device device) =>
				{
					addDeviceCalled++;
				});

			var addAlternateUuidCalled = 0;
			deviceService.Setup(i => i.AddAlternateMacAddressAsync(It.IsAny<int>(), It.IsAny<long>()))
				.Callback((int deviceId, long macAddress) =>
				{
					addAlternateUuidCalled++;
				});

			await unknownDeviceSubscriber.MessageAsync(Guid.NewGuid().ToString(), System.Text.Json.JsonSerializer.Serialize(message));
			Assert.Equal(1, getDeviceByMacAddressCalled);
			Assert.Equal(1, publishDeviceAsyncCalled);
			Assert.Equal(0, addDeviceCalled);
			Assert.Equal(0, addAlternateUuidCalled);

		}

		[Fact]
		public async Task NewMacAddressTestAsync()
		{
			var deviceService = new Mock<IDeviceService>();
			var config = new Mock<IConfiguration>();

			var builder = new ServiceCollection();
			builder.AddSingleton(deviceService.Object);

			var unknownDeviceSubscriber = new UnknownDeviceSubscriber(builder.BuildServiceProvider(),
				config.Object,
				CreateLogger<UnknownDeviceSubscriber>());

			var message = new Sannel.House.Base.Messages.Device.UnknownDeviceMessage()
			{
				MacAddress = (long)Math.Truncate(Random.NextDouble() * int.MaxValue)
			};

			var device = new Device();

			var getDeviceByMacAddressCalled = 0;
			deviceService.Setup(i => i.GetDeviceByMacAddressAsync(It.IsAny<long>()))
				.Callback((long macAddess) =>
				{
					getDeviceByMacAddressCalled++;
					Assert.Equal(message.MacAddress, macAddess);
				})
				.ReturnsAsync((Device?)null);

			var publishDeviceAsyncCalled = 0;
			deviceService.Setup(i => i.PublishDeviceAsync(It.IsAny<int>()))
				.Callback((int deviceId) =>
				{
					publishDeviceAsyncCalled++;
					Assert.Equal(device.DeviceId, deviceId);
				});

			var addDeviceCalled = 0;
			deviceService.Setup(i => i.AddDeviceAsync(It.IsAny<Device>()))
				.ReturnsAsync((Device sentDevice) =>
				{
					addDeviceCalled++;
					Assert.NotNull(sentDevice);
					Assert.Equal("Unknown Device", sentDevice.Name);
					Assert.Equal("Unknown Device", sentDevice.Description);
					Assert.Equal(default, sentDevice.DeviceId);
					Assert.Equal(DateTimeOffset.Now.UtcDateTime, sentDevice.DateCreated.UtcDateTime, TimeSpan.FromSeconds(5));
					Assert.False(sentDevice.Verified);
					Assert.False(sentDevice.IsReadOnly);
					Assert.Equal(default, sentDevice.DisplayOrder);

					device = new Device()
					{
						DeviceId = Random.Next(50, 10000),
						Name = sentDevice.Name,
						Description = sentDevice.Description,
						DateCreated = sentDevice.DateCreated,
						Verified = sentDevice.Verified,
						IsReadOnly = false,
						DisplayOrder = Random.Next(5000, 10000)
					};
					return device;
				});

			var addAlternateUuidCalled = 0;
			deviceService.Setup(i => i.AddAlternateMacAddressAsync(It.IsAny<int>(), It.IsAny<long>()))
				.Callback((int deviceId, long macAddress) =>
				{
					addAlternateUuidCalled++;
					Assert.Equal(device.DeviceId, deviceId);
					Assert.Equal(message.MacAddress, macAddress);
				});

			await unknownDeviceSubscriber.MessageAsync(Guid.NewGuid().ToString(), System.Text.Json.JsonSerializer.Serialize(message));
			Assert.Equal(1, getDeviceByMacAddressCalled);
			Assert.Equal(0, publishDeviceAsyncCalled);
			Assert.Equal(1, addDeviceCalled);
			Assert.Equal(1, addAlternateUuidCalled);

		}

		[Fact]
		public async Task ExistingManufactureTestAsync()
		{
			var deviceService = new Mock<IDeviceService>();
			var config = new Mock<IConfiguration>();

			var builder = new ServiceCollection();
			builder.AddSingleton(deviceService.Object);

			var unknownDeviceSubscriber = new UnknownDeviceSubscriber(builder.BuildServiceProvider(),
				config.Object,
				CreateLogger<UnknownDeviceSubscriber>());

			var message = new Sannel.House.Base.Messages.Device.UnknownDeviceMessage()
			{
				Manufacture = Guid.NewGuid().ToString(),
				ManufactureId = Guid.NewGuid().ToString()
			};

			var device = new Device()
			{
				DeviceId = Random.Next(50, 20000),
				Name = Guid.NewGuid().ToString(),
				Description = Guid.NewGuid().ToString(),
				DateCreated = DateTimeOffset.Now,
				DisplayOrder = Random.Next(20, 1111),
				IsReadOnly = Random.Next(0, 1) == 1,
				Verified = false
			};

			var getDeviceByMacAddressCalled = 0;
			deviceService.Setup(i => i.GetDeviceByManufactureIdAsync(It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string manufacture, string manufactureId) =>
				{
					getDeviceByMacAddressCalled++;
					Assert.Equal(message.Manufacture, manufacture);
					Assert.Equal(message.ManufactureId, manufactureId);
				})
				.ReturnsAsync(device);

			var publishDeviceAsyncCalled = 0;
			deviceService.Setup(i => i.PublishDeviceAsync(It.IsAny<int>()))
				.Callback((int deviceId) =>
				{
					publishDeviceAsyncCalled++;
					Assert.Equal(device.DeviceId, deviceId);
				});

			var addDeviceCalled = 0;
			deviceService.Setup(i => i.AddDeviceAsync(It.IsAny<Device>()))
				.Callback((Device device) =>
				{
					addDeviceCalled++;
				});

			var addAlternateUuidCalled = 0;
			deviceService.Setup(i => i.AddAlternateManufactureIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((int deviceId, string manufacture, string manufactureId) =>
				{
					addAlternateUuidCalled++;
				});

			await unknownDeviceSubscriber.MessageAsync(Guid.NewGuid().ToString(), System.Text.Json.JsonSerializer.Serialize(message));
			Assert.Equal(1, getDeviceByMacAddressCalled);
			Assert.Equal(1, publishDeviceAsyncCalled);
			Assert.Equal(0, addDeviceCalled);
			Assert.Equal(0, addAlternateUuidCalled);

		}

		[Fact]
		public async Task NewManufactureIdTestAsync()
		{
			var deviceService = new Mock<IDeviceService>();
			var config = new Mock<IConfiguration>();

			var builder = new ServiceCollection();
			builder.AddSingleton(deviceService.Object);

			var unknownDeviceSubscriber = new UnknownDeviceSubscriber(builder.BuildServiceProvider(),
				config.Object,
				CreateLogger<UnknownDeviceSubscriber>());

			var message = new Sannel.House.Base.Messages.Device.UnknownDeviceMessage()
			{
				Manufacture = Guid.NewGuid().ToString(),
				ManufactureId = Guid.NewGuid().ToString()
			};

			var device = new Device();

			var getDeviceByMacAddressCalled = 0;
			deviceService.Setup(i => i.GetDeviceByManufactureIdAsync(It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string manufacture, string manufactureId) =>
				{
					getDeviceByMacAddressCalled++;
					Assert.Equal(message.Manufacture, manufacture);
					Assert.Equal(message.ManufactureId, manufactureId);
				})
				.ReturnsAsync((Device?)null);

			var publishDeviceAsyncCalled = 0;
			deviceService.Setup(i => i.PublishDeviceAsync(It.IsAny<int>()))
				.Callback((int deviceId) =>
				{
					publishDeviceAsyncCalled++;
					Assert.Equal(device.DeviceId, deviceId);
				});

			var addDeviceCalled = 0;
			deviceService.Setup(i => i.AddDeviceAsync(It.IsAny<Device>()))
				.ReturnsAsync((Device sentDevice) =>
				{
					addDeviceCalled++;
					Assert.NotNull(sentDevice);
					Assert.Equal("Unknown Device", sentDevice.Name);
					Assert.Equal("Unknown Device", sentDevice.Description);
					Assert.Equal(default, sentDevice.DeviceId);
					Assert.Equal(DateTimeOffset.Now.UtcDateTime, sentDevice.DateCreated.UtcDateTime, TimeSpan.FromSeconds(5));
					Assert.False(sentDevice.Verified);
					Assert.False(sentDevice.IsReadOnly);
					Assert.Equal(default, sentDevice.DisplayOrder);

					device = new Device()
					{
						DeviceId = Random.Next(50, 10000),
						Name = sentDevice.Name,
						Description = sentDevice.Description,
						DateCreated = sentDevice.DateCreated,
						Verified = sentDevice.Verified,
						IsReadOnly = false,
						DisplayOrder = Random.Next(5000, 10000)
					};
					return device;
				});

			var addAlternateUuidCalled = 0;
			deviceService.Setup(i => i.AddAlternateManufactureIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((int deviceId, string manufacture, string manufactureId) =>
				{
					addAlternateUuidCalled++;
					Assert.Equal(device.DeviceId, deviceId);
					Assert.Equal(message.Manufacture, manufacture);
					Assert.Equal(message.ManufactureId, manufactureId);
				});

			await unknownDeviceSubscriber.MessageAsync(Guid.NewGuid().ToString(), System.Text.Json.JsonSerializer.Serialize(message));
			Assert.Equal(1, getDeviceByMacAddressCalled);
			Assert.Equal(0, publishDeviceAsyncCalled);
			Assert.Equal(1, addDeviceCalled);
			Assert.Equal(1, addAlternateUuidCalled);

		}
	}
}
