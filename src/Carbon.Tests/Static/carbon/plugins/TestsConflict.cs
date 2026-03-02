// Requires: Tests

using Carbon.Test;

namespace Carbon.Plugins;

[Info("TestConflict", "Carbon Community LTD", "1.0.0")]
public partial class TestsConflict : CarbonPlugin
{ 
	private object ConflictTest(Integrations.Test.Assert test)
	{
		test.Log("ConflictTest was fired (False) [TestConflict]");
		Tests.Hooks.hasFired = true;
		return false;
	} 
}