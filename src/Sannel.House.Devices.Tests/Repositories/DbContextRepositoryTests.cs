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
using System.Data;
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
						DateCreated = DateTime.UtcNow,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = true,
						Description = "Test 2 Description",
						DateCreated = DateTime.UtcNow.AddDays(-2),
						DisplayOrder = 1
					};
					var device3 = new Device()
					{
						DeviceId = 3,
						Name = "Test 3 Name",
						IsReadOnly = true,
						Description = "Test 3 Description",
						DateCreated = DateTime.UtcNow.AddDays(-1),
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
						DateCreated = DateTime.UtcNow,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = true,
						Description = "Test 2 Description",
						DateCreated = DateTime.UtcNow.AddDays(-2),
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
						DateCreated = DateTime.UtcNow,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = true,
						Description = "Test 2 Description",
						DateCreated = DateTime.UtcNow.AddDays(-2),
						DisplayOrder = 1
					};

					await context.Devices.AddRangeAsync(device1, device2);

					var altId1 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						DateCreated = DateTime.UtcNow,
						MacAddress = 0x3223C00DF70D
					};
					var altId2 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						DateCreated = DateTime.UtcNow,
						MacAddress = 0x18CCB5BC7ACD
					};
					var altId3 = new AlternateDeviceId()
					{
						DeviceId = device2.DeviceId,
						DateCreated = DateTime.UtcNow,
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
						DateCreated = DateTime.UtcNow,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = true,
						Description = "Test 2 Description",
						DateCreated = DateTime.UtcNow.AddDays(-2),
						DisplayOrder = 1
					};

					await context.Devices.AddRangeAsync(device1, device2);

					var altId1 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						DateCreated = DateTime.UtcNow,
						Uuid = Guid.NewGuid()
					};
					var altId2 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						DateCreated = DateTime.UtcNow,
						Uuid = Guid.NewGuid()
					};
					var altId3 = new AlternateDeviceId()
					{
						DeviceId = device2.DeviceId,
						DateCreated = DateTime.UtcNow,
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

		[Fact]
		public async Task CreateDeviceAsyncTest()
		{

			using (var connection = OpenConnection())
			{
				using (var context = GetTestDB(connection))
				{
					var repo = new DbContextRepository(context);
					await Assert.ThrowsAsync<ArgumentNullException>("device", () => repo.AddDeviceAsync(null));

					var device1 = new Device()
					{
						DeviceId = 1,
						Name = "Test Name",
						IsReadOnly = true,
						Description = "Dest Description",
						DateCreated = DateTime.UtcNow
					};

					var actualDevice = await repo.AddDeviceAsync(device1);
					Assert.NotNull(actualDevice);
					Assert.True(actualDevice.DeviceId > 0);
					Assert.Equal(device1.Name, actualDevice.Name);
					Assert.Equal(device1.IsReadOnly, actualDevice.IsReadOnly);
					Assert.Equal(device1.Description, actualDevice.Description);
					Assert.Equal(device1.DateCreated, actualDevice.DateCreated);
					Assert.Equal(1, device1.DisplayOrder);

					var device2 = new Device()
					{
						Name = "Test 2",
						IsReadOnly = false,
						Description = "Device Description 2",
						DateCreated = DateTime.UtcNow,
					};


					actualDevice = await repo.AddDeviceAsync(device2);
					Assert.NotNull(actualDevice);
					Assert.True(actualDevice.DeviceId > 1);
					Assert.Equal(device2.Name, actualDevice.Name);
					Assert.Equal(device2.IsReadOnly, actualDevice.IsReadOnly);
					Assert.Equal(device2.Description, actualDevice.Description);
					Assert.Equal(device2.DateCreated, actualDevice.DateCreated);
					Assert.True(device2.DisplayOrder > device1.DisplayOrder);


					actualDevice = await context.Devices.FirstOrDefaultAsync(i => i.DeviceId == actualDevice.DeviceId);
					Assert.NotNull(actualDevice);
					Assert.True(actualDevice.DeviceId > 1);
					Assert.Equal(device2.Name, actualDevice.Name);
					Assert.Equal(device2.IsReadOnly, actualDevice.IsReadOnly);
					Assert.Equal(device2.Description, actualDevice.Description);
					Assert.Equal(device2.DateCreated, actualDevice.DateCreated);
					Assert.True(device2.DisplayOrder > device1.DisplayOrder);
				}
			}
		}
		
		[Fact]
		public async Task UpdateDeviceAsyncTest()
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
						DateCreated = DateTime.UtcNow,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = false,
						Description = "Test 2 Description",
						DateCreated = DateTime.UtcNow.AddDays(-2),
						DisplayOrder = 1
					};
					await context.Devices.AddRangeAsync(device1, device2);
					await context.SaveChangesAsync();
					context.Entry(device2).State = EntityState.Detached;


					var repo = new DbContextRepository(context);
					await Assert.ThrowsAsync<ArgumentNullException>("device", 
						() => repo.UpdateDeviceAsync(null));

					await Assert.ThrowsAsync<ReadOnlyException>(() => repo.UpdateDeviceAsync(device1));

					var orginialDT = device2.DateCreated;
					device2.Name = "Updated Device 2 Name";
					device2.Description = "Updated Description";
					device2.DisplayOrder = 3;
					device2.DateCreated = DateTime.MinValue.AddDays(10);

					var actual = await repo.UpdateDeviceAsync(device2);
					Assert.NotNull(actual);
					Assert.Equal(orginialDT, actual.DateCreated);
					device2.DateCreated = orginialDT;
					AssertEqual(device2, actual);

					// an unknown device attempt to update.
					Assert.Null(await repo.UpdateDeviceAsync(new Device
					{
						DeviceId = 500000
					}));
				}
			}
		}
		[Fact]
		public async Task AddAlternateMacAddressAsyncTest()
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
						DateCreated = DateTime.UtcNow,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = false,
						Description = "Test 2 Description",
						DateCreated = DateTime.UtcNow.AddDays(-2),
						DisplayOrder = 1
					};
					await context.Devices.AddRangeAsync(device1, device2);
					await context.SaveChangesAsync();

					var id1 = 0x3223C00DF70D;

					var repo = new DbContextRepository(context);

					var actual = await repo.AddAlternateMacAddressAsync(-1, id1);
					Assert.Null(actual); // No device with -1 id

					actual = await repo.AddAlternateMacAddressAsync(device1.DeviceId, id1);
					Assert.NotNull(actual);
					AssertEqual(device1, actual);

					var actualAltId = await context.AlternateDeviceIds.FirstOrDefaultAsync(i =>
										i.DeviceId == device1.DeviceId
										&& i.MacAddress == id1);
					Assert.NotNull(actualAltId);
					Assert.True(actualAltId.DateCreated > default(DateTime) 
						&& actualAltId.DateCreated < DateTime.UtcNow, "Suspect Date time was not set correctly");

					await Assert.ThrowsAsync<AlternateDeviceIdException>(() => repo.AddAlternateMacAddressAsync(device1.DeviceId, id1));

				}
			}
		}

		[Fact]
		public async Task AddAlternateUuidAsyncTest()
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
						DateCreated = DateTime.UtcNow,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = false,
						Description = "Test 2 Description",
						DateCreated = DateTime.UtcNow.AddDays(-2),
						DisplayOrder = 1
					};
					await context.Devices.AddRangeAsync(device1, device2);
					await context.SaveChangesAsync();

					var id1 = Guid.NewGuid();

					var repo = new DbContextRepository(context);

					var actual = await repo.AddAlternateUuidAsync(-1, id1);
					Assert.Null(actual); // No device with -1 id

					actual = await repo.AddAlternateUuidAsync(device1.DeviceId, id1);
					Assert.NotNull(actual);
					AssertEqual(device1, actual);

					var actualAltId = await context.AlternateDeviceIds.FirstOrDefaultAsync(i =>
										i.DeviceId == device1.DeviceId
										&& i.Uuid == id1);
					Assert.NotNull(actualAltId);
					Assert.True(actualAltId.DateCreated > default(DateTime) 
						&& actualAltId.DateCreated < DateTime.UtcNow, "Suspect Date time was not set correctly");

					await Assert.ThrowsAsync<AlternateDeviceIdException>(() => repo.AddAlternateUuidAsync(device1.DeviceId, id1));

				}
			}
		}

		[Fact]
		public async Task RemoveAlternateMacAddressAsyncTest()
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
						DateCreated = DateTime.UtcNow,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = false,
						Description = "Test 2 Description",
						DateCreated = DateTime.UtcNow.AddDays(-2),
						DisplayOrder = 1
					};

					await context.Devices.AddRangeAsync(device1, device2);
					await context.SaveChangesAsync();

					var altId1 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						MacAddress = 0x3223C00DF70D,
						DateCreated = DateTime.UtcNow
					};
					var altId2 = new AlternateDeviceId
					{
						DeviceId = device1.DeviceId,
						MacAddress = 0x3223C00DF71D,
						DateCreated = DateTime.UtcNow
					};

					await context.AlternateDeviceIds.AddRangeAsync(altId1, altId2);
					await context.SaveChangesAsync();
					context.Entry(altId1).State = EntityState.Detached;
					context.Entry(altId2).State = EntityState.Detached;

					var repo = new DbContextRepository(context);

					var actual = await repo.RemoveAlternateMacAddressAsync(0x000023300);
					Assert.Null(actual);

					actual = await repo.RemoveAlternateMacAddressAsync(altId1.MacAddress.Value);
					Assert.NotNull(actual);
					AssertEqual(device1, actual);

					Assert.Null(context.AlternateDeviceIds.FirstOrDefault(i => i.MacAddress == altId1.MacAddress));
					Assert.NotNull(context.AlternateDeviceIds.FirstOrDefault(i => i.MacAddress == altId2.MacAddress));
				}
			}
		}
		
		[Fact]
		public async Task RemoveAlternateUuidAsyncTest()
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
						DateCreated = DateTime.UtcNow,
						DisplayOrder = 2
					};
					var device2 = new Device()
					{
						DeviceId = 2,
						Name = "Test 2 Name",
						IsReadOnly = false,
						Description = "Test 2 Description",
						DateCreated = DateTime.UtcNow.AddDays(-2),
						DisplayOrder = 1
					};

					await context.Devices.AddRangeAsync(device1, device2);
					await context.SaveChangesAsync();

					var altId1 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						Uuid = Guid.NewGuid(),
						DateCreated = DateTime.UtcNow
					};
					var altId2 = new AlternateDeviceId
					{
						DeviceId = device1.DeviceId,
						Uuid = Guid.NewGuid(),
						DateCreated = DateTime.UtcNow
					};

					await context.AlternateDeviceIds.AddRangeAsync(altId1, altId2);
					await context.SaveChangesAsync();
					context.Entry(altId1).State = EntityState.Detached;
					context.Entry(altId2).State = EntityState.Detached;

					var repo = new DbContextRepository(context);

					var actual = await repo.RemoveAlternateUuidAsync(Guid.NewGuid());
					Assert.Null(actual);

					actual = await repo.RemoveAlternateUuidAsync(altId1.Uuid.Value);
					Assert.NotNull(actual);
					AssertEqual(device1, actual);

					Assert.Null(context.AlternateDeviceIds.FirstOrDefault(i => i.Uuid == altId1.Uuid));
					Assert.NotNull(context.AlternateDeviceIds.FirstOrDefault(i => i.Uuid == altId2.Uuid));
				}
			}
		}

		[Fact]
		public async Task GetAlternateIdsForDeviceAsyncTest()
		{
			using (var connection = OpenConnection())
			{
				using (var context = GetTestDB(connection))
				{
					var repo = new DbContextRepository(context);
					var result = await repo.GetAlternateIdsForDeviceAsync(30);
					Assert.Null(result);

					var device1 = new Device()
					{
						DeviceId = 1,
						Name = "Test Name",
						IsReadOnly = true,
						Description = "Dest Description",
						DateCreated = DateTime.UtcNow,
						DisplayOrder = 2
					};
					await context.Devices.AddAsync(device1);
					await context.SaveChangesAsync();

					result = await repo.GetAlternateIdsForDeviceAsync(device1.DeviceId);
					Assert.NotNull(result);
					Assert.Empty(result);

					var altId1 = new AlternateDeviceId()
					{
						DeviceId = device1.DeviceId,
						Uuid = Guid.NewGuid(),
						MacAddress = 2,
						DateCreated = DateTime.UtcNow
					};
					var altId2 = new AlternateDeviceId
					{
						DeviceId = device1.DeviceId,
						Uuid = Guid.NewGuid(),
						MacAddress = 3,
						DateCreated = DateTime.UtcNow
					};

					await context.AlternateDeviceIds.AddRangeAsync(altId1, altId2);
					await context.SaveChangesAsync();
					context.Entry(altId1).State = EntityState.Detached;
					context.Entry(altId2).State = EntityState.Detached;

					result = await repo.GetAlternateIdsForDeviceAsync(device1.DeviceId);
					Assert.NotNull(result);
					var l = result.ToList();
					Assert.Equal(2, l.Count);
					var a = l[0];
					Assert.NotNull(a);
					Assert.Equal(altId1.DeviceId, a.DeviceId);
					Assert.Equal(altId1.Uuid, a.Uuid);
					Assert.Equal(altId1.MacAddress, a.MacAddress);
					Assert.Equal(altId1.DateCreated, a.DateCreated);
					a = l[1];
					Assert.NotNull(a);
					Assert.Equal(altId2.DeviceId, a.DeviceId);
					Assert.Equal(altId2.Uuid, a.Uuid);
					Assert.Equal(altId2.MacAddress, a.MacAddress);
					Assert.Equal(altId2.DateCreated, a.DateCreated);
				}
			}
		}
	}
}
