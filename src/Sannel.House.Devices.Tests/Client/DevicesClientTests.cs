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
using Moq;
using Moq.Protected;
using Sannel.House.Devices.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sannel.House.Devices.Tests.Client
{
	public class DevicesClientTests
	{
		[Fact]
		public async Task GetPagedAsyncTest()
		{
			var clientFactory = new Mock<IHttpClientFactory>();
			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			var httpClient = new HttpClient(client.Object);
			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

			var devicesClient = new DevicesClient(clientFactory.Object);
			var token = "test123";
			devicesClient.GetAuthenticationToken += (s, args) =>
			{
				Assert.NotNull(args);
				args.CacheToken = true;
				args.Token = token;
			};

			client.Protected().Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
			{
				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetPaged/0/25"), r.RequestUri);
				Assert.NotNull(r.Headers.Authorization);
				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
				Assert.Equal(token, r.Headers.Authorization.Parameter);
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(@"{

	""data"": [
		{
			""deviceId"": 1,
			""name"": ""Default Device"",
			""description"": ""Default Device used for unknown devices"",
			""displayOrder"": 1,
			""dateCreated"": ""2018-12-09T21:15:00"",
			""isReadOnly"": true
		},
		{
			""deviceId"": 2,
			""name"": ""Default Device 2"",
			""description"": ""Default Device used for unknown devices"",
			""displayOrder"": 2,
			""dateCreated"": ""2018-12-09T21:15:00"",
			""isReadOnly"": true
		}
	],
	""totalCount"": 2,
	""page"": 0,
	""pageSize"": 25
}")
				};
			}).Verifiable();

			var result = await devicesClient.GetPagedAsync();
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
			Assert.NotNull(result.Data);

		}

		[Fact]
		public async Task GetPagedAsyncExceptionTest()
		{
			var clientFactory = new Mock<IHttpClientFactory>();
			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			var httpClient = new HttpClient(client.Object);
			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

			var devicesClient = new DevicesClient(clientFactory.Object);
			var token = "test123";
			devicesClient.GetAuthenticationToken += (s, args) =>
			{
				Assert.NotNull(args);
				args.CacheToken = true;
				args.Token = token;
			};

			client.Protected().Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
			{
				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetPaged/0/25"), r.RequestUri);
				Assert.NotNull(r.Headers.Authorization);
				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
				Assert.Equal(token, r.Headers.Authorization.Parameter);
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(@"{

	""data"": [
		{
			""deviceId"": 1,
			""name"": ""Default Device"",
			""description"": ""Default Device used for unknown devices"",
			""displayOrder"": 1,
			""dateCreated"": ""2018-12-09T21:15:00"",
			""isReadOnly"": true
		},
		{
			""deviceId"": 2,
			""name"": ""Default Device 2"",
			""description"": ""Default Device used for unknown devices"",
			""displayOrder"": 2,
			""dateCreated"": ""2018-12-09T21:15:00"",
			""isReadOnly"": true
		}
	],
	")
				};
			}).Verifiable();

			var result = await devicesClient.GetPagedAsync();
			Assert.NotNull(result);
			Assert.False(result.Success);
			Assert.Equal(444, result.Status);
			Assert.Null(result.Data);
			Assert.NotNull(result.Exception);
			Assert.Equal("Exception", result.Title);

		}

		[Fact]
		public async Task GetAsyncTest()
		{
			var clientFactory = new Mock<IHttpClientFactory>();
			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			var httpClient = new HttpClient(client.Object);
			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

			var devicesClient = new DevicesClient(clientFactory.Object);
			var token = "test123";
			devicesClient.GetAuthenticationToken += (s, args) =>
			{
				Assert.NotNull(args);
				args.CacheToken = true;
				args.Token = token;
			};

			client.Protected().Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
			{
				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/5"), r.RequestUri);
				Assert.NotNull(r.Headers.Authorization);
				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
				Assert.Equal(token, r.Headers.Authorization.Parameter);
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(@"{
	""data"": {

		""deviceId"": 1,
		""name"": ""Default Device"",
		""description"": ""Default Device used for unknown devices"",
		""displayOrder"": 1,
		""dateCreated"": ""2018-12-09T21:15:00"",
		""isReadOnly"": true

	},
	""status"": 200,
	""title"": ""The Device""
}")
				};
			}).Verifiable();

			var result = await devicesClient.GetAsync(5);
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
			Assert.NotNull(result.Data);
			Assert.Equal(1, result.Data.DeviceId);
			Assert.Equal("Default Device", result.Data.Name);
			Assert.Equal("Default Device used for unknown devices", result.Data.Description);
			Assert.Equal(1, result.Data.DisplayOrder);
			Assert.Equal(new DateTime(2018, 12, 09, 21, 15, 0, DateTimeKind.Utc), result.Data.DateCreated);
			Assert.True(result.Data.IsReadOnly, "Device is not makred read only");

		}

		[Fact]
		public async Task GetAsyncExceptionTest()
		{
			var clientFactory = new Mock<IHttpClientFactory>();
			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			var httpClient = new HttpClient(client.Object);
			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

			var devicesClient = new DevicesClient(clientFactory.Object);
			var token = "test123";
			devicesClient.GetAuthenticationToken += (s, args) =>
			{
				Assert.NotNull(args);
				args.CacheToken = true;
				args.Token = token;
			};

			client.Protected().Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
			{
				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/5"), r.RequestUri);
				Assert.NotNull(r.Headers.Authorization);
				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
				Assert.Equal(token, r.Headers.Authorization.Parameter);
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(@"{
	""data"": {

		""deviceId"": 1,
		""name"": ""Default Device"",
		""description"": ""Default Device used for unknown devices"",
		""displayOrder"": 1,
		""dateCreated"": ""2018-12-09T21:15:00"",
		""isReadOnly"": true

	},
	""status"": 200,
	""title"": ""The Device""
")
				};
			}).Verifiable();

			var result = await devicesClient.GetAsync(5);
			Assert.NotNull(result);
			Assert.False(result.Success);
			Assert.Equal(444, result.Status);
			Assert.Null(result.Data);
			Assert.NotNull(result.Exception);

		}
		
		[Fact]
		public async Task GetByMacAddressAsyncTest()
		{
			var clientFactory = new Mock<IHttpClientFactory>();
			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			var httpClient = new HttpClient(client.Object);
			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

			var devicesClient = new DevicesClient(clientFactory.Object);
			var token = "test123";
			devicesClient.GetAuthenticationToken += (s, args) =>
			{
				Assert.NotNull(args);
				args.CacheToken = true;
				args.Token = token;
			};

			client.Protected().Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
			{
				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetByMac/123453322"), r.RequestUri);
				Assert.NotNull(r.Headers.Authorization);
				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
				Assert.Equal(token, r.Headers.Authorization.Parameter);
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(@"{
	""data"": {

		""deviceId"": 1,
		""name"": ""Default Device"",
		""description"": ""Default Device used for unknown devices"",
		""displayOrder"": 1,
		""dateCreated"": ""2018-12-09T21:15:00"",
		""isReadOnly"": true

	},
	""status"": 200,
	""title"": ""The Device""
}")
				};
			}).Verifiable();

			var result = await devicesClient.GetByMacAddressAsync(123453322);
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
			Assert.NotNull(result.Data);
			Assert.Equal(1, result.Data.DeviceId);
			Assert.Equal("Default Device", result.Data.Name);
			Assert.Equal("Default Device used for unknown devices", result.Data.Description);
			Assert.Equal(1, result.Data.DisplayOrder);
			Assert.Equal(new DateTime(2018, 12, 09, 21, 15, 0, DateTimeKind.Utc), result.Data.DateCreated);
			Assert.True(result.Data.IsReadOnly, "Device is not makred read only");

		}

		[Fact]
		public async Task GetByMacAddressAsyncExceptionTest()
		{
			var clientFactory = new Mock<IHttpClientFactory>();
			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			var httpClient = new HttpClient(client.Object);
			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

			var devicesClient = new DevicesClient(clientFactory.Object);
			var token = "test123";
			devicesClient.GetAuthenticationToken += (s, args) =>
			{
				Assert.NotNull(args);
				args.CacheToken = true;
				args.Token = token;
			};

			client.Protected().Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
			{
				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetByMac/123453322"), r.RequestUri);
				Assert.NotNull(r.Headers.Authorization);
				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
				Assert.Equal(token, r.Headers.Authorization.Parameter);
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(@"{
	""data"": {

		""deviceId"": 1,
		""name"": ""Default Device"",
		""description"": ""Default Device used for unknown devices"",
		""displayOrder"": 1,
		""dateCreated"": ""2018-12-09T21:15:00"",
		""isReadOnly"": true

	},
	""status"": 200,
	""title"": ""The Device""
")
				};
			}).Verifiable();

			var result = await devicesClient.GetByMacAddressAsync(123453322);
			Assert.NotNull(result);
			Assert.False(result.Success);
			Assert.Equal(444, result.Status);
			Assert.Null(result.Data);
			Assert.NotNull(result.Exception);

		}

		[Fact]
		public async Task GetByUuidAsyncTest()
		{
			var clientFactory = new Mock<IHttpClientFactory>();
			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			var httpClient = new HttpClient(client.Object);
			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

			var devicesClient = new DevicesClient(clientFactory.Object);
			var token = "test123";
			devicesClient.GetAuthenticationToken += (s, args) =>
			{
				Assert.NotNull(args);
				args.CacheToken = true;
				args.Token = token;
			};

			client.Protected().Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
			{
				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetByUuid/15ed12be-50ba-4041-b11d-1115c1fe74c6"), r.RequestUri);
				Assert.NotNull(r.Headers.Authorization);
				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
				Assert.Equal(token, r.Headers.Authorization.Parameter);
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(@"{
	""data"": {

		""deviceId"": 1,
		""name"": ""Default Device"",
		""description"": ""Default Device used for unknown devices"",
		""displayOrder"": 1,
		""dateCreated"": ""2018-12-09T21:15:00"",
		""isReadOnly"": true

	},
	""status"": 200,
	""title"": ""The Device""
}")
				};
			}).Verifiable();

			var result = await devicesClient.GetByUuidAsync(Guid.Parse("15ED12BE-50BA-4041-B11D-1115C1FE74C6"));
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
			Assert.NotNull(result.Data);
			Assert.Equal(1, result.Data.DeviceId);
			Assert.Equal("Default Device", result.Data.Name);
			Assert.Equal("Default Device used for unknown devices", result.Data.Description);
			Assert.Equal(1, result.Data.DisplayOrder);
			Assert.Equal(new DateTime(2018, 12, 09, 21, 15, 0, DateTimeKind.Utc), result.Data.DateCreated);
			Assert.True(result.Data.IsReadOnly, "Device is not makred read only");

		}

		[Fact]
		public async Task GetByUuidAsyncExceptionTest()
		{
			var clientFactory = new Mock<IHttpClientFactory>();
			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			var httpClient = new HttpClient(client.Object);
			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

			var devicesClient = new DevicesClient(clientFactory.Object);
			var token = "test123";
			devicesClient.GetAuthenticationToken += (s, args) =>
			{
				Assert.NotNull(args);
				args.CacheToken = true;
				args.Token = token;
			};

			client.Protected().Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
			{
				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetByUuid/15ed12be-50ba-4041-b11d-1115c1fe74c6"), r.RequestUri);
				Assert.NotNull(r.Headers.Authorization);
				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
				Assert.Equal(token, r.Headers.Authorization.Parameter);
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(@"{
	""data"": {

		""deviceId"": 1,
		""name"": ""Default Device"",
		""description"": ""Default Device used for unknown devices"",
		""displayOrder"": 1,
		""dateCreated"": ""2018-12-09T21:15:00"",
		""isReadOnly"": true

	},
	""status"": 200,
	""title"": ""The Device""
")
				};
			}).Verifiable();

			var result = await devicesClient.GetByUuidAsync(Guid.Parse("15ED12BE-50BA-4041-B11D-1115C1FE74C6"));
			Assert.NotNull(result);
			Assert.False(result.Success);
			Assert.Equal(444, result.Status);
			Assert.Null(result.Data);
			Assert.NotNull(result.Exception);

		}

	}
}
