using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Rust.Modular.EngineStorage ), "RefreshLoadoutData" )]
    public class OnEngineLoadoutRefresh
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEngineLoadoutRefresh" );
        }
    }
}