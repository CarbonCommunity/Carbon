using Harmony;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using UnityEngine;
using Oxide.Plugins;
using Humanlights.Extensions;

namespace Carbon.Core
{
    public static class CarbonLoader
    {
        public static List<Assembly> AssemblyCache { get; } = new List<Assembly> ();

        public static void LoadCarbonMods ()
        {
            try
            {
                HarmonyInstance.DEBUG = true;
                var path = Path.Combine ( CarbonCore.GetLogsFolder (), ".." );
                FileLog.logPath = Path.Combine ( path, "carbon_log.txt" );
                try
                {
                    File.Delete ( FileLog.logPath );
                }
                catch { }

                _modPath = CarbonCore.GetPluginsFolder ();
                if ( !Directory.Exists ( _modPath ) )
                {
                    try
                    {
                        Directory.CreateDirectory ( _modPath );
                        return;
                    }
                    catch { return; }
                }

                AppDomain.CurrentDomain.AssemblyResolve += delegate ( object sender, ResolveEventArgs args )
                {
                    Debug.Log ( "Trying to load assembly: " + args.Name );
                    AssemblyName assemblyName = new AssemblyName ( args.Name );
                    string text2 = Path.Combine ( _modPath, assemblyName.Name + ".dll" );
                    if ( !File.Exists ( text2 ) )
                    {
                        return null;
                    }
                    return LoadAssembly ( text2 );
                };

                foreach ( string text in Directory.EnumerateFiles ( _modPath, "*.dll" ) )
                {
                    if ( !string.IsNullOrEmpty ( text ) && !IsKnownDependency ( Path.GetFileNameWithoutExtension ( text ) ) )
                    {
                        CarbonCore.Instance.HarmonyProcessor.Prepare ( text );
                    }
                }
            }
            catch ( Exception ex )
            {
                CarbonCore.Error ( "Loading all DLLs failed.", ex );
            }
            finally
            {
                FileLog.FlushBuffer ();
            }
        }
        public static void UnloadCarbonMods ()
        {
            var list = Facepunch.Pool.GetList<CarbonMod> ();
            list.AddRange ( _loadedMods );

            foreach ( var mod in list )
            {
                if ( mod.IsCoreMod ) continue;

                UnloadCarbonMod ( mod.Name );
            }

            Facepunch.Pool.FreeList ( ref list );
        }
        public static bool LoadCarbonMod ( string name, bool silent = false )
        {
            var fileName = Path.GetFileName ( name );

            if ( fileName.EndsWith ( ".dll" ) )
            {
                fileName = fileName.Substring ( 0, fileName.Length - 4 );
            }

            var temp = GetTempModPath ( fileName );
            fileName = Path.GetFileName ( temp );

            if ( fileName.EndsWith ( ".dll" ) )
            {
                fileName = fileName.Substring ( 0, fileName.Length - 4 );
            }

            UnloadCarbonMod ( fileName, silent );

            var fullPath = temp;
            var domain = "com.rust.carbon." + fileName;

            try
            {
                Assembly assembly = LoadAssembly ( fullPath );
                if ( assembly == null )
                {
                    LogError ( domain, $"Failed to load harmony mod '{fileName}.dll' from '{_modPath}'" );
                    return false;
                }
                var mod = new CarbonMod
                {
                    Assembly = assembly,
                    AllTypes = assembly.GetTypes (),
                    Name = fileName,
                    File = temp
                };

                foreach ( var type in mod.AllTypes )
                {
                    if ( typeof ( IHarmonyModHooks ).IsAssignableFrom ( type ) )
                    {
                        try
                        {
                            var harmonyModHooks = Activator.CreateInstance ( type ) as IHarmonyModHooks;

                            if ( harmonyModHooks == null ) LogError ( mod.Name, "Failed to create hook instance: Is null" );
                            else mod.Hooks.Add ( harmonyModHooks );
                        }
                        catch ( Exception arg ) { LogError ( mod.Name, $"Failed to create hook instance {arg}" ); }
                    }
                }

                mod.Harmony = HarmonyInstance.Create ( domain );

                try
                {
                    mod.Harmony.PatchAll ( assembly );
                }
                catch ( Exception arg2 )
                {
                    LogError ( mod.Name, string.Format ( "Failed to patch all hooks: {0}", arg2 ) );
                    return false;
                }

                foreach ( var hook in mod.Hooks )
                {
                    try
                    {
                        var type = hook.GetType ();
                        if ( type.Name.Equals ( "CarbonInitializer" ) ) continue;

                        hook.OnLoaded ( new OnHarmonyModLoadedArgs () );
                    }
                    catch ( Exception arg3 )
                    {
                        LogError ( mod.Name, string.Format ( "Failed to call hook 'OnLoaded' {0}", arg3 ) );
                    }
                }

                AssemblyCache.Add ( assembly );
                _loadedMods.Add ( mod );

                InitializePlugin ( mod );
            }
            catch ( Exception e )
            {
                LogError ( domain, "Failed to load: " + fullPath );
                ReportException ( domain, e );
                return false;
            }
            return true;
        }
        public static bool UnloadCarbonMod ( string name, bool silent = false )
        {
            var mod = GetMod ( name );
            if ( mod == null )
            {
                return false;
            }
            foreach ( var hook in mod.Hooks )
            {
                try
                {
                    var type = hook.GetType ();
                    if ( type.Name.Equals ( "CarbonInitializer" ) ) continue;

                    hook.OnUnloaded ( new OnHarmonyModUnloadedArgs () );
                }
                catch ( Exception arg )
                {
                    LogError ( mod.Name, $"Failed to call hook 'OnLoaded' {arg}" );
                }
            }

            UnloadMod ( mod );
            UninitializePlugin ( mod );
            return true;
        }

