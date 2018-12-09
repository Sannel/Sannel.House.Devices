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

		/// <summary>
		/// Gets the specified device identifier.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		[HttpGet("{deviceId}")]
		[Authorize(Roles = "DeviceRead,Admin")]
		public async Task<ActionResult<IEnumerable<AlternateDeviceId>>> Get(int deviceId)
		{
			if(deviceId < 0)
			{
				logger.LogError("Invalid device id({0}) passed in.", deviceId);
				return BadRequest(new ErrorModel("deviceId", "Device Id must be greater then or equal to 0"));
			}

			var list = await repo.GetAlternateIdsForDeviceAsync(deviceId);

			if(list == null)
			{
				logger.LogDebug("Device with Id {0} not found", deviceId);
				return NotFound(new ErrorModel("device", "Device not found"));
			}

			return Ok(list);
		}

		/// <summary>
		/// Posts the specified mac address.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <param name="deviceId">The device identifier.</param>
		/// <returns></returns>
		[HttpPost("mac/{macAddress}/{deviceId}")]
		[Authorize(Roles = "DeviceWrite,Admin")]
		public async Task<ActionResult<Device>> Post(long macAddress, int deviceId)
		{
			if(macAddress < 0)
			{
				logger.LogError("Invalid macAddress passed {0}", macAddress);
				return BadRequest(new ErrorModel("macAddress", "Invalid MacAddress it must be greater then or equal to 0"));
			}

			if(deviceId < 0)
			{
				logger.LogError("Invalid device id({0}) passed in.", deviceId);
				return BadRequest(new ErrorModel("deviceId", "Device Id must be greater then or equal to 0"));
			}

			try
			{
				var device = await repo.AddAlternateMacAddressAsync(deviceId, macAddress);
				if(device == null)
				{
					logger.LogDebug("No device found with id {0}", deviceId);
					return NotFound(new ErrorModel("notFound", "No device found with that id"));
				}

				return Ok(device);
			}
			catch(AlternateDeviceIdException aIdException)
			{
				logger.LogError(aIdException, "Alternate Device Id Exception");
				return BadRequest(new ErrorModel("macAddress", "That mac address is already connected to a device"));
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
		public async Task<ActionResult<Device>> Post(Guid uuid, int deviceId)
		{
			if(uuid == Guid.Empty)
			{
				logger.LogError("Empty Guid passed in for Uuid");
				return BadRequest(new ErrorModel("uuid", "Uuid must be a valid Guid and not Guid.Empty"));
			}

			if(deviceId < 0)
			{
				logger.LogError("Invalid device id({0}) passed in.", deviceId);
				return BadRequest(new ErrorModel("deviceId", "Device Id must be greater then or equal to 0"));
			}

			try
			{
				var device = await repo.AddAlternateUuidAsync(deviceId, uuid);
				if(device == null)
				{
					logger.LogDebug("No device found with id {0}", deviceId);
					return NotFound(new ErrorModel("notFound", "No device found with that id"));
				}

				return Ok(device);
			}
			catch(AlternateDeviceIdException aIdException)
			{
				logger.LogError(aIdException, "Alternate Device Id Exception");
				return BadRequest(new ErrorModel("uuid", "That Uuid is already connected to a device"));
			}
		}

		/// <summary>
		/// Deletes the specified mac address.
		/// </summary>
		/// <param name="macAddress">The mac address.</param>
		/// <returns></returns>
		[HttpDelete("{macAddress}")]
		[Authorize(Roles = "DeviceWrite,Admin")]
		public async Task<ActionResult<Device>> Delete(long macAddress)
		{
			if(macAddress < 0)
			{
				logger.LogError("Invalid macAddress passed {0}", macAddress);
				return BadRequest(new ErrorModel("macAddress", "Invalid MacAddress it must be greater then or equal to 0"));
			}

			var device = await repo.RemoveAlternateMacAddressAsync(macAddress);
			if(device == null)
			{
				logger.LogDebug("No Mac Address found with id {0}", macAddress);
				return NotFound(new ErrorModel("macAddress", "Mac Address not found"));
			}

			return Ok(device);
		}

		/// <summary>
		/// Deletes the specified UUID.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns></returns>
		[HttpDelete("{uuid}")]
		[Authorize(Roles = "DeviceWrite,Admin")]
		public async Task<ActionResult<Device>> Delete(Guid uuid)
		{
			if(uuid == Guid.Empty)
			{
				logger.LogError("Empty Guid passed in for Uuid");
				return BadRequest(new ErrorModel("uuid", "Uuid must be a valid Guid and not Guid.Empty"));
			}

			var device = await repo.RemoveAlternateUuidAsync(uuid);
			if(device == null)
			{
				logger.LogDebug("Uuid not found {0}", uuid);
				return NotFound(new ErrorModel("uuid", "Uuid not found"));
			}

			return Ok(device);
		}

	}
}
