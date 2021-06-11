/* Copyright 2019-2021 Sannel Software, L.L.C.
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
using Sannel.House.Base.Models;
using Sannel.House.Base.Web;
using System.Security.Claims;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Sannel.House.Devices.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class DevicesController : Controller
	{
		/// <summary>
		/// Scopes allowed to read device information
		/// </summary>
		public const string ReadScope = "read:device";
		/// <summary>
		/// Scopes allowed to list devices
		/// </summary>
		public const string ListScope = "list:device";
		private IDeviceService service;
		private ILogger logger;
		/// <summary>
		/// Initializes a new instance of the <see cref="DevicesController"/> class.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="logger">The logger.</param>
		/// <exception cref="System.ArgumentNullException">
		/// service
		/// or
		/// logger
		/// </exception>
		public DevicesController(IDeviceService service, ILogger<DevicesController> logger)
		{
			this.service = service ?? throw new ArgumentNullException(nameof(service));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Gets the first 25 devices
		/// </summary>
		/// <returns></returns>
		[HttpGet("GetPaged")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[Authorize(Policy = "ListDevice")]
		public Task<ActionResult<PagedResponseModel<Device>>> GetPaged() 
			=> GetPaged(0, 25);

		/// <summary>
		/// Gets a list of 25 devices based on the index passed in by <paramref name="pageIndex"/>
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <returns></returns>
		[HttpGet("GetPaged/{pageIndex}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[Authorize(Policy = "ListDevice")]
		public Task<ActionResult<PagedResponseModel<Device>>> GetPaged(int pageIndex) 
			=> GetPaged(pageIndex, 25);

		/// <summary>
		/// Gets a list of <paramref name="pageSize"/> devices based on the index passed in by <paramref name="pageIndex"/>
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <returns></returns>
		[HttpGet("GetPaged/{pageIndex}/{pageSize}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesDefaultResponseType]
		[Authorize(Policy = "ListDevice")]
		public async Task<ActionResult<PagedResponseModel<Device>>> GetPaged(int pageIndex, int pageSize)
		{
			if (pageIndex < 0)
			{
				logger.LogError("GetPaged pageIndex invalid {pageIndex}", pageIndex);
				return BadRequest(new ErrorResponseModel("Invalid Page Index", nameof(pageIndex), "Page Index must be 0 or greater"));
			}
			if(pageSize < 2)
			{
				logger.LogError("GetPaged pageSize invalid {pageSize}", pageSize);
				return BadRequest(new ErrorResponseModel("Invalid Page Size", nameof(pageSize), "Page Size must be greater then 2"));
			}

			logger.LogDebug("GetPaged pageIndex: {pageIndex} pageSize {pageSize}", pageIndex, pageSize);

			return Ok(new PagedResponseModel<Device>("Paged Results", await service.GetDevicesListAsync(pageIndex, pageSize).ConfigureAwait(false)));
		}

		/// <summary>
		/// Gets the device by its device identifier.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		[HttpGet("{deviceId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "ReadDevice")]
		public async Task<ActionResult<ResponseModel<Device>>> Get(int deviceId)
		{
			if(deviceId < 0)
			{
				logger.LogError("Get: Invalid deviceId {deviceId}", deviceId);
				return BadRequest(new ErrorResponseModel("Invalid Device Id", nameof(deviceId), "Device Id must be greater then or equal to 0"));
			}

			var device = await service.GetDeviceByIdAsync(deviceId);
			if(device == null)
			{
				logger.LogDebug("Device with id {deviceId} not found", deviceId);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "device", "Device Not Found"));
			}

			logger.LogDebug("Device with id {deviceId} was found", deviceId);

			return Ok(new ResponseModel<Device>("The Device", device));
		}

		/// <summary>
		/// Gets the device by alt identifier.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		[HttpGet("GetByMac/{macAddress}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "ReadDevice")]
		public async Task<ActionResult<ResponseModel<Device>>> GetByMacAddress(long macAddress)
		{
			if(macAddress < 0)
			{
				logger.LogError("Get: Invalid macAddress {macAddress}", macAddress);
				return BadRequest(new ErrorResponseModel("Invalid Mac Address", nameof(macAddress), "Mac Address must be greater then or equal to 0"));
			}

			var device = await service.GetDeviceByMacAddressAsync(macAddress);
			if(device == null)
			{
				logger.LogDebug("Device with macAddress {macAddress} not found", macAddress);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "device", "Device Not Found"));
			}

			logger.LogDebug("Device with macAddress {macAddress} was found", macAddress);

			return Ok(new ResponseModel<Device>("The Device", device));
		}

		/// <summary>
		/// Gets the device by alt identifier.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns></returns>
		[HttpGet("GetByUuid/{uuid}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "ReadDevice")]
		public async Task<ActionResult<ResponseModel<Device>>> GetByUuid(Guid uuid)
		{
			if(Guid.Empty == uuid)
			{
				logger.LogError("Get: Invalid Uuid {uuid}", uuid);
				return BadRequest(new ErrorResponseModel("Invalid Uuid", nameof(uuid), $"Uuid cannot be Empty {Guid.Empty}"));
			}

			var device = await service.GetDeviceByUuidAsync(uuid);
			if(device == null)
			{
				logger.LogDebug("Device with Uuid {uuid} not found", uuid);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "device", "Device Not Found"));
			}

			logger.LogDebug("Device with Uuid {uuid} was found", uuid);

			return Ok(new ResponseModel<Device>("The Device", device));
		}

		/// <summary>
		/// Gets the device by manufacture identifier.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns></returns>
		[HttpGet("GetByManufactureId/{manufacture}/{manufactureId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "ReadDevice")]
		public async Task<ActionResult<ResponseModel<Device>>> GetByManufactureId([NotNull]string manufacture, [NotNull]string manufactureId)
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

			var device = await service.GetDeviceByManufactureIdAsync(manufacture, manufactureId);
			if(device == null)
			{
				logger.LogDebug("Device with Manufacture/ManufactureId {manufacture}/{manufactureId} not found", manufacture, manufactureId);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "device", "Device Not Found"));
			}

			logger.LogDebug("Device with Manufacture/ManufactureId {manufacture}/{manufactureId} was found", manufacture, manufactureId);

			return Ok(new ResponseModel<Device>("The Device", device));
		}

		/// <summary>
		/// Creates a new device with the data provided.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		[HttpPost]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[Authorize(Policy = "CreateDevice")]
		public async Task<ActionResult<ResponseModel<int>>> Post([FromBody]Device device)
		{
			if(device == null)
			{
				logger.LogError("Post: device is null");
				return BadRequest(new ErrorResponseModel("Invalid Device Object", nameof(device), "No device info provided"));
			}

			if(ModelState.IsValid)
			{
				logger.LogDebug("Post: adding new device '{name}'", device.Name);
				device.DeviceId = 0;
				var d = await service.AddDeviceAsync(device);
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
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "WriteDevice")]
		public async Task<ActionResult<ResponseModel<int>>> Put([FromBody]Device device)
		{
			if(device == null)
			{
				logger.LogError("Put: No device passed");
				return BadRequest(new ErrorResponseModel("Invalid Device Object", nameof(device), "No device info provided"));
			}

			if(device.DeviceId < 0)
			{
				logger.LogError("Put: Invalid deviceId {deviceId}", device.DeviceId);
				return BadRequest(new ErrorResponseModel("Invalid Device Id", nameof(device.DeviceId), "Device Id must be 0 or greater"));
			}

			if(ModelState.IsValid)
			{
				try
				{
					var d = await service.UpdateDeviceAsync(device); 

					if(d == null)
					{
						logger.LogDebug("Put: Device {deviceId} was not found", device.DeviceId);
						return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "notfound", "Device not found to update"));
					}

					logger.LogDebug("Put: Updated device {deviceId}", device.DeviceId);
					return Ok(new ResponseModel<int>("Device Id",d.DeviceId));
				}
				catch(ReadOnlyException roe)
				{
					logger.LogError(roe, "Put: Device {deviceId} is read-only", device.DeviceId);
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
