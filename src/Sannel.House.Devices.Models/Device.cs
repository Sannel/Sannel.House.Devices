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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sannel.House.Devices.Models
{
	public class Device
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int DeviceId { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[MaxLength(256)]
		[Required]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[MaxLength(2000)]
		[Required]
		public string Description { get; set; } = string.Empty;


		/// <summary>
		/// Gets or sets the display order.
		/// </summary>
		/// <value>
		/// The display order.
		/// </value>
		public int DisplayOrder { get; set; }

		/// <summary>
		/// Gets or sets the date created.
		/// </summary>
		/// <value>
		/// The date created.
		/// </value>
		public DateTimeOffset DateCreated { get; set; }

		/// <summary>
		/// Gets or sets if this device is readonly
		/// </summary>
		public bool IsReadOnly { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Device"/> is verified.
		/// </summary>
		/// <value>
		///   <c>true</c> if verified; otherwise, <c>false</c>.
		/// </value>
		public bool Verified { get; set; }
	}
}
