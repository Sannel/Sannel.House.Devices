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
using Microsoft.EntityFrameworkCore;
using Sannel.House.Devices.Models;
using Sannel.House.Devices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sannel.House.Devices.Tests.Repositories
{
	public class DbContextRepositoryTests : BaseTests
	{
		[Fact]
		public void CreateNullTest()
		{
			Assert.Throws<ArgumentNullException>("context",() =>
			{
				new DbContextRepository(null);
			});
		}

		[Fact]
		public async Task GetDevicesListAsyncTest()
		{
			using (var connection = OpenConnection())
			{
				using (var context = GetTestDB(connection))
				{
					var device1 = new Device()
					{
						DeviceId = 1,
						Name = "Test Name",
						IsReadOnly = true,
						Description = "Dest Description",
						DateCreated = DateTime.Now,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = true,
						Description = "Test 2 Description",
						DateCreated = DateTime.Now.AddDays(-2),
						DisplayOrder = 1
					};
					var device3 = new Device()
					{
						DeviceId = 3,
						Name = "Test 3 Name",
						IsReadOnly = true,
						Description = "Test 3 Description",
						DateCreated = DateTime.Now.AddDays(-1),
						DisplayOrder = 3
					};
					await context.AddRangeAsync(device1, device2, device3);
					await context.SaveChangesAsync();

					var repository = new DbContextRepository(context);
					var result = await repository.GetDevicesListAsync(1, 3);
					Assert.NotNull(result);
					Assert.NotEmpty(result.Data);
					Assert.Equal(3, result.TotalCount);
					Assert.Equal(1, result.Page);
					Assert.Equal(3, result.PageSize);
					var list = result.Data.ToList();
					var device = list[0];
					AssertEqual(device2, device);
					device = list[1];
					AssertEqual(device1, device);
					device = list[2];
					AssertEqual(device3, device);

					result = await repository.GetDevicesListAsync(1, 2);
					Assert.NotNull(result);
					Assert.NotEmpty(result.Data);
					Assert.Equal(2, result.Data.Count());
					Assert.Equal(3, result.TotalCount);
					Assert.Equal(1, result.Page);
					Assert.Equal(2, result.PageSize);
					list = result.Data.ToList();
					device = list[0];
					AssertEqual(device2, device);
					device = list[1];
					AssertEqual(device1, device);

					result = await repository.GetDevicesListAsync(2, 2);
					Assert.NotNull(result);
					Assert.NotEmpty(result.Data);
					Assert.Single(result.Data);
					Assert.Equal(3, result.TotalCount);
					Assert.Equal(2, result.Page);
					Assert.Equal(2, result.PageSize);
					list = result.Data.ToList();
					device = list[0];
					AssertEqual(device3, device);

				}
			}
		}

		[Fact]
		public async Task GetDeviceByIdAsyncTest()
		{
			using (var connection = OpenConnection())
			{
				using (var context = GetTestDB(connection))
				{
					var device1 = new Device()
					{
						DeviceId = 1,
						Name = "Test Name",
						IsReadOnly = true,
						Description = "Dest Description",
						DateCreated = DateTime.Now,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = true,
						Description = "Test 2 Description",
						DateCreated = DateTime.Now.AddDays(-2),
						DisplayOrder = 1
					};

					await context.Devices.AddRangeAsync(device1, device2);
					await context.SaveChangesAsync();

					var repository = new DbContextRepository(context);
					var actual = await repository.GetDeviceByIdAsync(device1.DeviceId);
					Assert.NotNull(actual);
					AssertEqual(device1, actual);

					actual = await repository.GetDeviceByIdAsync(50);
					Assert.Null(actual);
				}
			}
		}

		[Fact]
		public async Task GetDeviceByMacAddressAsyncTest()
		{
			using (var connection = OpenConnection())
			{
				using (var context = GetTestDB(connection))
				{
					var device1 = new Device()
					{
						DeviceId = 1,
						Name = "Test Name",
						IsReadOnly = true,
						Description = "Dest Description",
						DateCreated = DateTime.Now,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = true,
						Description = "Test 2 Description",
						DateCreated = DateTime.Now.AddDays(-2),
						DisplayOrder = 1
					};

					await context.Devices.AddRangeAsync(device1, device2);

					var altId1 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						DateCreated = DateTime.Now,
						MacAddress = 0x3223C00DF70D
					};
					var altId2 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						DateCreated = DateTime.Now,
						MacAddress = 0x18CCB5BC7ACD
					};
					var altId3 = new AlternateDeviceId()
					{
						DeviceId = device2.DeviceId,
						DateCreated = DateTime.Now,
						MacAddress = 0xC5275F245FA3
					};

					await context.AlternateDeviceIds.AddRangeAsync(altId1, altId2, altId3);
					await context.SaveChangesAsync();

					var repository = new DbContextRepository(context);
					var actual = await repository.GetDeviceByMacAddressAsync(altId1.MacAddress.Value);
					Assert.NotNull(actual);
					AssertEqual(device1, actual);

					actual = await repository.GetDeviceByMacAddressAsync(altId2.MacAddress.Value);
					Assert.NotNull(actual);
					AssertEqual(device1, actual);

					actual = await repository.GetDeviceByMacAddressAsync(altId3.MacAddress.Value);
					Assert.NotNull(actual);
					AssertEqual(device2, actual);

					actual = await repository.GetDeviceByMacAddressAsync(0x0FE03C8431F7);
					Assert.Null(actual);
				}
			}
		}

		[Fact]
		public async Task GetDeviceByUuidAsyncTest()
		{
			using (var connection = OpenConnection())
			{
				using (var context = GetTestDB(connection))
				{
					var device1 = new Device()
					{
						DeviceId = 1,
						Name = "Test Name",
						IsReadOnly = true,
						Description = "Dest Description",
						DateCreated = DateTime.Now,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = true,
						Description = "Test 2 Description",
						DateCreated = DateTime.Now.AddDays(-2),
						DisplayOrder = 1
					};

					await context.Devices.AddRangeAsync(device1, device2);

					var altId1 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						DateCreated = DateTime.Now,
						Uuid = Guid.NewGuid()
					};
					var altId2 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						DateCreated = DateTime.Now,
						Uuid = Guid.NewGuid()
					};
					var altId3 = new AlternateDeviceId()
					{
						DeviceId = device2.DeviceId,
						DateCreated = DateTime.Now,
						Uuid = Guid.NewGuid()
					};

					await context.AlternateDeviceIds.AddRangeAsync(altId1, altId2, altId3);
					await context.SaveChangesAsync();

					var repository = new DbContextRepository(context);
					var actual = await repository.GetDeviceByUuidAsync(altId1.Uuid.Value);
					Assert.NotNull(actual);
					AssertEqual(device1, actual);

					actual = await repository.GetDeviceByUuidAsync(altId2.Uuid.Value);
					Assert.NotNull(actual);
					AssertEqual(device1, actual);

					actual = await repository.GetDeviceByUuidAsync(altId3.Uuid.Value);
					Assert.NotNull(actual);
					AssertEqual(device2, actual);

					actual = await repository.GetDeviceByUuidAsync(Guid.Empty);
					Assert.Null(actual);
				}
			}
		}
	}
}
