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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sannel.House.Base.Web;
using Sannel.House.Base.Models;
using System.Net;
using System.Diagnostics.CodeAnalysis;

namespace Sannel.House.Devices.Controllers
{
	/// <summary>
	/// Controller dealing with Alternate Id
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class AlternateIdController : Controller
	{
		private readonly IDeviceService service;
		private readonly ILogger logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="AlternateIdController" /> class.
		/// </summary>
		/// <param name="service">The repository.</param>
		/// <param name="logger">The logger.</param>
		/// <exception cref="ArgumentNullException">
		/// repository
		/// or
		/// logger
		/// </exception>
		public AlternateIdController(IDeviceService service, ILogger<AlternateIdController> logger)
		{
			this.service = service ?? throw new ArgumentNullException(nameof(service));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Gets the specified device identifier.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		[HttpGet("{deviceId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "ReadDevice")]
		public async Task<ActionResult<ResponseModel<IEnumerable<AlternateDeviceId>>>> Get(int deviceId)
		{
			if(deviceId < 0)
			{
				logger.LogError("Invalid device id({0}) passed in.", deviceId);
				return BadRequest(new ErrorResponseModel("Invalid DeviceId", "deviceId", "Device Id must be greater then or equal to 0"));
			}

			var list = await service.GetAlternateIdsForDeviceAsync(deviceId);

			if(list == null || !list.Any())
			{
				logger.LogDebug("Device with Id {0} not found", deviceId);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "device", "Device not found"));
			}

			return Ok(new ResponseModel<IEnumerable<AlternateDeviceId>>("Alternate Ids", list));
		}

		/// <summary>
		/// Posts the specified mac address.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		[HttpPost("mac/{macAddress}/{deviceId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "WriteDevice")]
		public async Task<ActionResult<ResponseModel<Device>>> Post(long macAddress, int deviceId)
		{
			if(macAddress < 0)
			{
				logger.LogError("Invalid macAddress passed {macAddress}", macAddress);
				return BadRequest(new ErrorResponseModel("Invalid Mac Address", "macAddress", "Invalid MacAddress it must be greater then 0"));
			}

			if(deviceId <= 0)
			{
				logger.LogError("Invalid device id({deviceId}) passed in.", deviceId);
				return BadRequest(new ErrorResponseModel("Invalid Device Id", "deviceId", "Device Id must be greater then 0"));
			}

			try
			{
				var device = await service.AddAlternateMacAddressAsync(deviceId, macAddress);
				if(device == null)
				{
					logger.LogDebug("No device found with id {deviceId}", deviceId);
					return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "notFound", "No device found with that id"));
				}

				return Ok(new ResponseModel<Device>("Device", device));
			}
			catch(AlternateDeviceIdException aIdException)
			{
				logger.LogError(aIdException, "Alternate Device Id Exception");
				return BadRequest(new ErrorResponseModel("Mac Address Already Connected", "macAddress", "That mac address is already connected to a device"));
			}
		}

		/// <summary>
		/// Posts the specified UUID.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		[HttpPost("uuid/{uuid}/{deviceId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "WriteDevice")]
		public async Task<ActionResult<ResponseModel<Device>>> Post(Guid uuid, int deviceId)
		{
			if(uuid == Guid.Empty)
			{
				logger.LogError("Empty Guid passed in for Uuid");
				return BadRequest(new ErrorResponseModel("Guid is Empty","uuid", "Uuid must be a valid Guid and not Guid.Empty"));
			}

			if(deviceId < 0)
			{
				logger.LogError("Invalid device id({deviceId}) passed in.", deviceId);
				return BadRequest(new ErrorResponseModel("Invalid Device Id", "deviceId", "Device Id must be greater then or equal to 0"));
			}

			try
			{
				var device = await service.AddAlternateUuidAsync(deviceId, uuid);
				if(device == null)
				{
					logger.LogDebug("No device found with id {deviceId}", deviceId);
					return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "notFound", "No device found with that id"));
				}

