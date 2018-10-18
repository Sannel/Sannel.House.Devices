using Moq;
using Sannel.House.Devices.Controllers;
using Sannel.House.Devices.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sannel.House.Devices.Tests.Controllers
{
	public class AlternateIdControllerTest : BaseTests
	{
		[Fact]
		public void ConstructorArgumentsTest()
		{
			Assert.Throws<ArgumentNullException>("repository",
				() => new AlternateIdController(null, null));
			Assert.Throws<ArgumentNullException>("logger",
				() => new AlternateIdController(new Mock<IDeviceRepository>().Object, null));
		}
	}
}
