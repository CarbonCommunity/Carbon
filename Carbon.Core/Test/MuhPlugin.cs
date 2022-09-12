using Carbon.Core;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info ( "MuhPlugin", "Carbon Test Plugin", "1.0.0" )]
    [Description ( "This is a mock description." )]
    public class MuhPlugin : RustPlugin
    {
        public static MuhPlugin Instance;

        [PluginReference]
        public Plugin Locker;

        [PluginReference]
        public Plugin PortableLocker;

        private void OnServerInitialized ()
        {
            PortableLocker?.Call ( "OpenLocker", BasePlayer.Find ( "Raul" ), null );

            Instance = this;
    
            PatchPlugin ( Assembly.GetExecutingAssembly () );

            Puts ( $"{Assembly.GetExecutingAssembly ()?.GetTypes () [ 0 ]?.FullName}" );
            Puts ( $"{permission.GroupExists ( "default" )} {permission.PermissionExists ( "portablelocker.use" )}" );

            timer.In ( 1f, () =>
            {
                Puts ( $"heh?" );
                Oxide.Core.Interface.CallHook ( "ayaya" );
                Oxide.Core.Interface.CallHook ( "MyHookTest" );
            } );

            webrequest.Enqueue ( "https://codefling.com/capi/category-2/?do=apicall", null, ( error, result ) =>
            {
                Puts ( $"This worked :D {result.Length}" );
            }, this );
        }

        [HookMethod ( "ayaya" )]
        public void MyHookTest ()
        {
            CarbonCore.Log ( $"Howdy partner" );
        }

        private void Unload ()
        {
            UnpatchPlugin ();
        }

        private object OnHammerHit ( BasePlayer player, HitInfo info )
        {
            player.ChatMessage ( $"You bonked {info.HitEntity}" );
            Puts ( $"{player} bonked {info.HitEntity}     " );
            info.HitEntity.Kill ();
            return true;
        }

        [ChatCommand ( "cmdtest" )]
        private void CmdTest ( BasePlayer player, string command, string [] args )
        {
            player.ChatMessage ( "MOVEE" );
            player.MovePosition ( player.ServerPosition + ( Vector3.up * 10f ) );
        }

        [ConsoleCommand ( "cmdtest" )]
        private void CmdTest2 ( ConsoleSystem.Arg arg )
        {
            var player = arg.Player ();
            player.ChatMessage ( "MOVEEd" );
            player.MovePosition ( player.ServerPosition + ( Vector3.up * 10f ) );
        }

        [ChatCommand ( "mystuff" )]
        private void DoStuffs ( BasePlayer player, string command, string [] args )
        {
            player.ChatMessage ( $"Yeee" );
        }

        private void OnPlayerInput ( BasePlayer player, InputState input )
        {
            // Puts ( $"{player} {input.current.buttons}" );
        }
    }

    [HarmonyPatch ( typeof ( BasePlayer ), "OnReceiveTick" )]
    public class BasePlayer_OnReceiveTick
    {
        public static void Prefix ( PlayerTick msg, bool wasPlayerStalled, ref BasePlayer __instance )
        {
            HookExecutor.CallStaticHook ( "OnPlayerInput", __instance, __instance.serverInput );
        }
    }

    // [HarmonyPatch ( typeof ( BaseProjectile ), "CLProject" )]
    // public class BaseProjectile_CLProject
    // {
    //     public static void Prefix ( BaseEntity.RPCMessage msg, ref BaseProjectile __instance )
    //     {
    //         HookExecutor.CallStaticHook ( "OnWeaponFired", __instance, msg.player, __instance.PrimaryMagazineAmmo.GetComponent<ItemModProjectile> (), null );
    //     }
    // }
}