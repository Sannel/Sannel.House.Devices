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
using System.Data;
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
		[HttpGet("GetPaged")]
		[Authorize(Roles = "DeviceRead,Admin")]
		public Task<ActionResult<PagedResults<Device>>> GetPaged() 
			=> GetPaged(1, 25);

		/// <summary>
		/// Gets a list of 25 devices based on the index passed in by <paramref name="pageIndex"/>
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <returns></returns>
		[HttpGet("GetPaged/{pageIndex}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		public Task<ActionResult<PagedResults<Device>>> GetPaged(int pageIndex) 
			=> GetPaged(pageIndex, 25);

		/// <summary>
		/// Gets a list of <paramref name="pageSize"/> devices based on the index passed in by <paramref name="pageIndex"/>
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <returns></returns>
		[HttpGet("GetPaged/{pageIndex}/{pageSize}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		public async Task<ActionResult<PagedResults<Device>>> GetPaged(int pageIndex, int pageSize)
		{
			if(pageIndex < 1)
			{
				logger.LogError("GetPaged pageIndex invalid {0}", pageIndex);
				return BadRequest(new ErrorModel(nameof(pageIndex), "Page Index must be 1 or greater"));
			}
			if(pageSize < 2)
			{
				logger.LogError("GetPaged pageSize invalid {0}", pageSize);
				return BadRequest(new ErrorModel(nameof(pageSize), "Page Size must be greater then 2"));
			}

			logger.LogDebug("GetPaged pageIndex: {0} pageSize {1}", pageIndex, pageSize);

			return Ok(await repo.GetDevicesListAsync(pageIndex, pageSize));
		}

		/// <summary>
		/// Gets the device by its device identifier.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		[HttpGet("{deviceId}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		public async Task<ActionResult<Device>> Get(int deviceId)
		{
			if(deviceId < 0)
			{
				logger.LogError("Get: Invalid deviceId {0}", deviceId);
				return BadRequest(new ErrorModel(nameof(deviceId), "Device Id must be geater then or equal to 0"));
			}

			var device = await repo.GetDeviceByIdAsync(deviceId);
			if(device == null)
			{
				logger.LogDebug("Device with id {0} not found", deviceId);
				return NotFound(new ErrorModel("device", "Device Not Found"));
			}

			logger.LogDebug("Device with id {0} was found", deviceId);

			return Ok(device);
		}

		/// <summary>
		/// Gets the device by alt identifier.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		[HttpGet("GetByMac/{macAddress}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		public async Task<ActionResult<Device>> GetByAltId(long macAddress)
		{
			if(macAddress < 0)
			{
				logger.LogError("Get: Invalid macAddress {0}", macAddress);
				return BadRequest(new ErrorModel(nameof(macAddress), "Mac Address must be geater then or equal to 0"));
			}

			var device = await repo.GetDeviceByMacAddressAsync(macAddress);
			if(device == null)
			{
				logger.LogDebug("Device with macAddress {0} not found", macAddress);
				return NotFound(new ErrorModel("device", "Device Not Found"));
			}

			logger.LogDebug("Device with macAddress {0} was found", macAddress);

			return Ok(device);
		}

		/// <summary>
		/// Gets the device by alt identifier.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns></returns>
		[HttpGet("GetByUuid/{uuid}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		public async Task<ActionResult<Device>> GetByAltId(Guid uuid)
		{
			if(Guid.Empty == uuid)
			{
				logger.LogError("Get: Invalid Uuid {0}", uuid);
				return BadRequest(new ErrorModel(nameof(uuid), $"Uuid cannot be Empty {Guid.Empty}"));
			}

			var device = await repo.GetDeviceByUuidAsync(uuid);
			if(device == null)
			{
				logger.LogDebug("Device with Uuid {0} not found", uuid);
				return NotFound(new ErrorModel("device", "Device Not Found"));
			}

			logger.LogDebug("Device with Uuid {0} was found", uuid);

			return Ok(device);
		}

		/// <summary>
		/// Creates a new device with the data provided.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "DeviceWrite,Admin")]
		public async Task<ActionResult<int>> Post([FromBody]Device device)
		{
			if(device == null)
			{
				logger.LogError("Post: device is null");
				return BadRequest(new ErrorModel(nameof(device), "No device info provided"));
			}

			if(ModelState.IsValid)
			{
				logger.LogDebug("Post: adding new device '{0}'", device.Name);
				device.DeviceId = 0;
				var d = await repo.AddDeviceAsync(device);
				return Ok(d.DeviceId);
			}
			else
			{
				logger.LogInformation("Post: Invalid Model");
				return BadRequest(new ErrorModel(ModelState));
			}
		}

		/// <summary>
		/// Updateds the provided device.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		[HttpPut]
		[Authorize(Roles = "DeviceWrite,Admin")]
		public async Task<ActionResult<int>> Put([FromBody]Device device)
		{
			if(device == null)
			{
				logger.LogError("Put: No device passed");
				return BadRequest(new ErrorModel(nameof(device), "No device info provided"));
			}

			if(device.DeviceId < 0)
			{
				logger.LogError("Put: Invalid deviceId {0}", device.DeviceId);
				return BadRequest(new ErrorModel(nameof(device.DeviceId), "Device Id must be 0 or greater"));
			}

			if(ModelState.IsValid)
			{
				try
				{
					var d = await repo.UpdateDeviceAsync(device); 

					if(d == null)
					{
						logger.LogDebug("Put: Device {0} was not found", device.DeviceId);
						return NotFound(new ErrorModel("notfound", "Device not found to update"));
					}

					logger.LogDebug("Put: Updated device {0}", device.DeviceId);
					return Ok(d.DeviceId);
				}
				catch(ReadOnlyException roe)
				{
					logger.LogError(roe, "Put: Device {0} is readonly", device.DeviceId);
					return BadRequest(new ErrorModel("readonly", "Device is readonly and cannot be updated."));
				}
			}
			else
			{
				logger.LogInformation("Put: Invalid device passed");
				return BadRequest(new ErrorModel(ModelState));
			}
		}

	}
}
