using Carbon.Core;
using ConVar;
using Harmony;
using Oxide.Core;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( KeyLock ), "Lock" )]
    public class KeyLock_Lock
    {
        public static bool Prefix ( BasePlayer player, ref KeyLock __instance )
        {
            if ( player == null )
            {
                return true;
            }

            if ( !player.CanInteract () )
            {
                return true;
            }

            if ( __instance.IsLocked () )
            {
                return true;
            }

            return HookExecutor.CallStaticHook ( "CanLock", player, __instance ) == null;
        }
    }

    [HarmonyPatch ( typeof ( CodeLock ), "TryLock" )]
    public class CodeLock_Lock
    {
        public static bool Prefix ( BaseEntity.RPCMessage rpc, ref CodeLock __instance )
        {
            if ( !rpc.player.CanInteract () )
            {
                return true;
            }
            if ( __instance.IsLocked () )
            {
                return true;
            }
            if ( __instance.code.Length != 4 )
            {
                return true;
            }

            return HookExecutor.CallStaticHook ( "CanLock", rpc.player, __instance ) == null;
        }
    }
}