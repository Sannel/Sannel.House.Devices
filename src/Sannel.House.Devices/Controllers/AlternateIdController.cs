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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sannel.House.Web;
using Sannel.House.Models;
using System.Net;

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

		/// <summary>
		/// Gets the specified device identifier.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		[HttpGet("{deviceId}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ResponseModel<IEnumerable<AlternateDeviceId>>>> Get(int deviceId)
		{
			if(deviceId < 0)
			{
				logger.LogError("Invalid device id({0}) passed in.", deviceId);
				return BadRequest(new ErrorResponseModel("Invalid DeviceId", "deviceId", "Device Id must be greater then or equal to 0"));
			}

			var list = await repo.GetAlternateIdsForDeviceAsync(deviceId);

			if(list == null)
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
		[Authorize(Roles = "DeviceWrite,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ResponseModel<Device>>> Post(long macAddress, int deviceId)
		{
			if(macAddress < 0)
			{
				logger.LogError("Invalid macAddress passed {0}", macAddress);
				return BadRequest(new ErrorResponseModel("Invalid Mac Address", "macAddress", "Invalid MacAddress it must be greater then or equal to 0"));
			}

			if(deviceId < 0)
			{
				logger.LogError("Invalid device id({0}) passed in.", deviceId);
				return BadRequest(new ErrorResponseModel("Invalid Device Id", "deviceId", "Device Id must be greater then or equal to 0"));
			}

			try
			{
				var device = await repo.AddAlternateMacAddressAsync(deviceId, macAddress);
				if(device == null)
				{
					logger.LogDebug("No device found with id {0}", deviceId);
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
		[Authorize(Roles = "DeviceWrite,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ResponseModel<Device>>> Post(Guid uuid, int deviceId)
		{
			if(uuid == Guid.Empty)
			{
				logger.LogError("Empty Guid passed in for Uuid");
				return BadRequest(new ErrorResponseModel("Guid is Empty","uuid", "Uuid must be a valid Guid and not Guid.Empty"));
			}

			if(deviceId < 0)
			{
				logger.LogError("Invalid device id({0}) passed in.", deviceId);
				return BadRequest(new ErrorResponseModel("Invalid Device Id", "deviceId", "Device Id must be greater then or equal to 0"));
			}

			try
			{
				var device = await repo.AddAlternateUuidAsync(deviceId, uuid);
				if(device == null)
				{
					logger.LogDebug("No device found with id {0}", deviceId);
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
		/// Deletes the specified mac address.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		[HttpDelete("mac/{macAddress}")]
		[Authorize(Roles = "DeviceWrite,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ResponseModel<Device>>> Delete(long macAddress)
		{
			if(macAddress < 0)
			{
				logger.LogError("Invalid macAddress passed {0}", macAddress);
				return BadRequest(new ErrorResponseModel("Invalid Mac Address","macAddress", "Invalid MacAddress it must be greater then or equal to 0"));
			}

			var device = await repo.RemoveAlternateMacAddressAsync(macAddress);
			if(device == null)
			{
				logger.LogDebug("No Mac Address found with id {0}", macAddress);
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
		[Authorize(Roles = "DeviceWrite,Admin")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ResponseModel<Device>>> Delete(Guid uuid)
		{
			if(uuid == Guid.Empty)
			{
				logger.LogError("Empty Guid passed in for Uuid");
				return BadRequest(new ErrorResponseModel("Uuid is Empty","uuid", "Uuid must be a valid Guid and not Guid.Empty"));
			}

			var device = await repo.RemoveAlternateUuidAsync(uuid);
			if(device == null)
			{
				logger.LogDebug("Uuid not found {0}", uuid);
				return NotFound(new ErrorResponseModel(HttpStatusCode.NotFound,"Uuid Not Found","uuid", "Uuid not found"));
			}

			return Ok(new ResponseModel<Device>("Device", device));
		}

	}
}
