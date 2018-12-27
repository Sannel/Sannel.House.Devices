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
using Sannel.House.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Devices.Models
{
	public class PagedResults<TDevice> : IPagedResults<TDevice>
	{
		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public IEnumerable<TDevice> Data { get; set; }
		/// <summary>
		/// The Total Count
		/// </summary>
		/// <value>
		/// The total count.
		/// </value>
		public long TotalCount { get; set; }
		/// <summary>
		/// Gets or sets the page.
		/// </summary>
		/// <value>
		/// The page.
		/// </value>
		public long Page { get; set; }
		/// <summary>
		/// Gets or sets the size of the page.
		/// </summary>
		/// <value>
		/// The size of the page.
		/// </value>
		public int PageSize { get; set; }
	}
}
