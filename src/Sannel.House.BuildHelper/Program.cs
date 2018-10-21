using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Sannel.House.BuildHelper
{
	class Program
	{
		static void Main(string[] args)
		{
			var arguments = new Arguments(args);
			if(arguments.HasArgument("preparedirectory"))
			{
				var dir = arguments.ArgumentValue("preparedirectory");
				dir = Path.GetFullPath(dir);
				if(Directory.Exists(dir))
				{
					Console.WriteLine($"Deleting {dir}");
					Directory.Delete(dir, true);
				}

				Console.WriteLine($"Creating {dir}");
				Directory.CreateDirectory(dir);
			}
			//else if(arguments.HasArgument("installcertificate"))
			//{
			//	var certFile = arguments.ArgumentValue("installcertificate");
			//	var cert = new X509Certificate2(certFile);
			//	using (var store = new X509Store(StoreName.AuthRoot, StoreLocation.LocalMachine))
			//	{
			//		store.Open(OpenFlags.ReadWrite);
			//		store.Add(cert);
			//		store.Close();
			//	}
			//}
			else
			{
				Console.WriteLine("No supported Arguments");
				foreach(var key in arguments.ArgumentValues.Keys)
				{
					Console.WriteLine($"\t{key}:{arguments.ArgumentValues[key]}");
				}
			}
		}
	}
}
