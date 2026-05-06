using System;
using System.IO;
using System.Reflection;
using System.Security;
using Carbon.Core;
using Doorstop.Utility;

namespace Doorstop;

[SuppressUnmanagedCodeSecurity]
public sealed class Entrypoint
{
	public static void Start()
	{
		Defines.Initialize();
		Config.Init();

		if (Config.Singleton.SelfUpdating.Enabled)
		{
			try
			{
				SelfUpdater.Init();
				SelfUpdater.GetCarbonVersions();
				SelfUpdater.Execute();
			}
			catch (Exception ex)
			{
				Logger.Error("Failed self-updating process! Report to developers.", ex);
			}
		}
		else
		{
			Logger.Log(" Skipped self-updating process as it's disabled in the config.");
		}

		try
		{
			var startup = Assembly.Load(File.ReadAllBytes(Path.Combine(Defines.GetManagedFolder(), "Carbon.Startup.dll")));
			var endpointType = startup.GetType("Startup.Entrypoint");
			if (endpointType.GetMethod("Start", BindingFlags.Static | BindingFlags.Public) is MethodInfo method)
			{
				method.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed Entrypoint.Startup! Report to developers.", ex.InnerException);
		}
	}
}
