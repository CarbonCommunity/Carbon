///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Carbon.Utility;
using Harmony;

namespace Carbon.Loader
{
	public class Entrypoint : IHarmonyModHooks
	{
		/// The entrypoint for our loader based on Rust's Harmony loader hooks.
		/// Our main objective here is to inject an entrypoint later on the 
		/// game's bootstrap process. We can't do the cleanup right now because
		/// it's undefined if all the Harmony scripts were already loaded at
		/// the time of execution of this event.
		public void OnLoaded(OnHarmonyModLoadedArgs args)
		{
			Logger.None(
				Environment.NewLine +
				"CARBON LOADER STARTING" + Environment.NewLine +
				Environment.NewLine
			);

			Program.GetInstance().DoSomething();
		}

		public void OnUnloaded(OnHarmonyModUnloadedArgs args) { }
	}
}