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
	[ApiController]
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
		/// <summary>
		/// Gets the first 25 devices
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = "DeviceRead,Admin")]
		public Task<ActionResult<PagedResults<Device>>> GetPaged() 
			=> GetPaged(1, 25);

		/// <summary>
		/// Gets a list of 25 devices based on the index passed in by <paramref name="pageIndex"/>
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = "DeviceRead,Admin")]
		public Task<ActionResult<PagedResults<Device>>> GetPaged(int pageIndex) 
			=> GetPaged(pageIndex, 25);

		/// <summary>
		/// Gets a list of <paramref name="pageSize"/> devices based on the index passed in by <paramref name="pageIndex"/>
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = "DeviceRead,Admin")]
		public async Task<ActionResult<PagedResults<Device>>> GetPaged(int pageIndex, int pageSize)
		{
			if(pageIndex < 1)
			{
				return BadRequest(new ErrorModel(nameof(pageIndex), "Page Index must be 1 or greater"));
			}
			if(pageSize < 2)
			{
				return BadRequest(new ErrorModel(nameof(pageSize), "Page Size must be greater then 2"));
			}
			return Ok(await repo.GetDevicesListAsync(pageIndex, pageSize));
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		public async Task<ActionResult<Device>> Get(int deviceId)
		{
			if(deviceId < 0)
			{
				return BadRequest(new ErrorModel(nameof(deviceId), "Device Id must be geater then or equal to 0"));
			}

			var device = await repo.GetDeviceByIdAsync(deviceId);
			if(device == null)
			{
				return NotFound(new ErrorModel("device", "Device Not Found"));
			}

			return Ok(device);
		}

		[HttpPost]
		[Authorize(Roles = "DeviceWrite,Admin")]
		public async Task<ActionResult<int>> Post([FromBody]Device device)
		{
			if(device == null)
			{
				return BadRequest(new ErrorModel(nameof(device), "No device info provided"));
			}

			if(ModelState.IsValid)
			{
				device.DeviceId = 0;
				var d = await repo.AddDeviceAsync(device);
				return Ok(d.DeviceId);
			}
			else
			{
				return BadRequest(new ErrorModel(ModelState));
			}
		}

		/*
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
