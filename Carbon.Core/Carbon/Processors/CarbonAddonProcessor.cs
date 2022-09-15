using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Core
{
    public class CarbonAddonProcessor
    {
        public List<Assembly> Addons { get; } = new List<Assembly> ();
        public Dictionary<string, List<HarmonyInstance>> Patches { get; } = new Dictionary<string, List<HarmonyInstance>> ();

        public bool DoesHookExist ( string hookName )
        {
            foreach ( var addon in Addons )
            {
                foreach ( var type in addon.GetTypes () )
                {
                    var hook = type.GetCustomAttribute<Hook> ();
                    if ( hook == null ) continue;

                    if ( hook.Name == hookName ) return true;
                }
            }

            return false;
        }

        public void InstallHooks ( string hookName )
        {
            if ( !DoesHookExist ( hookName ) ) return;
            CarbonCore.Log ( $" Found '{hookName}'." );

            foreach ( var addon in Addons )
            {
                foreach ( var type in addon.GetTypes () )
                {
                    var hook = type.GetCustomAttribute<Hook> ();
                    if ( hook == null ) continue;

                    if ( hook.Name == hookName )
                    {
                        var patch = type.GetCustomAttribute<HarmonyPatch> ();
                        var list = ( List<HarmonyInstance> )null;

                        if ( !Patches.TryGetValue ( hookName, out list ) )
                        {
                            Patches.Add ( hookName, list = new List<HarmonyInstance> () );
                        }

                        var instance = HarmonyInstance.Create ( hook.Name );
                        var prefix = type.GetMethod ( "Prefix" );
                        var postfix = type.GetMethod ( "Postfix" );
                        var transplier = type.GetMethod ( "Transplier" );
                        instance.Patch ( patch.info.declaringType.GetMethod ( patch.info.methodName ),
                            prefix: prefix == null ? null : new HarmonyMethod ( prefix ),
                            postfix: postfix == null ? null : new HarmonyMethod ( postfix ),
                            transpiler: transplier == null ? null : new HarmonyMethod ( transplier ) );
                        list.Add ( instance );
                        CarbonCore.Log ( $" Patched '{hookName}'..." );
                    }
                }
            }
        }
        public void UninstallHooks ( string hookName )
        {
            if ( Patches.TryGetValue ( hookName, out var list ) )
            {
                foreach ( var patch in list )
                {
                    patch.UnpatchAll ( hookName );
                }
            }
        }
    }
}