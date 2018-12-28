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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Linq;
using System.Net.Http;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Sannel.House.Client;
using Sannel.House.Devices.Client;

namespace Sannel.House.Devices.Tests.Client
{
	public class ExtensionsTests
	{
		[Fact]
		public void AddDevicesHttpClientRegistrationTest()
		{
			var service = new ServiceCollection();
			service.AddDevicesHttpClientRegistration(new Uri("http://gateway.dev.local"));
			var provider = service.BuildServiceProvider();
			var factory = provider.GetService<IHttpClientFactory>();
			var httpClient = factory.CreateClient(nameof(DevicesClient));
			Assert.NotNull(httpClient);
			Assert.Equal("http://gateway.dev.local/api/v1/", httpClient.BaseAddress.ToString());
			if(httpClient.DefaultRequestHeaders.TryGetValues("Accept", out var s))
			{
				Assert.Single(s);
				Assert.Equal("application/json", s.First());
			}
			else
			{
				Assert.True(false, "Header not set correctly");
			}
			if(httpClient.DefaultRequestHeaders.TryGetValues("User-Agent", out var h))
			{
				var cs = string.Join(' ', h);
				Assert.Equal("DevicesClient/1.0", cs);
			}
			else
			{
				Assert.True(false, "Header not set correctly");
			}
		}

		/*[Fact]
		public void AddDevicesSDKRegistrationTest()
		{
			var service = new ServiceCollection();
			service.AddDevicesSDKRegistration(new Uri("http://gateway.dev.local"));
			var provider = service.BuildServiceProvider();
			var client = provider.GetService<DevicesClient>();
			Assert.NotNull(client);
			var m = client.GetType().GetRuntimeMethods().First(i => i.Name == "FireGetAuthenticationToken");

			var args = new AuthenticationTokenArgs();
			m.Invoke(client, new[] { args });
			Assert.False(args.CacheToken);
			Assert.Null(args.Token);

			var hd = new HeaderDictionary();
			var mock = new Mock<IHttpContextAccessor>();
			mock.Setup(i => i.HttpContext.Request.Headers).Returns(hd);
			service.AddTransient(i => mock.Object);

			provider = service.BuildServiceProvider();
			client = provider.GetService<DevicesClient>();
			m.Invoke(client, new[] { args });
			Assert.False(args.CacheToken);
			Assert.Null(args.Token);

			hd.Add("Authorization", new Microsoft.Extensions.Primitives.StringValues("Bearer test"));
			m.Invoke(client, new[] { args });
			Assert.True(args.CacheToken);
			Assert.Equal("test", args.Token);
		}*/
	}
}
