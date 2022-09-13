using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseNpc ), "WantsToEat" )]
    public class CanNpcEat [BaseNpc]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanNpcEat [BaseNpc]" );
        }
    }
}