        #region Carbon

        public static string GetTempModPath ( string name )
        {
            var temp = Path.Combine ( CarbonCore.GetTempFolder (), $"{name}_{Guid.NewGuid ()}.dll" );
            OsEx.File.Copy ( Path.Combine ( CarbonCore.GetPluginsFolder (), $"{name}.dll" ), temp, true );

            return temp;
        }

        public static void InitializePlugin ( CarbonMod mod )
        {
            mod.IsAddon = CarbonCore.IsAddon ( mod.Name );

            if ( mod.IsAddon )
            {
                Log ( mod.Name, "Initialized Carbon extension." );
            }

            foreach ( var type in mod.AllTypes )
            {
                try
                {
                    if ( !type.FullName.StartsWith ( "Oxide.Plugins" ) ) return;

                    if ( !IsValidPlugin ( type ) ) continue;

                    var instance = Activator.CreateInstance ( type, false );
                    var plugin = instance as RustPlugin;
                    var info = type.GetCustomAttribute<InfoAttribute> ();
                    var description = type.GetCustomAttribute<DescriptionAttribute> ();

                    if ( info == null )
                    {
                        CarbonCore.Warn ( $"Failed loading '{type.Name}'. The plugin doesn't have the Info attribute." );
                        continue;
                    }

                    plugin.CallPublicHook ( "SetupMod", mod, info.Title, info.Author, info.Version, description == null ? string.Empty : description.Description );
                    HookExecutor.CallStaticHook ( "OnPluginLoaded", plugin );
                    plugin.Init ();
                    plugin.DoLoadConfig ();
                    plugin.CallHook ( "OnServerInitialized" );

                    mod.Plugins.Add ( plugin );
                    ProcessCommands ( type, plugin );

                    CarbonCore.Log ( $"Loaded plugin {plugin}" );
                }
                catch ( Exception ex ) { CarbonCore.Error ( $"Failed loading '{mod.Name}'", ex ); }
            }
        }
        public static void UninitializePlugin ( CarbonMod mod )
        {
            foreach ( var plugin in mod.Plugins )
            {
                try
                {
                    HookExecutor.CallStaticHook ( "OnPluginUnloaded", plugin );
                    plugin.CallHook ( "Unload" );
                    RemoveCommands ( plugin );

                    CarbonCore.Log ( $"Unloaded plugin {plugin}" );
                }
                catch ( Exception ex ) { CarbonCore.Error ( $"Failed unloading '{mod.Name}'", ex ); }
            }
        }

        public static bool IsValidPlugin ( Type type )
        {
            if ( type == null ) return false;
            if ( type.Name == "RustPlugin" ) return true;

            return IsValidPlugin ( type.BaseType );
        }

