using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Contracts;

public interface ICarbonProcessor : IDisposable
{
	Queue<Action> OnFrameQueue { get; }
}
