using Facepunch;
using Harmony;
using Humanlights.Extensions;
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
                    try
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

                            var matchedParameters = GetMatchedParameters ( patch.info.declaringType, patch.info.methodName, prefix.GetParameters () );
                            var instance = HarmonyInstance.Create ( patchId );
                            instance.Patch ( patch.info.declaringType.GetMethod ( patch.info.methodName, matchedParameters ),
                                prefix: prefix == null ? null : new HarmonyMethod ( prefix ),
                                postfix: postfix == null ? null : new HarmonyMethod ( postfix ),
                                transpiler: transplier == null ? null : new HarmonyMethod ( transplier ) );
                            hookInstance.Patches.Add ( instance );
                            hookInstance.Id = patchId;
                            CarbonCore.Log ( $" Patched '{hookName}'[{args}]..." );

                            Pool.Free ( ref matchedParameters );
                        }
                    }
                    catch ( Exception exception )
                    {
                        CarbonCore.Error ( $"Couldn't patch hook '{hookName}' ({type.FullName})", exception );
                    }
                }
            }

        }
        public void UninstallHooks ( string hookName )
        {
            if ( Patches.TryGetValue ( hookName, out var instance ) )
            {
                foreach ( var patch in instance.Patches )
                {
                    patch.UnpatchAll ( instance.Id );
                }

                instance.Patches.Clear ();
            }
        }

        internal Type [] GetMatchedParameters ( Type type, string methodName, ParameterInfo [] parameters )
        {
            var list = Pool.GetList<Type> ();

            foreach ( var method in type.GetMethods ( BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic ) )
            {
                if ( method.Name != methodName ) continue;

                var @params = method.GetParameters ();

                for ( int i = 0; i < @params.Length; i++ )
                {
                    try
                    {
                        var param = @params [ i ];
                        var otherParam = parameters [ i ];
                        if ( param.Name == otherParam.Name && param.ParameterType.FullName == otherParam.ParameterType.FullName )
                        {
                            list.Add ( param.ParameterType );
                        }
                    }
                    catch { }
                }
            }

            var result = list.ToArray ();
            Pool.FreeList ( ref list );
            return result;
        }

        public class HookInstance
        {
            public string Id { get; set; }
            public int Hooks { get; set; } = 0;
            public List<HarmonyInstance> Patches { get; } = new List<HarmonyInstance> ();
        }
    }
}