        public static void ProcessCommands ( Type type, RustPlugin plugin = null, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance, string prefix = null )
        {
            var methods = type.GetMethods ( flags );

            foreach ( var method in methods )
            {
                var chatCommand = method.GetCustomAttribute<ChatCommandAttribute> ();
                var consoleCommand = method.GetCustomAttribute<ConsoleCommandAttribute> ();

                if ( chatCommand != null )
                {
                    CarbonCore.Instance.CorePlugin.cmd.AddChatCommand ( string.IsNullOrEmpty ( prefix ) ? chatCommand.Name : $"{prefix}.{chatCommand.Name}", plugin, method.Name );
                }

                if ( consoleCommand != null )
                {
                    CarbonCore.Instance.CorePlugin.cmd.AddConsoleCommand ( string.IsNullOrEmpty ( prefix ) ? consoleCommand.Name : $"{prefix}.{consoleCommand.Name}", plugin, method.Name );
                }
            }

            Facepunch.Pool.Free ( ref methods );
        }
        public static void RemoveCommands ( RustPlugin plugin )
        {
            CarbonCore.Instance.AllChatCommands.RemoveAll ( x => x.Plugin == plugin );
            CarbonCore.Instance.AllConsoleCommands.RemoveAll ( x => x.Plugin == plugin );
        }

        #endregion

        internal static void UnloadMod ( CarbonMod mod )
        {
            if ( mod.IsCoreMod ) return;

            if ( mod.Harmony != null )
            {
                Log ( mod.Name, "Unpatching hooks..." );
                mod.Harmony.UnpatchAll ( mod.Harmony.Id );
                Log ( mod.Name, "Unloaded mod" );

                OsEx.File.Delete ( mod.File );
            }

            _loadedMods.Remove ( mod );
        }
        internal static CarbonMod GetMod ( string name )
        {
            foreach ( var mod in _loadedMods )
            {
                if ( mod.Name.StartsWith ( name, StringComparison.OrdinalIgnoreCase ) ) return mod;
            }

            return null;
        }
        internal static Assembly LoadAssembly ( string assemblyPath )
        {
            if ( !File.Exists ( assemblyPath ) )
            {
                return null;
            }
            byte [] rawAssembly = File.ReadAllBytes ( assemblyPath );
            string path = assemblyPath.Substring ( 0, assemblyPath.Length - 4 ) + ".pdb";
            if ( File.Exists ( path ) )
            {
                byte [] rawSymbolStore = File.ReadAllBytes ( path );
                return Assembly.Load ( rawAssembly, rawSymbolStore );
            }
            return Assembly.Load ( rawAssembly );
        }
        internal static bool IsKnownDependency ( string assemblyName )
        {
            return assemblyName.StartsWith ( "System.", StringComparison.InvariantCultureIgnoreCase ) || assemblyName.StartsWith ( "Microsoft.", StringComparison.InvariantCultureIgnoreCase ) || assemblyName.StartsWith ( "Newtonsoft.", StringComparison.InvariantCultureIgnoreCase ) || assemblyName.StartsWith ( "UnityEngine.", StringComparison.InvariantCultureIgnoreCase );
        }

        internal static void ReportException ( string harmonyId, Exception e )
        {
            LogError ( harmonyId, e );
            ReflectionTypeLoadException ex;
            if ( ( ex = ( e as ReflectionTypeLoadException ) ) != null )
            {
                LogError ( harmonyId, string.Format ( "Has {0} LoaderExceptions:", ex.LoaderExceptions ) );
                foreach ( Exception e2 in ex.LoaderExceptions )
                {
                    ReportException ( harmonyId, e2 );
                }
            }
            if ( e.InnerException != null )
            {
                LogError ( harmonyId, "Has InnerException:" );
                ReportException ( harmonyId, e.InnerException );
            }
        }
        internal static void Log ( string harmonyId, object message )
        {
            CarbonCore.Format ( $"[{harmonyId}] {message}" );
        }
        internal static void LogError ( string harmonyId, object message )
        {
            CarbonCore.ErrorFormat ( $"[{harmonyId}] {message}" );
        }
        internal static void LogError ( object message )
        {
            CarbonCore.Error ( message );
        }

        internal static string _modPath;

        internal static List<CarbonMod> _loadedMods = new List<CarbonMod> ();

        public class CarbonMod
        {
            public string Name { get; set; } = string.Empty;
            public string File { get; set; } = string.Empty;
            public bool IsCoreMod { get; set; } = false;
            public bool IsAddon { get; set; } = false;
            public HarmonyInstance Harmony { get; set; }
            public Assembly Assembly { get; set; }
            public Type [] AllTypes { get; set; }
            public List<IHarmonyModHooks> Hooks { get; } = new List<IHarmonyModHooks> ();
            public List<RustPlugin> Plugins { get; set; } = new List<RustPlugin> ();
        }
    }
}