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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sannel.House.Devices.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AlternateIdController : Controller
	{
		private IDeviceRepository repo;
		private ILogger logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="AlternateIdController" /> class.
		/// </summary>
		/// <param name="repository">The repository.</param>
		/// <param name="logger">The logger.</param>
		/// <exception cref="ArgumentNullException">
		/// repository
		/// or
		/// logger
		/// </exception>
		public AlternateIdController(IDeviceRepository repository, ILogger<AlternateIdController> logger)
		{
			this.repo = repository ?? throw new ArgumentNullException(nameof(repository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		public IEnumerable<AlternateDeviceId> Get(int deviceId)
		{
			return null;
		}

		[HttpPost("{macAddress}/{deviceId}")]
		[Authorize(Roles = "DeviceWrite,Admin")]
		public void Post(long macAddress, int deviceId)
		{

		}

		[HttpPost("{uuid}/{deviceId}")]
		[Authorize(Roles = "DeviceWrite,Admin")]
		public void Post(Guid uuid, int deviceId)
		{

		}

		[HttpDelete("{macAddress}/{deviceId}")]
		[Authorize(Roles = "DeviceWrite,Admin")]
		public void Delete(long macAddress, int deviceId)
		{

		}

		[HttpDelete("{uuid}/{deviceId}")]
		[Authorize(Roles = "DeviceWrite,Admin")]
		public void Delete(Guid uuid, int deviceId)
		{

		}

	}
}
