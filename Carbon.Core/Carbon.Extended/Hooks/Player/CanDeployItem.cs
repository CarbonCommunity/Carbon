using Carbon.Core;
using CompanionServer.Handlers;
using Harmony;
using Oxide.Core;
using ProtoBuf;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "CanDeployItem" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "this", typeof ( Deployer ) )]
    [Hook.Parameter ( "entityId", typeof ( uint ) )]
    [Hook.Info ( "Useful for denying items' deployment." )]
    [HarmonyPatch ( typeof ( Deployer ), "DoDeploy" )]
    public class Deployer_DoDeploy
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg, ref Deployer __instance )
        {
            if ( !msg.player.CanInteract () )
            {
                return true;
            }

            var deployable = __instance.GetDeployable ();

            if ( deployable == null )
            {
                return true;
            }

            var oldPosition = msg.read.Position;
            var ray = msg.read.Ray ();
            var num = msg.read.UInt32 ();
            msg.read.Position = oldPosition;

            return HookExecutor.CallStaticHook( "CanDeployItem", msg.player, __instance, num ) == null;
        }
    }
}