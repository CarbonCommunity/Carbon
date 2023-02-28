using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carbon.Hooks;

namespace Carbon.Contracts
{
	internal interface IHookManager
	{
		internal void Reload();

		internal bool enabled { get; set; }
	}
}
