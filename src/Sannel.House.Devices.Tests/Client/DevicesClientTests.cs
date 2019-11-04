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
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
//using Sannel.House.Devices.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

//namespace Sannel.House.Devices.Tests.Client
//{
//	public class DevicesClientTests : BaseTests
//	{
//		[Fact]
//		public async Task GetPagedAsyncTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetPaged/0/25"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{

//	""data"": [
//		{
//			""deviceId"": 1,
//			""name"": ""Default Device"",
//			""description"": ""Default Device used for unknown devices"",
//			""displayOrder"": 1,
//			""dateCreated"": ""2019-12-09T21:15:00"",
//			""isReadOnly"": true
//		},
//		{
//			""deviceId"": 2,
//			""name"": ""Default Device 2"",
//			""description"": ""Default Device used for unknown devices"",
//			""displayOrder"": 2,
//			""dateCreated"": ""2019-12-09T21:15:00"",
//			""isReadOnly"": true
//		}
//	],
//	""totalCount"": 2,
//	""page"": 0,
//	""pageSize"": 25
//}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetPagedAsync();
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			Assert.NotNull(result.Data);

//		}

//		[Fact]
//		public async Task GetPagedAsyncBadGatewayTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetPaged/0/25"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.BadGateway,
//					Content = new StringContent("Bad Gateway")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetPagedAsync();
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(HttpStatusCode.BadGateway, result.StatusCode);
//			Assert.Equal("Bad Gateway", result.Title);
//		}

//		[Fact]
//		public async Task GetPagedAsyncExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetPaged/0/25"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{

//	""data"": [
//		{
//			""deviceId"": 1,
//			""name"": ""Default Device"",
//			""description"": ""Default Device used for unknown devices"",
//			""displayOrder"": 1,
//			""dateCreated"": ""2019-12-09T21:15:00"",
//			""isReadOnly"": true
//		},
//		{
//			""deviceId"": 2,
//			""name"": ""Default Device 2"",
//			""description"": ""Default Device used for unknown devices"",
//			""displayOrder"": 2,
//			""dateCreated"": ""2019-12-09T21:15:00"",
//			""isReadOnly"": true
//		}
//	],
//	")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetPagedAsync();
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);
//			Assert.NotNull(result.Exception);
//			Assert.Equal("Exception", result.Title);

//		}

//		[Fact]
//		public async Task GetAsyncTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/5"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{
//	""data"": {

//		""deviceId"": 1,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 1,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true

//	},
//	""status"": 200,
//	""title"": ""The Device""
//}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetDeviceAsync(5);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			Assert.NotNull(result.Data);
//			Assert.Equal(1, result.Data.DeviceId);
//			Assert.Equal("Default Device", result.Data.Name);
//			Assert.Equal("Default Device used for unknown devices", result.Data.Description);
//			Assert.Equal(1, result.Data.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, TimeSpan.Zero), result.Data.DateCreated);
//			Assert.True(result.Data.IsReadOnly, "Device is not marked read only");

//		}

//		[Fact]
//		public async Task GetAsyncExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/5"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{
//	""data"": {

//		""deviceId"": 1,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 1,
//		""dateCreated"": ""2019-12-09T21:15:00"",
//		""isReadOnly"": true

//	},
//	""status"": 200,
//	""title"": ""The Device""
//")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetDeviceAsync(5);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);
//			Assert.NotNull(result.Exception);

//		}
		
//		[Fact]
//		public async Task GetByMacAddressAsyncTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetByMac/123453322"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{
//	""data"": {

//		""deviceId"": 1,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 1,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true

//	},
//	""status"": 200,
//	""title"": ""The Device""
//}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetByMacAddressAsync(123453322);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			Assert.NotNull(result.Data);
//			Assert.Equal(1, result.Data.DeviceId);
//			Assert.Equal("Default Device", result.Data.Name);
//			Assert.Equal("Default Device used for unknown devices", result.Data.Description);
//			Assert.Equal(1, result.Data.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, TimeSpan.Zero), result.Data.DateCreated);
//			Assert.True(result.Data.IsReadOnly, "Device is not marked read only");

