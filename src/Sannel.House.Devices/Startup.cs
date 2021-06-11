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
using Sannel.House.Base.Data;
using Sannel.House.Base.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sannel.House.Devices.Data;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Repositories;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Sannel.House.Devices.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Sannel.House.Devices
{
	/// <summary>
	/// The startup class for Devices
	/// </summary>
	public class Startup
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="Startup" /> class.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public Startup(IConfiguration configuration)
		{
			var root = (ConfigurationRoot)configuration;

			Configuration = configuration;
		}

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <value>
		/// The configuration.
		/// </value>
		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		/// <summary>
		/// Configures the services.
		/// </summary>
		/// <param name="services">The services.</param>
		public void ConfigureServices(IServiceCollection services)
		{
			var connectionString = Configuration.GetWithReplacement("Db:ConnectionString");
			if (string.IsNullOrWhiteSpace(connectionString))
			{
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
				throw new ArgumentNullException("Db:ConnectionString", "Db:ConnectionString is required");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
			}

			services.AddDbContextPool<DevicesDbContext>(o =>
			{
				switch (Configuration["Db:Provider"])
				{
					case "MySQL":
					case "mysql":
						throw new NotSupportedException("We are currently not supporting mysql as a database provider");

					case "sqlserver":
					case "SqlServer":
						o.ConfigureSqlServer(connectionString);
						break;
					case "PostgreSQL":
					case "postgresql":
						o.ConfigurePostgreSQL(connectionString);
						break;
					case "sqlite":
					default:
						o.ConfigureSqlite(connectionString);
						break;
				}
			});

			services.AddScoped<IDeviceRepository, DbContextRepository>();
			services.AddScoped<IDeviceService, DeviceService>();


			services.AddControllers();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.Authority = Configuration.GetValue<string>("Authentication:Authority");
				options.Audience = Configuration.GetValue<string>("Authentication:Audience");
			});

			services.AddAuthorization(options =>
			{
				var permissionClaim = Configuration["Authentication:PermissionClaim"];
				options.AddPolicy("ListDevice", b =>
				{
					b.AddScopeRequirement("list:device");
				});
				options.AddPolicy("ReadDevice", b =>
				{
					b.AddScopeRequirement("read:device");
				});
				options.AddPolicy("WriteDevice", b =>
				{
					b.AddScopeRequirement("write:device");
				});
				options.AddPolicy("CreateDevice", b =>
				{
					b.AddScopeRequirement("create:device");
				});
			});

			services.AddOpenApiDocument();

			services.AddMqttService(Configuration);

			services.AddHealthChecks()
				.AddDbHealthCheck<DevicesDbContext>("DbHealthCheck", async (context) =>
				{
					await context.Devices.AnyAsync().ConfigureAwait(false);
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		/// <summary>
		/// Configures the specified application.
		/// </summary>
		/// <param name="app">The application.</param>
		/// <param name="env">The env.</param>
		/// <param name="provider">The provider.</param>
		/// <param name="logger">The logger.</param>
		/// <exception cref="Exception">Shutting Down</exception>
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IServiceProvider provider,ILogger<Startup> logger)
		{
			provider.CheckAndInstallTrustedCertificate();

			var p = Configuration["Db:Provider"];
			var db = provider.GetService<DevicesDbContext>() ?? throw new Exception("DevicesDbContext is not set in service provider");

			if (string.Compare(p, "mysql", true, CultureInfo.InvariantCulture) == 0
					|| string.Compare(p, "postgresql", true, CultureInfo.InvariantCulture) == 0
					|| string.Compare(p, "sqlserver", true, CultureInfo.InvariantCulture) == 0)
			{
				if(!db.WaitForServer(logger))
				{
					throw new Exception("Shutting Down");
				}
			}

			db.Database.Migrate();
			app.UseRouting();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseOpenApi();
			app.UseReDoc();

			app.UseEndpoints(i =>
			{
				i.MapControllers();
				i.MapHouseHealthChecks("/health");
				i.MapHouseRobotsTxt();
			});
		}
	}
}
