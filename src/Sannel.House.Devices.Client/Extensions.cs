using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Devices.Client
{
	public static class Extensions
	{
		public static IServiceCollection AddDevicesHttpClientRegistration(this IServiceCollection service, Uri baseUrl)
		{
			var builder = new UriBuilder(baseUrl)
			{
				Path = "/api/v1/"
			};
			service.AddHttpClient(nameof(DevicesClient), (i) =>
			{
				i.BaseAddress = builder.Uri;
				i.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
				i.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("DevicesClient","1.0"));
			});

			return service;
		}
	}
}