				return Ok(new ResponseModel<Device>("Device", device));
			}
			catch(AlternateDeviceIdException aIdException)
			{
				logger.LogError(aIdException, "Alternate Device Id Exception");
				return BadRequest(new ErrorResponseModel("Uuid Already Connected", "uuid", "That Uuid is already connected to a device"));
			}
		}

		/// <summary>
		/// Posts the specified manufacture.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		[HttpPost("manufactureid/{manufacture}/{manufactureId}/{deviceId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "WriteDevice")]
		public async Task<ActionResult<ResponseModel<Device>>> Post([NotNull]string manufacture, [NotNull]string manufactureId, int deviceId)
		{
			if(string.IsNullOrWhiteSpace(manufacture))
			{
				logger.LogError("Null or Empty Manufacture");
				return BadRequest(new ErrorResponseModel("Manufacture is Empty","manufacture", "manufacture must not be null or whitespace"));
			}

			if(string.IsNullOrWhiteSpace(manufactureId))
			{
				logger.LogError("Null or Empty ManufactureId");
				return BadRequest(new ErrorResponseModel("ManufactureID is Empty","manufactureId", "manufactureId must not be null or whitespace"));
			}

			if(deviceId < 0)
			{
				logger.LogError("Invalid device id({deviceId}) passed in.", deviceId);
				return BadRequest(new ErrorResponseModel("Invalid Device Id", "deviceId", "Device Id must be greater then or equal to 0"));
			}

			try
			{
				var device = await service.AddAlternateManufactureIdAsync(deviceId, manufacture, manufactureId);
				if(device == null)
				{
					logger.LogDebug("No device found with id {deviceId}", deviceId);
					return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound, "Device Not Found", "notFound", "No device found with that id"));
				}

				return Ok(new ResponseModel<Device>("Device", device));
			}
			catch(AlternateDeviceIdException aIdException)
			{
				logger.LogError(aIdException, "Alternate Device Id Exception");
				return BadRequest(new ErrorResponseModel("Manufacture and ManufactureId Already Connected", "manufactureId", "That Manufacture and ManufactureId is already connected to a device"));
			}
		}

		/// <summary>
		/// Deletes the specified mac address.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		[HttpDelete("mac/{macAddress}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "WriteDevice")]
		public async Task<ActionResult<ResponseModel<Device>>> Delete(long macAddress)
		{
			if(macAddress <= 0)
			{
				logger.LogError("Invalid macAddress passed {macAddress}", macAddress);
				return BadRequest(new ErrorResponseModel("Invalid Mac Address","macAddress", "Invalid MacAddress it must be greater then or equal to 0"));
			}

			var device = await service.RemoveAlternateMacAddressAsync(macAddress);
			if(device == null)
			{
				logger.LogDebug("No Mac Address found with id {macAddress}", macAddress);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound,"Mac Address Not Found","macAddress", "Mac Address not found"));
			}

			return Ok(new ResponseModel<Device>("Device",device));
		}

		/// <summary>
		/// Deletes the specified UUID.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns></returns>
		[HttpDelete("uuid/{uuid}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "WriteDevice")]
		public async Task<ActionResult<ResponseModel<Device>>> Delete(Guid uuid)
		{
			if(uuid == Guid.Empty)
			{
				logger.LogError("Empty Guid passed in for Uuid");
				return BadRequest(new ErrorResponseModel("Uuid is Empty","uuid", "Uuid must be a valid Guid and not Guid.Empty"));
			}

			var device = await service.RemoveAlternateUuidAsync(uuid);
			if(device == null)
			{
				logger.LogDebug("Uuid not found {uuid}", uuid);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound,"Uuid Not Found","uuid", "Uuid not found"));
			}

			return Ok(new ResponseModel<Device>("Device", device));
		}

		/// <summary>
		/// Deletes the specified manufacture.
		/// </summary>
		/// <param name="manufacture">The manufacture.</param>
		/// <param name="manufactureId">The manufacture identifier.</param>
		/// <returns></returns>
		[HttpDelete("manufactureid/{manufacture}/{manufactureId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Policy = "WriteDevice")]
		public async Task<ActionResult<ResponseModel<Device>>> Delete([NotNull]string manufacture, [NotNull]string manufactureId)
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

			var device = await service.RemoveAlternateManufactureIdAsync(manufacture, manufactureId);
			if(device == null)
			{
				logger.LogDebug("ManufactureId not found {manufacture}/{manufactureId}", manufacture, manufactureId);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound,"ManufactureId Not Found","manufactureid", "Manufacture/ManufactureId not found"));
			}

			return Ok(new ResponseModel<Device>("Device", device));
		}

	}
}
