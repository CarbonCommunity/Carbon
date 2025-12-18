using System.Linq;
using System.Threading.Tasks;
using API.Hooks;
using Carbon.Components;
using Carbon.Extensions;
using Carbon.Pooling;
using Carbon.Test;

namespace Carbon.Plugins;

public partial class Tests
{
    public class Hooks
    {
        internal static uint hookId = HookStringPool.GetOrAdd(nameof(TestingHook));
        internal static uint hook2Id = HookStringPool.GetOrAdd(nameof(ConflictTest));
        public static bool hasFired;

        [Integrations.Test.Assert]
        public void validate(Integrations.Test.Assert test)
        {
            test.IsTrue(singleton.HookPool.Count > 0, "singleton.HookPool.Count > 0");
        }

        [Integrations.Test.Assert]
        public void static_call(Integrations.Test.Assert test)
        {
            test.IsFalse(hasFired, "hasFired");
            test.IsTrue(hookId != 0, "hookId != 0");
            test.Log($"HookCaller.CallStaticHook({hookId}, test);");
            HookCaller.CallStaticHook(hookId, test);
            test.IsTrue(hasFired, "hasFired");
            test.IsFalse(hasFired = false, "hasFired reset");
        }

        [Integrations.Test.Assert]
        public void plugin_call(Integrations.Test.Assert test)
        {
            test.IsFalse(hasFired, "hasFired");
            test.IsTrue(hookId != 0, "hookId != 0");
            test.Log($"singleton.Call(\"{nameof(TestingHook)}\", test);");
            singleton.Call(nameof(TestingHook), test);
            test.IsTrue(hasFired, "hasFired");
            test.IsFalse(hasFired = false, "hasFired reset");
        }

        [Integrations.Test.Assert]
        public void conflict(Integrations.Test.Assert test)
        {
            test.IsFalse(hasFired, "hasFired");
            test.Log($"HookCaller.CallStaticHook({hook2Id}, test);");
            var result = (bool)HookCaller.CallStaticHook(hook2Id, test);
            test.IsTrue(result, $"(bool)HookCaller.CallStaticHook({hook2Id}, test);");
            test.IsTrue(hasFired, "hasFired");
            test.IsFalse(hasFired = false, "hasFired reset");
        }

        [Integrations.Test.Assert(Timeout = 6_000)]
        public async Task patch_all(Integrations.Test.Assert test)
        {
	        var hookManager = Community.Runtime.HookManager;

	        PrintOutHooksInfo();

	        foreach (var hook in hookManager.LoadedDynamicHooks)
	        {
		        hookManager.Subscribe(hook.HookName, nameof(Tests));
	        }

	        foreach (var hook in hookManager.LoadedPatches)
	        {
		        hookManager.Subscribe(hook.HookName, nameof(Tests));
	        }

	        foreach (var hook in hookManager.LoadedStaticHooks)
	        {
		        hookManager.Subscribe(hook.HookName, nameof(Tests));
	        }

	        hookManager.ForceUpdateHooks();

	        await AsyncEx.WaitForSeconds(.1f);

	        PrintOutHooksInfo();

	        // test.IsTrue(hookManager.LoadedPatches.Count() == hookManager.InstalledPatches.Count(), "loaded patches == all patches");
	        // test.IsTrue(hookManager.LoadedDynamicHooks.Count() == hookManager.InstalledDynamicHooks.Count(), "loaded dynamic hooks == all dynamic hooks");
	        // test.IsTrue(hookManager.LoadedStaticHooks.Count() == hookManager.InstalledStaticHooks.Count(), "loaded static hooks == all static hooks");

	        test.Complete();

	        return;

	        void PrintOutHooksInfo()
	        {
		        test.Log($"{nameof(IPatchManager.LoadedPatches)}: {hookManager.LoadedPatches.Count()}");
		        test.Log($"{nameof(IPatchManager.InstalledPatches)}: {hookManager.InstalledPatches.Count()}");
		        test.Log($"{nameof(IPatchManager.LoadedDynamicHooks)}: {hookManager.LoadedDynamicHooks.Count()}");
		        test.Log($"{nameof(IPatchManager.InstalledDynamicHooks)}: {hookManager.InstalledDynamicHooks.Count()}");
		        test.Log($"{nameof(IPatchManager.LoadedStaticHooks)}: {hookManager.LoadedStaticHooks.Count()}");
		        test.Log($"{nameof(IPatchManager.InstalledStaticHooks)}: {hookManager.InstalledStaticHooks.Count()}");
	        }
        }
    }

    private void TestingHook(Integrations.Test.Assert test)
    {
        test.Warn("TestingHook was fired");
        Hooks.hasFired = true;
    }

    private object ConflictTest(Integrations.Test.Assert test)
    {
        test.Log("ConflictTest was fired (True)");
        Hooks.hasFired = true;
        return true;
    }
}
