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
		private readonly IDeviceService service;
		private readonly ILogger logger;

		public UnknownDeviceSubscriber(IDeviceService service, IConfiguration configuration, ILogger<UnknownDeviceSubscriber> logger)
		{
			this.service = service ?? throw new ArgumentNullException(nameof(service));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Topic = (configuration ?? throw new ArgumentNullException(nameof(configuration)))["MQTT:UnknownDeviceTopic"];
		}

		public string Topic
		{
			get;
			private set;
		}

		private async Task<Device> createDeviceAsync()
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
			UnknownDeviceMessage obj;
			try
			{
				obj = JsonSerializer.Deserialize<UnknownDeviceMessage>(message, new JsonSerializerOptions()
				{
					PropertyNameCaseInsensitive = true
				});
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
					device = await createDeviceAsync();

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
					device = await createDeviceAsync();
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
					device = await createDeviceAsync();
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
