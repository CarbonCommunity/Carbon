///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;

namespace Oxide.Core.Libraries
{
	public abstract class Library : IDisposable
	{
		public virtual void Dispose() { }
	}
}