//		}

//		[Fact]
//		public async Task GetByMacAddressAsyncExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetByMac/123453322"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{
//	""data"": {

//		""deviceId"": 1,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 1,
//		""dateCreated"": ""2019-12-09T21:15:00"",
//		""isReadOnly"": true

//	},
//	""status"": 200,
//	""title"": ""The Device""
//")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetByMacAddressAsync(123453322);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);
//			Assert.NotNull(result.Exception);

//		}

//		[Fact]
//		public async Task GetByUuidAsyncTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetByUuid/15ed12be-50ba-4041-b11d-1115c1fe74c6"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{
//	""data"": {

//		""deviceId"": 1,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 1,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true

//	},
//	""status"": 200,
//	""title"": ""The Device""
//}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetByUuidAsync(Guid.Parse("15ED12BE-50BA-4041-B11D-1115C1FE74C6"));
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			Assert.NotNull(result.Data);
//			Assert.Equal(1, result.Data.DeviceId);
//			Assert.Equal("Default Device", result.Data.Name);
//			Assert.Equal("Default Device used for unknown devices", result.Data.Description);
//			Assert.Equal(1, result.Data.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, TimeSpan.Zero), result.Data.DateCreated);
//			Assert.True(result.Data.IsReadOnly, "Device is not marked read only");

//		}

//		[Fact]
//		public async Task GetByUuidAsyncExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices/GetByUuid/15ed12be-50ba-4041-b11d-1115c1fe74c6"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{
//	""data"": {

//		""deviceId"": 1,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 1,
//		""dateCreated"": ""2019-12-09T21:15:00"",
//		""isReadOnly"": true

//	},
//	""status"": 200,
//	""title"": ""The Device""
//")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetByUuidAsync(Guid.Parse("15ED12BE-50BA-4041-B11D-1115C1FE74C6"));
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);
//			Assert.NotNull(result.Exception);

//		}

//		[Fact]
//		public async Task GetByManufactureIdAsyncTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			var manufacture = "Particle";
//			var manufactureId = "Photon";

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/Devices/GetByManufactureId/{manufacture}/{manufactureId}"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{
//	""data"": {

//		""deviceId"": 1,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 1,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true

//	},
//	""status"": 200,
//	""title"": ""The Device""
//}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetByManufactureIdAsync(manufacture, manufactureId);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			Assert.NotNull(result.Data);
//			Assert.Equal(1, result.Data.DeviceId);
//			Assert.Equal("Default Device", result.Data.Name);
//			Assert.Equal("Default Device used for unknown devices", result.Data.Description);
//			Assert.Equal(1, result.Data.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, TimeSpan.Zero), result.Data.DateCreated);
//			Assert.True(result.Data.IsReadOnly, "Device is not marked read only");

//		}

//		[Fact]
//		public async Task GetByManufactureIdAsyncExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var manufacture = "Particle";
//			var manufactureId = "Photon";

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/Devices/GetByManufactureId/{manufacture}/{manufactureId}"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{
//	""data"": {

//		""deviceId"": 1,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 1,
//		""dateCreated"": ""2019-12-09T21:15:00"",
//		""isReadOnly"": true

//	},
//	""status"": 200,
//	""title"": ""The Device""
//")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetByManufactureIdAsync(manufacture, manufactureId);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);
//			Assert.NotNull(result.Exception);

//		}

//		[Fact]
//		public async Task AddDeviceAsyncTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Post, r.Method);
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				Assert.Equal(JsonConvert.SerializeObject(device), r.Content.ReadAsStringAsync().Result);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status: 200,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.AddDeviceAsync(device);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			Assert.Equal(5, result.Data);

//		}

