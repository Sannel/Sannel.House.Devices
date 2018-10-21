/*
Copyright 2018 Sannel Software, L.L.C.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sannel.House.BuildHelper
{
	/// <summary>
	/// This class parses command line arguments into Arguments, Values and NonArgumentValues. It expects arguments to be in the format --[name]=[value] or /[name]=[value]. Name can be a-z,A-Z,0-9,-,_. 
	/// </summary>
	public class Arguments
	{
		private Dictionary<string, string> argNames = new Dictionary<string, string>();
		private List<string> nonArgValues = new List<string>();
		/// <summary>
		/// Creates a new Arguments class but does not parse anything until the parse method is called
		/// </summary>
		public Arguments()
		{
		}

		/// <summary>
		/// Creates a new Arguments class and parses the provided array
		/// </summary>
		/// <param name="args">The array to parse</param>
		public Arguments(string[] args)
		{
			Parse(args);
		}

		/// <summary>
		/// Returns true if the specified name was passed as an argument false otherwise
		/// </summary>
		/// <param name="name">The name of the argument to look for</param>
		/// <returns>True if the argument was found false if not</returns>
		public bool HasArgument(string name)
		{
			return argNames.ContainsKey(name);
		}

		/// <summary>
		/// Returns a String representing the value passed to the argument name or null if none was passed or name does not exist
		/// </summary>
		/// <param name="name">The name of the argument to get the value for</param>
		/// <returns>The value passed to the argument or null</returns>
		public string ArgumentValue(string name)
		{
			if (argNames.ContainsKey(name))
			{
				return argNames[name];
			}

			return null;
		}

		/// <summary>
		/// Returns a dictionary containing the arguments parsed out by Parse.
		/// </summary>
		/// <returns></returns>
		public IReadOnlyDictionary<string, string> ArgumentValues 
			=> argNames;

		/// <summary>
		/// Returns an array of strings representing arguments that were not in the form --[name]=[value] or /[name]=[value]
		/// </summary>
		public IReadOnlyList<string> NonArgumentValues 
			=> this.nonArgValues;

		/// <summary>
		/// Parses the provided <paramref name="args"/> array into Arguments, Values and NonArgumentValues
		/// </summary>
		/// <param name="args">The arguments to parse</param>
		public void Parse(string[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}

			foreach (var a in args)
			{
				if(!string.IsNullOrEmpty(a))
				{
					var match = ArgumentNameRegex.Match(a);
					if (match.Success)
					{
						var name = match.Groups["name"].Value.ToLowerInvariant();
						if (a.Length > match.Length)
						{
							var value = a.Substring(match.Length);

							if (value.Length > 1 && value[0] == '"')
							{
								if (value[value.Length - 1] == '"')
								{
									value = value.Substring(1, value.Length - 2);
								}
							}
							
							argNames[name] = value;
						}
						else
						{
							argNames[name] = null;
						}
					}
					else
					{
						nonArgValues.Add(a);
					}
				}
			}
		}

		/// <summary>
		/// Parses the provided <paramref name="args"/> array into Arguments, Values and NonArgumentValues
		/// </summary>
		/// <param name="args">The arguments to parse</param>
		public Task ParseAsync(string[] args)
			=> Task.Run(() =>
				{
					Parse(args);
				});

		public readonly static Regex ArgumentNameRegex = new Regex("^\\s*(?<full>(--|-|/)(?<name>[a-zA-Z0-9-_?]+)[=]?)", 
			RegexOptions.IgnoreCase
			| RegexOptions.CultureInvariant
			| RegexOptions.IgnorePatternWhitespace
			| RegexOptions.Compiled
			);

	}
}
