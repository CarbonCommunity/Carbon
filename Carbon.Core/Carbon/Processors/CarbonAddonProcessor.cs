using Facepunch;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Carbon.Core
{
    public class CarbonAddonProcessor
    {
        public List<Assembly> Addons { get; } = new List<Assembly> ();
        public Dictionary<string, HookInstance> Patches { get; } = new Dictionary<string, HookInstance> ();

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

        public void AppendHook ( string hookName )
        {
            if ( !DoesHookExist ( hookName ) ) return;

            if ( Patches.TryGetValue ( hookName, out var instance ) )
            {
                instance.Hooks++;
            }
        }
        public void UnappendHook ( string hookName )
        {
            if ( !DoesHookExist ( hookName ) ) return;

            if ( Patches.TryGetValue ( hookName, out var instance ) )
            {
                instance.Hooks--;

                if ( instance.Hooks <= 0 )
                {
                    CarbonCore.Warn ( $" No plugin is using '{hookName}'. Unpatching." );
                    UninstallHooks ( hookName );
                }
            }
        }

        public void InstallHooks ( string hookName )
        {
            if ( !DoesHookExist ( hookName ) ) return;
            CarbonCore.Log ( $" Found '{hookName}'." );

            foreach ( var addon in Addons )
            {
                foreach ( var type in addon.GetTypes () )
                {
                    var parameters = type.GetCustomAttributes<Hook.Parameter> ();
                    var hook = type.GetCustomAttribute<Hook> ();
                    var args = parameters == null || !parameters.Any () ? 0 : parameters.Count ();

                    if ( hook == null ) continue;

                    if ( hook.Name == hookName )
                    {
                        var patch = type.GetCustomAttribute<HarmonyPatch> ();
                        var hookInstance = ( HookInstance )null;

                        if ( !Patches.TryGetValue ( hookName, out hookInstance ) )
                        {
                            Patches.Add ( hookName, hookInstance = new HookInstance () );
                        }

                        var prefix = type.GetMethod ( "Prefix" );
                        var postfix = type.GetMethod ( "Postfix" );
                        var transplier = type.GetMethod ( "Transplier" );
                        var patchId = $"{hook.Name}.{args}";

                        if ( hookInstance.Patches.Any ( x => x.Id == patchId ) ) continue;

                        var originalMethodParameters = Pool.GetList<Type> ();
                        foreach(var param in prefix.GetParameters () )
                        {
                            if ( !param.ParameterType.IsByRef && !param.IsOut ) originalMethodParameters.Add ( param.ParameterType );
                        }
                        var originalMethodParametersResult = originalMethodParameters.ToArray ();

                        var instance = HarmonyInstance.Create ( patchId );
                        instance.Patch ( patch.info.declaringType.GetMethod ( patch.info.methodName, originalMethodParametersResult ),
                            prefix: prefix == null ? null : new HarmonyMethod ( prefix ),
                            postfix: postfix == null ? null : new HarmonyMethod ( postfix ),
                            transpiler: transplier == null ? null : new HarmonyMethod ( transplier ) );
                        hookInstance.Patches.Add ( instance );
                        CarbonCore.Log ( $" Patched '{hookName}'[{args}]..." );

                        Pool.FreeList ( ref originalMethodParameters );
                        Pool.Free ( ref originalMethodParametersResult );
                    }
                }
            }
        }
        public void UninstallHooks ( string hookName )
        {
            if ( Patches.TryGetValue ( hookName, out var list ) )
            {
                foreach ( var patch in list.Patches )
                {
                    patch.UnpatchAll ();
                }

                list.Patches.Clear ();
            }
        }

        public class HookInstance
        {
            public int Hooks { get; set; } = 0;
            public List<HarmonyInstance> Patches { get; } = new List<HarmonyInstance> ();
        }
    }
}