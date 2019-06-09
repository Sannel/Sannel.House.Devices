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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Sannel.House.Devices.Client
{
	public static class Extensions
	{
		/// <summary>
		/// Adds the devices HTTP client registration.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="baseUrl">The base URL.</param>
		/// <returns></returns>
		public static IServiceCollection AddDevicesHttpClientRegistration(this IServiceCollection service, Uri baseUrl)
		{
			Sannel.House.Client.Helpers.RegisterClient(service, baseUrl, "/api/v1/", nameof(DevicesClient), "1.0");

			return service;
		}

		/// <summary>
		/// Adds the devices client registration.
		/// </summary>
		/// <param name="services">The services.</param>
		/// <returns></returns>
		public static IServiceCollection AddDevicesClientRegistration(this IServiceCollection services)
			=> services.AddTransient((s) =>
				new DevicesClient(s.GetService<IHttpClientFactory>(),
					s.GetService<ILogger<DevicesClient>>()));

		/// <summary>
		/// Adds the devices client.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="baseUrl">The base URL.</param>
		/// <returns></returns>
		public static IServiceCollection AddDevicesClient(this IServiceCollection service, Uri baseUrl)
			=> service.AddDevicesHttpClientRegistration(baseUrl)
				.AddDevicesClientRegistration();

	}
}
