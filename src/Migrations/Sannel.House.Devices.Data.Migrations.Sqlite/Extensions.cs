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
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Devices.Data
{
	public static class Extensions
	{
		public static DbContextOptionsBuilder ConfigureSqlite(this DbContextOptionsBuilder option, string connectionString)
		=> option.UseSqlite(connectionString, i => i.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));

		public static DbContextOptionsBuilder ConfigureSqlite(this DbContextOptionsBuilder option, SqliteConnection connection)
			=> option.UseSqlite(connection, i => i.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
	}
}
