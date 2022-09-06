using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info ( "MuhPlugin", "Carbon Test Plugin", "1.0.0" )]
    [Description ( "This is a mock description." )]
    public class MuhPlugin : RustPlugin
    {
        private void OnServerInitialized ()
        {
            Puts ( $"I'm in >:)as dasd  " );

            var counter = 1;
            var timeTest = timer.Every ( 0.25f, () =>
            {
                Puts ( $"YEEET" );
            } );
            timer.In ( 2f, () =>
            {
                timeTest.Destroy ();
            } );
        }

        private object OnHammerHit ( BasePlayer player, HitInfo info )
        {
            player.ChatMessage ( $"You bonked {info.HitEntity}" );
            Puts ( $"{player} bonked {info.HitEntity}     " );
            info.HitEntity.Kill ();
            return true;
        }

        [ChatCommand ( "mystuff" )]
        private void DoStuffs ( BasePlayer player, string command, string [] args )
        {
            player.ChatMessage ( $"Yeee" );
        }
    }
}