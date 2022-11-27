using Carbon.Core;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Core
{
	public partial class Core_ServerMgr
	{
		/*
		[Hook.AlwaysPatched]
		[Hook.Parameter("booted", typeof(bool), true)]
		[Hook.Info("Called after the server startup has been completed and is awaiting connections.")]
		[Hook.Info("Also called for plugins that are hotloaded while the server is already started running.")]
		[Hook.Info("Boolean parameter, false if called on plugin hotload and true if called on server initialization.")]
		[Hook("OnServerInitialized"), Hook.Category(Hook.Category.Enum.Server)]
		[Hook.Patch(typeof(ServerMgr), "OpenConnection")]
		*/

		public class Core_ServerMgr_OpenConnection_b91c13017e4a43fcb2d81244efd8e5b6
		{
			public static Metadata metadata = new Metadata("OnServerInitialized",
					typeof(ServerMgr), "OpenConnection", new System.Type[] { });

			static Core_ServerMgr_OpenConnection_b91c13017e4a43fcb2d81244efd8e5b6()
			{
				metadata.SetAlwaysPatch(true);
			}

			public static void Postfix()
			{
				Loader.OnPluginProcessFinished();
			}
		}
	}
}