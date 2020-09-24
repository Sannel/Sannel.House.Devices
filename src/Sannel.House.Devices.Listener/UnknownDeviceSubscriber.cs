/* Copyright 2020-2020 Sannel Software, L.L.C.
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
      http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Sannel.House.Base.Messages.Device;
using Sannel.House.Base.MQTT.Interfaces;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sannel.House.Devices.Listener
{
	public class UnknownDeviceSubscriber : IMqttTopicSubscriber
	{
		private readonly IServiceProvider provider;
		private readonly ILogger logger;

		public UnknownDeviceSubscriber(IServiceProvider provider, IConfiguration configuration, ILogger<UnknownDeviceSubscriber> logger)
		{
			this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Topic = (configuration ?? throw new ArgumentNullException(nameof(configuration)))["MQTT:UnknownDeviceTopic"];
		}

		public string Topic
		{
			get;
			private set;
		}

		private async Task<Device> createDeviceAsync(IDeviceService service)
		{
			var device = new Device()
			{
				Name = "Unknown Device",
				Description = "Unknown Device",
				DateCreated = DateTimeOffset.Now,
				Verified = false
			};

			device = await service.AddDeviceAsync(device);

			return device;
		}

		public async void Message(string topic, string message)
			=> await MessageAsync(topic, message);

		internal async Task MessageAsync(string topic, string message)
		{
			using var scope = provider.CreateScope();
			var service = scope.ServiceProvider.GetService<IDeviceService>();

			if(service is null)
			{
				logger.LogError("Unable to get service IDeviceService. Unable to process topic {topic} with message {message}",
					topic,
					message);
				return;
			}

			UnknownDeviceMessage obj;
			try
			{
				obj = JsonSerializer.Deserialize<UnknownDeviceMessage>(message, new JsonSerializerOptions()
				{
					PropertyNameCaseInsensitive = true
				}) ?? throw new NullReferenceException("Deserialization Resulted in null value");
			}
			catch(JsonException ex)
			{
				logger.LogError("Exception Deserializing Message {message}. Exception {exception}", 
					message, 
					ex);
				return;
			}

			if(obj.MacAddress != null)
			{
				if(logger.IsEnabled(LogLevel.Debug))
				{
					logger.LogDebug("Mac Address received {MacAddress}", obj.MacAddress);
				}
				var device = await service.GetDeviceByMacAddressAsync(obj.MacAddress.Value);

				if(device != null)
				{
					await service.PublishDeviceAsync(device.DeviceId);
				}
				else
				{
					device = await createDeviceAsync(service);

					if(device?.DeviceId > default(int))
					{
						await service.AddAlternateMacAddressAsync(device.DeviceId, obj.MacAddress.Value);
					}

				}
			}
			else if(obj.Uuid != null)
			{
				if(logger.IsEnabled(LogLevel.Debug))
				{
					logger.LogDebug("Uuid received {Uuid}", obj.Uuid);
				}

				var device = await service.GetDeviceByUuidAsync(obj.Uuid.Value);

				if(device != null)
				{
					await service.PublishDeviceAsync(device.DeviceId);
				}
				else
				{
					device = await createDeviceAsync(service);
					if(device?.DeviceId > default(int))
					{
						await service.AddAlternateUuidAsync(device.DeviceId, obj.Uuid.Value);
					}
				}
			}
			else if(!string.IsNullOrWhiteSpace(obj.Manufacture) 
				&& !string.IsNullOrWhiteSpace(obj.ManufactureId))
			{
				if(logger.IsEnabled(LogLevel.Debug))
				{
					logger.LogDebug("Manufacture and ManufactureId received {Manufacture} {ManufactureId}",
						obj.Manufacture,
						obj.ManufactureId);
				}

				var device = await service.GetDeviceByManufactureIdAsync(obj.Manufacture, obj.ManufactureId);

				if(device != null)
				{
					await service.PublishDeviceAsync(device.DeviceId);
				}
				else
				{
					device = await createDeviceAsync(service);
					if(device?.DeviceId > default(int))
					{
						await service.AddAlternateManufactureIdAsync(device.DeviceId,
							obj.Manufacture,
							obj.ManufactureId);
					}
				}
			}
			else
			{
				logger.LogWarning("Unknown UnknownDevice {message}", message);
			}
		}
	}
}
