using System.Collections.Generic;
using API.Assembly;

namespace API.Events;

public class ModuleEventArgs : CarbonEventArgs
{
	public IModulePackage ModulePackage;
	public IEnumerable<object> Data;

	public void Init(object payload, IModulePackage modulePackage, IEnumerable<object> data)
	{
		Init(payload);
		ModulePackage = modulePackage;
		Data = data;
	}

	public override void EnterPool()
	{
		base.EnterPool();

		ModulePackage = null;
		Data = null;
	}
}
