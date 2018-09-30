using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Devices.Tests
{
	public abstract class BaseTests
	{
		private ILoggerFactory loggerFactory;

		public ILogger<T> CreateLogger<T>()
		{
			var l = loggerFactory ?? new LoggerFactory();
			return l.CreateLogger<T>();
		}
	}
}