//		[Fact]
//		public async Task AddDeviceAsyncExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Post, r.Method);
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				Assert.Equal(JsonConvert.SerializeObject(device), r.Content.ReadAsStringAsync().Result);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status 400,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.AddDeviceAsync(device);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Equal(default(int), result.Data);

//		}

//		[Fact]
//		public async Task UpdateDeviceAsyncTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Put, r.Method);
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				Assert.Equal(JsonConvert.SerializeObject(device), r.Content.ReadAsStringAsync().Result);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status: 200,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.UpdateDeviceAsync(device);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			Assert.Equal(5, result.Data);

//		}

//		[Fact]
//		public async Task UpdateDeviceAsync2Test()
//		{
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object)
//			{
//				BaseAddress = new Uri("https://gateway.dev.local/api/v1/")
//			};

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(httpClient, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Put, r.Method);
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				Assert.Equal(JsonConvert.SerializeObject(device), r.Content.ReadAsStringAsync().Result);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status: 200,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.UpdateDeviceAsync(device);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			Assert.Equal(5, result.Data);

//		}

//		[Fact]
//		public async Task UpdateDeviceAsyncExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Put, r.Method);
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/Devices"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				Assert.Equal(JsonConvert.SerializeObject(device), r.Content.ReadAsStringAsync().Result);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status 400,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.UpdateDeviceAsync(device);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Equal(default(int), result.Data);

//		}

//		[Fact]
//		public async Task GetAlternateIdsAsyncTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Get, r.Method);
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/AlternateId/20"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{
//	""data"": [{

//		""alternateId"": 1,
//		""deviceId"": 20,
//		""dateCreated"": ""2019-12-09T21:15:00"",
//		""macAddress"": 123456780,
//""device"":{
//		""deviceId"": 20,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 1,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true
//}
//	}],
//	""status"": 200,
//	""title"": ""The Device""
//}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetAlernateIdsAsync(20);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			Assert.NotNull(result.Data);
//			Assert.Single(result.Data);
//			var first = result.Data[0];
//			Assert.Equal(123456780, first.MacAddress);
//			Assert.Null(first.Uuid);
//			var d = first.Device;
//			Assert.Equal(20, d.DeviceId);
//			Assert.Equal("Default Device", d.Name);
//			Assert.Equal("Default Device used for unknown devices", d.Description);
//			Assert.Equal(1, d.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, TimeSpan.Zero), d.DateCreated);
//			Assert.True(d.IsReadOnly, "Device is not marked read only");

//		}

//		[Fact]
//		public async Task GetAlternateIdsAsyncExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Get, r.Method);
//				Assert.Equal(new Uri("https://gateway.dev.local/api/v1/AlternateId/20"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);
//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{
//	""data"": {

//		""deviceId"": 1,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 1,
//		""dateCreated"": ""2019-12-09T21:15:00"",
//		""isReadOnly"": true

//	},
//	""status"": 200,
//	""title"": ""The Device""
//")
//				};
//			}).Verifiable();

//			var result = await devicesClient.GetAlernateIdsAsync(20);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);
//			Assert.NotNull(result.Exception);

//		}

//		[Fact]
//		public async Task AddAlternateIdAsyncMacTest()
//		{
//			var macAddress = new byte[]
//			{
//				0xD3,
//				0x25,
//				0x35,
//				0x22,
//				0x6A,
//				0xDB,
//			};

//			var macAddressFixed = new byte[8];
//			Array.Fill<byte>(macAddressFixed, 0);
//			Array.Copy(macAddress, 0, macAddressFixed, 2, 6);
//			Array.Reverse(macAddressFixed);

//			var macAddressLong = 232156758698715;
//			Assert.Equal(macAddressLong, BitConverter.ToInt64(macAddressFixed, 0));
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Post, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/mac/{macAddressLong}/20"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				Assert.Equal("{}", r.Content.ReadAsStringAsync().Result);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{'success':true,status: 200,'Data':
//{
//		""deviceId"": 20,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 2,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true
//}}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.AddAlternateIdAsync(macAddress, 20);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			var d = result.Data;
//			Assert.Equal(20, d.DeviceId);
//			Assert.Equal("Default Device", d.Name);
//			Assert.Equal("Default Device used for unknown devices", d.Description);
//			Assert.Equal(2, d.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0,0, TimeSpan.Zero), d.DateCreated);
//			Assert.True(d.IsReadOnly, "Device is not marked read only");

