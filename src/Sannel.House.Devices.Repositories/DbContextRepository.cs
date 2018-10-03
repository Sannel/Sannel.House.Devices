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
using JetBrains.Annotations;
using Sannel.House.Devices.Data;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannel.House.Devices.Repositories
{
	public class DbContextRepository : IDeviceRepository
	{
		private DevicesDbContext context;
		/// <summary>
		/// Initializes a new instance of the <see cref="DbContextRepository"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <exception cref="ArgumentNullException">context</exception>
		public DbContextRepository([NotNull]DevicesDbContext context)
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Gets the devices list asynchronous.
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <returns></returns>
		public async Task<PagedResults<Device>> GetDevicesListAsync(int pageIndex, int pageSize)
		{
			var result = await Task.Run(() => new PagedResults<Device>
			{
				Page = pageIndex,
				PageSize = pageSize,
				TotalCount = context.Devices.LongCount(),
				Data = context.Devices
							.OrderBy(i => i.DisplayOrder)
							.Skip((pageIndex - 1) * pageSize)
							.Take(pageSize)
			});

			return result;
		}
	}
}
