///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Patterns;
using Carbon.Utility;

namespace Carbon.Loader
{
	internal class Program : Singleton<Program>
	{
		static Program() { }

		public void DoSomething()
		{
			Logger.Log("I've done something");
		}
	}
}