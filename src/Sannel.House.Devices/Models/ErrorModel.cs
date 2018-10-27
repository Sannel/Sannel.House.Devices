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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sannel.House.Devices.Models
{
	public class ErrorModel
	{
		public ErrorModel(string key, string value) 
			=> Errors.Add(key, value);

		public ErrorModel(ModelStateDictionary modelState)
		{
			foreach(var k in modelState)
			{
				Errors.Add(k.Key, string.Join("\n", k.Value?.Errors.Select(i => i.ErrorMessage)));
			}
		}

		public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
	}
}
