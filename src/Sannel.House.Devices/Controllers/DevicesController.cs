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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using Sannel.House.Models;
using Sannel.House.Web;

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
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public Task<ActionResult<PagedResponseModel<Device>>> GetPaged() 
			=> GetPaged(0, 25);

		/// <summary>
		/// Gets a list of 25 devices based on the index passed in by <paramref name="pageIndex"/>
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <returns></returns>
		[HttpGet("GetPaged/{pageIndex}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public Task<ActionResult<PagedResponseModel<Device>>> GetPaged(int pageIndex) 
			=> GetPaged(pageIndex, 25);

		/// <summary>
		/// Gets a list of <paramref name="pageSize"/> devices based on the index passed in by <paramref name="pageIndex"/>
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <returns></returns>
		[HttpGet("GetPaged/{pageIndex}/{pageSize}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public async Task<ActionResult<PagedResponseModel<Device>>> GetPaged(int pageIndex, int pageSize)
		{
			if(pageIndex < 0)
			{
				logger.LogError("GetPaged pageIndex invalid {0}", pageIndex);
				return BadRequest(new ErrorResponseModel("Invalid Page Index", nameof(pageIndex), "Page Index must be 0 or greater"));
			}
			if(pageSize < 2)
			{
				logger.LogError("GetPaged pageSize invalid {0}", pageSize);
				return BadRequest(new ErrorResponseModel("Invalid Page Size", nameof(pageSize), "Page Size must be greater then 2"));
			}

			logger.LogDebug("GetPaged pageIndex: {0} pageSize {1}", pageIndex, pageSize);

			return Ok(new PagedResponseModel<Device>("Paged Results", await repo.GetDevicesListAsync(pageIndex, pageSize)));
		}

		/// <summary>
		/// Gets the device by its device identifier.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		[HttpGet("{deviceId}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ResponseModel<Device>>> Get(int deviceId)
		{
			if(deviceId < 0)
			{
				logger.LogError("Get: Invalid deviceId {0}", deviceId);
				return BadRequest(new ErrorResponseModel("Invalid Device Id", nameof(deviceId), "Device Id must be greater then or equal to 0"));
			}

			var device = await repo.GetDeviceByIdAsync(deviceId);
			if(device == null)
			{
				logger.LogDebug("Device with id {0} not found", deviceId);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "device", "Device Not Found"));
			}

			logger.LogDebug("Device with id {0} was found", deviceId);

			return Ok(new ResponseModel<Device>("The Device", device));
		}

		/// <summary>
		/// Gets the device by alt identifier.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		[HttpGet("GetByMac/{macAddress}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ResponseModel<Device>>> GetByMacAddress(long macAddress)
		{
			if(macAddress < 0)
			{
				logger.LogError("Get: Invalid macAddress {0}", macAddress);
				return BadRequest(new ErrorResponseModel("Invalid Mac Address", nameof(macAddress), "Mac Address must be greater then or equal to 0"));
			}

			var device = await repo.GetDeviceByMacAddressAsync(macAddress);
			if(device == null)
			{
				logger.LogDebug("Device with macAddress {0} not found", macAddress);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "device", "Device Not Found"));
			}

			logger.LogDebug("Device with macAddress {0} was found", macAddress);

			return Ok(new ResponseModel<Device>("The Device", device));
		}

		/// <summary>
		/// Gets the device by alt identifier.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns></returns>
		[HttpGet("GetByUuid/{uuid}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ResponseModel<Device>>> GetByUuid(Guid uuid)
		{
			if(Guid.Empty == uuid)
			{
				logger.LogError("Get: Invalid Uuid {0}", uuid);
				return BadRequest(new ErrorResponseModel("Invalid Uuid", nameof(uuid), $"Uuid cannot be Empty {Guid.Empty}"));
			}

			var device = await repo.GetDeviceByUuidAsync(uuid);
			if(device == null)
			{
				logger.LogDebug("Device with Uuid {0} not found", uuid);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "device", "Device Not Found"));
			}

			logger.LogDebug("Device with Uuid {0} was found", uuid);

			return Ok(new ResponseModel<Device>("The Device", device));
		}

		[HttpGet("GetByManufactureId/{manufacture}/{manufactureId}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ResponseModel<Device>>> GetByManufactureId(string manufacture, string manufactureId)
		{
			if(string.IsNullOrWhiteSpace(manufacture))
			{
				logger.LogError("Null or Empty Manufacture");
				return BadRequest(new ErrorResponseModel("Manufacture is Empty","manufacture", "Manufacture must not be null or whitespace"));
			}

			if(string.IsNullOrWhiteSpace(manufactureId))
			{
				logger.LogError("Null or Empty ManufactureId");
				return BadRequest(new ErrorResponseModel("ManufactureID is Empty","manufactureId", "ManufactureId must not be null or whitespace"));
			}

			var device = await repo.GetDeviceByManufactureIdAsync(manufacture, manufactureId);
			if(device == null)
			{
				logger.LogDebug("Device with Manufacture/ManufactureId {0}/{1} not found", manufacture, manufactureId);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "device", "Device Not Found"));
			}

			logger.LogDebug("Device with Manufacture/ManufactureId {0}/{1} was found", manufacture, manufactureId);

			return Ok(new ResponseModel<Device>("The Device", device));
		}

		/// <summary>
		/// Creates a new device with the data provided.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "DeviceWrite,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public async Task<ActionResult<ResponseModel<int>>> Post([FromBody]Device device)
		{
			if(device == null)
			{
				logger.LogError("Post: device is null");
				return BadRequest(new ErrorResponseModel("Invalid Device Object", nameof(device), "No device info provided"));
			}

			if(ModelState.IsValid)
			{
				logger.LogDebug("Post: adding new device '{0}'", device.Name);
				device.DeviceId = 0;
				var d = await repo.AddDeviceAsync(device);
				return Ok(new ResponseModel<int>("The Device Id", d.DeviceId));
			}
			else
			{
				logger.LogInformation("Post: Invalid Model");
				return BadRequest(new ErrorResponseModel(HttpStatusCode.BadRequest, "Invalid Model").FillWithStateDictionary( ModelState));
			}
		}

		/// <summary>
		/// Updates the provided device.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		[HttpPut]
		[Authorize(Roles = "DeviceWrite,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ResponseModel<int>>> Put([FromBody]Device device)
		{
			if(device == null)
			{
				logger.LogError("Put: No device passed");
				return BadRequest(new ErrorResponseModel("Invalid Device Object", nameof(device), "No device info provided"));
			}

			if(device.DeviceId < 0)
			{
				logger.LogError("Put: Invalid deviceId {0}", device.DeviceId);
				return BadRequest(new ErrorResponseModel("Invalid Device Id", nameof(device.DeviceId), "Device Id must be 0 or greater"));
			}

			if(ModelState.IsValid)
			{
				try
				{
					var d = await repo.UpdateDeviceAsync(device); 

					if(d == null)
					{
						logger.LogDebug("Put: Device {0} was not found", device.DeviceId);
						return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "notfound", "Device not found to update"));
					}

					logger.LogDebug("Put: Updated device {0}", device.DeviceId);
					return Ok(new ResponseModel<int>("Device Id",d.DeviceId));
				}
				catch(ReadOnlyException roe)
				{
					logger.LogError(roe, "Put: Device {0} is read-only", device.DeviceId);
					return BadRequest(new ErrorResponseModel("Device is Read-only", "readonly", "Device is read-only and cannot be updated."));
				}
			}
			else
			{
				logger.LogInformation("Put: Invalid device passed");
				return BadRequest(new ErrorResponseModel(HttpStatusCode.BadRequest, "Invalid Device Object").FillWithStateDictionary(ModelState));
			}
		}

	}
}
