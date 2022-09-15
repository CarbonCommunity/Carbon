using Carbon.Core;
using CompanionServer.Handlers;
using Harmony;
using Oxide.Core;
using ProtoBuf;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMelee ), "PlayerAttack" )]
    public class BaseMelee_PlayerAttack
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg, ref BaseMelee __instance )
        {
            var player = msg.player;
            var result = true;
            var oldPosition = msg.read.Position;

            if ( !__instance.VerifyClientAttack ( player ) )
            {
                return true;
            }
            using ( var playerAttack = ProtoBuf.PlayerAttack.Deserialize ( msg.read ) )
            {
                if ( playerAttack != null )
                {
                    var hitInfo = Facepunch.Pool.Get<global::HitInfo> ();
                    hitInfo.LoadFromAttack ( playerAttack.attack, true );
                    hitInfo.Initiator = player;
                    hitInfo.Weapon = __instance;
                    hitInfo.WeaponPrefab = __instance;
                    hitInfo.Predicted = msg.connection;
                    hitInfo.damageProperties = __instance.damageProperties;
                    result = HookExecutor.CallStaticHook ( "OnMeleeAttack", player, hitInfo ) == null;
                    Facepunch.Pool.Free ( ref hitInfo );
                }
            }

            msg.read.Position = oldPosition;

            return result;
        }
    }
}