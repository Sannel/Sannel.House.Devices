using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sannel.House.Devices.Models
{
	public class AlternateDeviceIdException : Exception
	{
		public AlternateDeviceIdException()
		{
		}

		public AlternateDeviceIdException(string message) : base(message)
		{
		}

		public AlternateDeviceIdException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
