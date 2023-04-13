using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carbon.Base;
using Carbon.Hooks;

namespace Carbon.Contracts;

public interface ICommandProcessor : IDisposable
{
	bool RegisterCommand(BaseCommand command, out string reason);
	bool UnregisterCommand(string command, out string reason);
}
