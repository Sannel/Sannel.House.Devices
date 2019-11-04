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
using System.Data.Common;
using System.IO;
using System.Linq;
using Sannel.House.Data;
using Sannel.House.Web;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sannel.House.Devices.Data;
using Sannel.House.Devices.Data.Migrations.Sqlite;
using Sannel.House.Devices.Data.Migrations.SqlServer;
using Sannel.House.Devices.Data.Migrations.PostgresSQL;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Repositories;
using System.Net;
using System.Net.Security;
using System.Net.Http;
using NSwag.AspNetCore;
using Microsoft.Extensions.Hosting;
using System.Globalization;

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
			services.AddDbContextPool<DevicesDbContext>(o =>
			{
				switch (Configuration["Db:Provider"])
				{
					case "MySQL":
					case "mysql":
						throw new NotSupportedException("We are currently not supporting mysql as a database provider");

					case "sqlserver":
					case "SqlServer":
						o.ConfigureSqlServer(Configuration["Db:ConnectionString"]);
						break;
					case "PostgreSQL":
					case "postgresql":
						o.ConfigurePostgreSQL(Configuration["Db:ConnectionString"]);
						break;
					case "sqlite":
					default:
						o.ConfigureSqlite(Configuration["Db:ConnectionString"]);
						break;
				}
			});

			services.AddScoped<IDeviceRepository, DbContextRepository>();


			services.AddAuthentication(Configuration["Authentication:Schema"])
					.AddIdentityServerAuthentication(Configuration["Authentication:Schema"], o =>
						{
							o.Authority = this.Configuration["Authentication:AuthorityUrl"];
							o.ApiName = this.Configuration["Authentication:ApiName"];
							o.SupportedTokens = SupportedTokens.Both;
							if (Configuration.GetValue<bool?>("Authentication:DisableRequireHttpsMetadata") == true)
							{
								o.RequireHttpsMetadata = false;
							}
						});

			services.AddSwaggerDocument();
			services.AddOpenApiDocument();

			services.AddAuthorization();
			services.AddControllers();

			services.AddHealthChecks()
				.AddDbHealthCheck<DevicesDbContext>("DbHealthCheck", async (context) =>
				{
					await context.Devices.Take(1).CountAsync();
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
			var db = provider.GetService<DevicesDbContext>();

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
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHealthChecks("/health");
			app.UseRouting();


			app.UseAuthentication();
			app.UseAuthorization();

			//app.UseSwaggerUi3();
			//app.UseOpenApi();

			app.UseEndpoints(i =>
			{
				i.MapControllers();
			});

		}
	}
}
