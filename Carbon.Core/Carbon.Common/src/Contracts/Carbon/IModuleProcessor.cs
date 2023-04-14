using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carbon.Base;
using Carbon.Hooks;

namespace Carbon.Contracts;

public interface IModuleProcessor : IDisposable
{
	void Init();
	void OnServerInit();
	void OnServerSave();
	void Setup(BaseHookable hookable);
	List<BaseHookable> Modules { get; }
}