//			result = await devicesClient.AddAlternateIdAsync(macAddressFixed, 20);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			d = result.Data;
//			Assert.Equal(20, d.DeviceId);
//			Assert.Equal("Default Device", d.Name);
//			Assert.Equal("Default Device used for unknown devices", d.Description);
//			Assert.Equal(2, d.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, 0, TimeSpan.Zero), d.DateCreated);
//			Assert.True(d.IsReadOnly, "Device is not marked read only");
//		}

//		[Fact]
//		public async Task AddAlternateIdMacAddressExceptionsTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;

//			await Assert.ThrowsAsync<ArgumentNullException>("macAddress", () => devicesClient.AddAlternateIdAsync(null, 20));
//			await Assert.ThrowsAsync<ArgumentOutOfRangeException>("macAddress", () => devicesClient.AddAlternateIdAsync(new byte[] { 0x23 }, 20));
//		}

//		[Fact]
//		public async Task AddAlternateIdAsyncMacExceptionTest()
//		{
//			var macAddress = new byte[]
//			{
//				0xD3,
//				0x25,
//				0x35,
//				0x22,
//				0x6A,
//				0xDB,
//			};

//			var macAddressFixed = new byte[8];
//			Array.Fill<byte>(macAddressFixed, 0);
//			Array.Copy(macAddress, 0, macAddressFixed, 2, 6);
//			Array.Reverse(macAddressFixed);

//			var macAddressLong = 232156758698715;
//			Assert.Equal(macAddressLong, BitConverter.ToInt64(macAddressFixed, 0));
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Post, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/mac/{macAddressLong}/20"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status 400,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.AddAlternateIdAsync(macAddress, 20);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);

//		}

//		[Fact]
//		public async Task AddAlternateIdAsyncUuidTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);
//			var uuid = Guid.NewGuid();

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTimeOffset.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Post, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/uuid/{uuid}/20"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				Assert.Equal("{}", r.Content.ReadAsStringAsync().Result);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{'success':true,status: 200,'Data':
//{
//		""deviceId"": 20,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 2,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true
//}}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.AddAlternateIdAsync(uuid, 20);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			var d = result.Data;
//			Assert.Equal(20, d.DeviceId);
//			Assert.Equal("Default Device", d.Name);
//			Assert.Equal("Default Device used for unknown devices", d.Description);
//			Assert.Equal(2, d.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, TimeSpan.Zero), d.DateCreated);
//			Assert.True(d.IsReadOnly, "Device is not marked read only");

//		}

//		[Fact]
//		public async Task AddAlternateIdAsyncUuidExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);
//			var uuid = Guid.NewGuid();

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Post, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/uuid/{uuid}/20"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status 400,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.AddAlternateIdAsync(uuid, 20);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);

//		}
		
//		[Fact]
//		public async Task AddAlternateIdAsyncManufactureIdTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);
//			var manufacture = "Particle";
//			var manufactureId = "Photon";

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTimeOffset.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Post, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/manufactureid/{manufacture}/{manufactureId}/20"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				Assert.Equal("{}", r.Content.ReadAsStringAsync().Result);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{'success':true,status: 200,'Data':
//{
//		""deviceId"": 20,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 2,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true
//}}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.AddAlternateIdAsync(manufacture, manufactureId, 20);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			var d = result.Data;
//			Assert.Equal(20, d.DeviceId);
//			Assert.Equal("Default Device", d.Name);
//			Assert.Equal("Default Device used for unknown devices", d.Description);
//			Assert.Equal(2, d.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, 0, TimeSpan.Zero), d.DateCreated);
//			Assert.True(d.IsReadOnly, "Device is not marked read only");

