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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;

namespace Sannel.House.Devices.Controllers
{
	[Route("api/[controller]")]
	public class DevicesController : Controller
	{
		private IDeviceRepository repo;
		private ILogger logger;
		public DevicesController(IDeviceRepository repository, ILogger<DevicesController> logger)
		{
			repo = repository ?? throw new ArgumentNullException(nameof(repository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		/*						"DeviceCreate",
						"DeviceEdit",
						"DeviceRead",*/
		// GET: api/<controller>
		[HttpGet]
		[Authorize(Roles = "DeviceRead,Admin")]
		public async Task<PagedResults<Device>> Get() 
			=> await repo.GetDevicesListAsync(0, 25);

		/*// GET api/<controller>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<controller>
		[HttpPost]
		public void Post([FromBody]string value)
		{
		}

		// PUT api/<controller>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/<controller>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
		*/
	}
}
