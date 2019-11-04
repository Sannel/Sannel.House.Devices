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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sannel.House.Devices
{
	/// <summary>
	/// 
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// Defines the entry point of the application.
		/// </summary>
		/// <param name="args">The arguments.</param>
		public static void Main(string[] args) 
			=> CreateHostBuilder(args).Build().Run();

		/// <summary>
		/// Creates the host builder.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(o =>
				{
					o.ConfigureAppConfiguration(b =>
					{
						b.AddJsonFile(Path.Combine("app_config", "appsettings.json"), true, true);
						b.AddYamlFile(Path.Combine("app_config", "appsettings.yml"), true, true);
						var shared = Path.Combine("app_config", "shared");
						if(Directory.Exists(shared))
						{
							foreach(var f in Directory.GetFiles(Path.Combine(shared), "*.yaml"))
							{
								b.AddYamlFile(f, true, true);
							}
						}
						b.AddEnvironmentVariables();
					});
					o.UseStartup<Startup>();
				});
	}
}
