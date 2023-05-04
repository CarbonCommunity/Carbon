using System;
using System.Collections.Generic;
using Carbon.Base;

namespace Carbon.Contracts;

public interface IModuleProcessor : IDisposable
{
	void Init();
	void OnServerInit();
	void OnServerSave();
	void Setup(BaseHookable hookable);
	List<BaseHookable> Modules { get; }
}
