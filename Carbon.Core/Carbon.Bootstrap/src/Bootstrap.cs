using System.Reflection;
using Carbon.Utility;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon;

public static class Bootstrap
{
	public static void Initialize()
	{
		string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
		Logger.Log($"{assemblyName} loaded.");
		Legacy.Loader.GetInstance().Initialize();

		References.Load();
	}
}
