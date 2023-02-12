using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2021 BepInEx, released under MIT License
 * All rights reserved.
 *
 */

namespace Carbon.Utility;

internal class References
{
	public static void Load()
	{
		#region Modules
		LoadAssembly(Path.Combine(Context.CarbonManaged, "Carbon.API.dll"));
		#endregion

		#region Libs
		LoadAssembly(Path.Combine(Context.CarbonLib, "MySql.Data.dll"));
		LoadAssembly(Path.Combine(Context.CarbonLib, "System.Data.SQLite.dll"));
		#endregion
	}

	private static void LoadAssembly(string filePath)
	{
		Logger.Log($">> Loading reference: {filePath}");

		Assembly.Load(File.ReadAllBytes(filePath));
	}
}
