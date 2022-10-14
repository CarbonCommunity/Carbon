///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Core.Libraries
{
	public abstract class Library : IDisposable
	{
		public virtual void Dispose() { }
	}
}
