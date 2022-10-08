///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

using System;

namespace Carbon.Core.Modules
{
	public class AutoWipeModule : CarbonModule<AutoWipeConfig, AutoWipeData>
	{
		public override string Name => "AutoWipe";
		public override Type Type => typeof(AutoWipeModule);
	}

	public class AutoWipeConfig
	{
		public int NextWipeSeed = -1;
	}
	public class AutoWipeData
	{

	}
}
