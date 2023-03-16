using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carbon.Base;
using Carbon.Hooks;

namespace Carbon.Contracts
{
	public interface IModuleProcessor : IDisposable
	{
		void Init();
		void OnServerInit();
		List<BaseHookable> Modules { get; }
	}
}
