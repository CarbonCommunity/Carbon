using System;
using System.Collections.Generic;

namespace Carbon.Contracts;

public interface ICarbonProcessor : IDisposable
{
	Queue<Action> OnFrameQueue { get; }
}