//		}

//		[Fact]
//		public async Task AddAlternateIdAsyncManufactureIdExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var manufacture = "Particle";
//			var manufactureId = "Photon";

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Post, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/manufactureid/{manufacture}/{manufactureId}/20"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status 400,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.AddAlternateIdAsync(manufacture, manufactureId, 20);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);

//		}

//		[Fact]
//		public async Task DeleteAlternateIdAsyncMacTest()
//		{
//			var macAddress = new byte[]
//			{
//				0xD3,
//				0x25,
//				0x35,
//				0x22,
//				0x6A,
//				0xDB,
//			};

//			var macAddressFixed = new byte[8];
//			Array.Fill<byte>(macAddressFixed, 0);
//			Array.Copy(macAddress, 0, macAddressFixed, 2, 6);
//			Array.Reverse(macAddressFixed);

//			var macAddressLong = 232156758698715;
//			Assert.Equal(macAddressLong, BitConverter.ToInt64(macAddressFixed, 0));
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTimeOffset.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Delete, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/mac/{macAddressLong}"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{'success':true,status: 200,'Data':
//{
//		""deviceId"": 20,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 2,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true
//}}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.DeleteAlternateIdAsync(macAddress);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			var d = result.Data;
//			Assert.Equal(20, d.DeviceId);
//			Assert.Equal("Default Device", d.Name);
//			Assert.Equal("Default Device used for unknown devices", d.Description);
//			Assert.Equal(2, d.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, TimeSpan.Zero), d.DateCreated);
//			Assert.True(d.IsReadOnly, "Device is not marked read only");

//			result = await devicesClient.DeleteAlternateIdAsync(macAddressFixed);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			d = result.Data;
//			Assert.Equal(20, d.DeviceId);
//			Assert.Equal("Default Device", d.Name);
//			Assert.Equal("Default Device used for unknown devices", d.Description);
//			Assert.Equal(2, d.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, TimeSpan.Zero), d.DateCreated);
//			Assert.True(d.IsReadOnly, "Device is not marked read only");
//		}

//		[Fact]
//		public async Task DeleteAlternateIdMacAddressExceptionsTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;

//			await Assert.ThrowsAsync<ArgumentNullException>("macAddress", () => devicesClient.DeleteAlternateIdAsync(null));
//			await Assert.ThrowsAsync<ArgumentOutOfRangeException>("macAddress", () => devicesClient.DeleteAlternateIdAsync(new byte[] { 0x23 }));
//		}

//		[Fact]
//		public async Task DeleteAlternateIdAsyncMacExceptionTest()
//		{
//			var macAddress = new byte[]
//			{
//				0xD3,
//				0x25,
//				0x35,
//				0x22,
//				0x6A,
//				0xDB,
//			};

//			var macAddressFixed = new byte[8];
//			Array.Fill<byte>(macAddressFixed, 0);
//			Array.Copy(macAddress, 0, macAddressFixed, 2, 6);
//			Array.Reverse(macAddressFixed);

//			var macAddressLong = 232156758698715;
//			Assert.Equal(macAddressLong, BitConverter.ToInt64(macAddressFixed, 0));
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Delete, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/mac/{macAddressLong}"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status 400,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.DeleteAlternateIdAsync(macAddress);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);

//		}

//		[Fact]
//		public async Task DeleteAlternateIdAsyncUuidTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);
//			var uuid = Guid.NewGuid();

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTimeOffset.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Delete, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/uuid/{uuid}"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{'success':true,status: 200,'Data':
//{
//		""deviceId"": 20,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 2,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true
//}}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.DeleteAlternateIdAsync(uuid);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			var d = result.Data;
//			Assert.Equal(20, d.DeviceId);
//			Assert.Equal("Default Device", d.Name);
//			Assert.Equal("Default Device used for unknown devices", d.Description);
//			Assert.Equal(2, d.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, TimeSpan.Zero), d.DateCreated);
//			Assert.True(d.IsReadOnly, "Device is not marked read only");

