using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMountable ), "MountPlayer" )]
    public class OnEntityMounted
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityMounted" );
        }
    }
}