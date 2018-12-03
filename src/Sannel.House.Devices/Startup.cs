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
using Sannel.House.Devices.Data.Migrations.MySql;
using Sannel.House.Devices.Data.Migrations.Sqlite;
using Sannel.House.Devices.Data.Migrations.SqlServer;
using Sannel.House.Devices.Interfaces;
using Sannel.House.Devices.Repositories;
using System.Net;
using System.Net.Security;
using System.Net.Http;

namespace Sannel.House.Devices
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			var root = (ConfigurationRoot)configuration;

			foreach(var p in root.Providers)
			{
				if(p is JsonConfigurationProvider j)
				{
					Console.WriteLine(Path.Combine(((PhysicalFileProvider)j.Source.FileProvider).Root, j.Source.Path));
				}
			}

			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			switch (Configuration["Db:Provider"])
			{
				case "mysql":
					services.AddDbContext<DevicesDbContext, Data.Migrations.MySql.MySqlDbContext>(o => {
						o.UseMySql(Configuration["Db:ConnectionString"]);
					});
					break;
				case "sqlserver":
					services.AddDbContext<DevicesDbContext, SqlServerDbContext>(o =>
						o.UseSqlServer(Configuration["Db:ConnectionString"]));
					break;
				case "sqlite":
				default:
					services.AddDbContext<DevicesDbContext, SqliteDbContext>(o =>
						o.UseSqlite(Configuration["Db:ConnectionString"]));
					break;
			}

			services.AddScoped<IDeviceRepository, DbContextRepository>();


			services.AddAuthentication("houseapi")
					.AddIdentityServerAuthentication("houseapi", o =>
						{
							o.Authority = this.Configuration["Authentication:AuthorityUrl"];
							o.ApiName = this.Configuration["Authentication:ApiName"];
							o.SupportedTokens = SupportedTokens.Both;
						});

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env,IServiceProvider provider,ILogger<Startup> logger)
		{
			provider.CheckAndInstallTrustedCertificate();

			var p = Configuration["Db:Provider"];
			var db = provider.GetService<DevicesDbContext>();

			if (string.Compare(p, "mysql", true) == 0
					|| string.Compare(p, "sqlserver", true) == 0)
			{
				if(!db.WaitForServer(logger))
				{
					throw new Exception("Shutting down");
				}
			}

			db.Database.Migrate();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();
			app.UseHttpsRedirection();
			app.UseMvc();

		}
	}
}