//		}

//		[Fact]
//		public async Task DeleteAlternateIdAsyncUuidExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);
//			var uuid = Guid.NewGuid();

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Delete, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/uuid/{uuid}"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status 400,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.DeleteAlternateIdAsync(uuid);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);

//		}

//		[Fact]
//		public async Task DeleteAlternateIdAsyncManufactureIdTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var manufacture = "Particle";
//			var manufactureId = "Photon";

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTimeOffset.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Delete, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/manufacture/{manufacture}/{manufactureId}"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent(@"{'success':true,status: 200,'Data':
//{
//		""deviceId"": 20,
//		""name"": ""Default Device"",
//		""description"": ""Default Device used for unknown devices"",
//		""displayOrder"": 2,
//		""dateCreated"": ""2019-12-09T21:15:00+00:00"",
//		""isReadOnly"": true
//}}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.DeleteAlternateIdAsync(manufacture, manufactureId);
//			Assert.NotNull(result);
//			Assert.True(result.Success);
//			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//			var d = result.Data;
//			Assert.Equal(20, d.DeviceId);
//			Assert.Equal("Default Device", d.Name);
//			Assert.Equal("Default Device used for unknown devices", d.Description);
//			Assert.Equal(2, d.DisplayOrder);
//			Assert.Equal(new DateTimeOffset(2019, 12, 09, 21, 15, 0, TimeSpan.Zero), d.DateCreated);
//			Assert.True(d.IsReadOnly, "Device is not marked read only");

//		}

//		[Fact]
//		public async Task DeleteAlternateIdAsyncManufacuteIdExceptionTest()
//		{
//			var clientFactory = new Mock<IHttpClientFactory>();
//			var client = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//			var httpClient = new HttpClient(client.Object);
//			httpClient.BaseAddress = new Uri("https://gateway.dev.local/api/v1/");
//			clientFactory.Setup(i => i.CreateClient(nameof(DevicesClient))).Returns(httpClient);

//			var manufacture = "Particle";
//			var manufactureId = "Photon";

//			var device = new Device()
//			{
//				DeviceId = 20,
//				Name = "Name Device",
//				Description = "Test Description",
//				IsReadOnly = true,
//				DateCreated = DateTime.UtcNow,
//				DisplayOrder = 20
//			};

//			var devicesClient = new DevicesClient(clientFactory.Object, new Uri("https://gateway.dev.local"), CreateLogger<DevicesClient>()) ;
//			var token = "test123";
//			devicesClient.AuthToken = token;

//			client.Protected().Setup<Task<HttpResponseMessage>>(
//				"SendAsync",
//				ItExpr.IsAny<HttpRequestMessage>(),
//				ItExpr.IsAny<CancellationToken>()
//			).ReturnsAsync((HttpRequestMessage r,CancellationToken c) =>
//			{
//				Assert.Equal(HttpMethod.Delete, r.Method);
//				Assert.Equal(new Uri($"https://gateway.dev.local/api/v1/AlternateId/manufacture/{manufacture}/{manufactureId}"), r.RequestUri);
//				Assert.NotNull(r.Headers.Authorization);
//				Assert.Equal("Bearer", r.Headers.Authorization.Scheme);
//				Assert.Equal(token, r.Headers.Authorization.Parameter);

//				return new HttpResponseMessage()
//				{
//					StatusCode = System.Net.HttpStatusCode.OK,
//					Content = new StringContent("{ 'success':true,status 400,'Data':5}")
//				};
//			}).Verifiable();

//			var result = await devicesClient.DeleteAlternateIdAsync(manufacture, manufactureId);
//			Assert.NotNull(result);
//			Assert.False(result.Success);
//			Assert.Equal(444, result.Status);
//			Assert.Null(result.Data);

//		}
//	}
//}
