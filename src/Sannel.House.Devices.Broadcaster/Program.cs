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
using MQTTnet.Server;
using Sannel.House.Base.Models;
using Sannel.House.Base.MQTT;
using Sannel.House.Base.Web;
using Sannel.House.Devices.Data;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using Sannel.House.Devices.Repositories;
using Sannel.House.Devices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sannel.House.Devices.Broadcaster
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var configuration = buildConfiguration(args);

			var serviceProvider = buildServices(configuration);

			using var scope = serviceProvider.CreateScope();

			var pageSize = configuration.GetValue<int?>("PageSize") ?? 25;
			var delayMilliseconds = TimeSpan.FromMilliseconds(configuration.GetValue<long?>("DelayMilliSeconds") ?? 5000);

			var mqttService = scope.ServiceProvider.GetService<MqttService>();
			var deviceService = scope.ServiceProvider.GetService<IDeviceService>();
			var logger = scope.ServiceProvider.GetService<ILogger<Program>>();

			if(mqttService is null)
			{
				throw new NullReferenceException("MqttService is not configured in Service Provider");
			}

			if(logger is null)
			{
				throw new NullReferenceException("ILogger is not configured in Service Provider");
			}

			if(deviceService is null)
			{
				throw new NullReferenceException("IDeviceService is not configured in Service Provider");
			}

			await mqttService.StartAsync(default);

			var pageIndex = 0;
			PagedResponseModel<Device> result;
			List<Device> data;

			var debugEnabled = logger.IsEnabled(LogLevel.Debug);

			do
			{
				result = await deviceService.GetDevicesListAsync(pageIndex, pageSize);
				data = result.Data.ToList();
				if (data.Any())
				{
					if(debugEnabled)
					{
						logger.LogDebug("Result from Service TotalCount: {TotalCount} Page: {Page} PageSize: {PageSize}", result.TotalCount, result.Page, result.PageSize);
					}

					foreach (var device in result.Data)
					{
						if(debugEnabled)
						{
							logger.LogDebug("Device: {DeviceId} Description: {Description} DateCreated: {DateCreated} DisplayOrder: {DisplayOrder} IsReadOnly: {IsReadOnly} Verified: {Verified}",
								device.DeviceId,
								device.Description,
								device.DateCreated,
								device.DisplayOrder,
								device.IsReadOnly,
								device.Verified);
						}
						await deviceService.PublishDeviceAsync(device);
					}

					logger.LogInformation("Page Sent Delaying sending next page by MilliSeconds {DelayMilliSeconds}", delayMilliseconds.TotalMilliseconds);
					await Task.Delay(delayMilliseconds);
				}
				else
				{
					logger.LogInformation("No more devices. TotalDevices {TotalDevices}", result.TotalCount);
				}
				pageIndex++;
			} while (data.Any());

			await mqttService.StopAsync(default);
			logger.LogInformation("Done Sending Devices");
		}

		private static IConfiguration buildConfiguration(string[] args)
		{
			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddJsonFile("appsettings.json", false);
			configurationBuilder.AddJsonFile("app_config/appsettings.json", true);
			configurationBuilder.AddYamlFile("app_config/appsettings.yml", true);
			configurationBuilder.AddEnvironmentVariables();
			configurationBuilder.AddCommandLine(args);

			return configurationBuilder.Build();
		}
		
		private static ServiceProvider buildServices(IConfiguration configuration)
		{
			var services = new ServiceCollection();
			services.AddSingleton(configuration);
			services.AddSingleton<IConfiguration>(configuration);
			services.AddLogging(i =>
			{
				i.AddConsole();
			});

			var connectionString = configuration.GetWithReplacement("Db:ConnectionString");
			if (string.IsNullOrWhiteSpace(connectionString))
			{
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
				throw new ArgumentNullException("Db:ConnectionString", "Db:ConnectionString is required");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
			}

			services.AddDbContextPool<DevicesDbContext>(o =>
			{
				switch (configuration["Db:Provider"])
				{
					case "MySQL":
					case "mysql":
						throw new NotSupportedException("We are currently not supporting mysql as a database provider");

					case "sqlserver":
					case "SqlServer":
						o.ConfigureSqlServer(connectionString);
						break;
					case "PostgreSQL":
					case "postgresql":
						o.ConfigurePostgreSQL(connectionString);
						break;
					case "sqlite":
					default:
						o.ConfigureSqlite(connectionString);
						break;
				}
			});

			services.AddScoped<IDeviceRepository, DbContextRepository>();
			services.AddScoped<IDeviceService, DeviceService>();

			services.AddMqttPublishService(configuration["MQTT:Server"], 
										configuration["MQTT:DefaultTopic"], 
										configuration.GetValue<int?>("MQTT:Port"));

			return services.BuildServiceProvider();
		}
	}
}
