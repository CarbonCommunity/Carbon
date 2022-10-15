///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using Carbon;

internal sealed class Entrypoint : IHarmonyModHooks
{
	public void OnLoaded(OnHarmonyModLoadedArgs args)
	{
		/// The entrypoint for our loader is based on Facepunch's Harmony loader
		/// events. The only goal at this stage is create a second entrypoint
		/// later on the game bootstrap process.
		/// At this point in time we can't know in which position of the loading
		/// queue we are at thus executing the purge right now would result in 
		/// an undefined result.
	}

	public void OnUnloaded(OnHarmonyModUnloadedArgs args)
	{
		Loader.GetInstance().Dispose();
	}
}
