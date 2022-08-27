using Harmony;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using UnityEngine;
using System.Linq;
using Oxide.Plugins;
using Facepunch;
using JSON;
using System.Runtime.Serialization;

public static class RexideLoader
{
    public static void LoadRexideMods ()
    {
        try
        {
            HarmonyInstance.DEBUG = true;
            string path = Path.Combine ( Rexide.GetLogsFolder (), ".." );
            FileLog.logPath = Path.Combine ( path, "rexide_log.txt" );
            try
            {
                File.Delete ( FileLog.logPath );
            }
            catch
            {
            }
            _modPath = Rexide.GetPluginsFolder ();
            if ( !Directory.Exists ( _modPath ) )
            {
                try
                {
                    Directory.CreateDirectory ( _modPath );
                    return;
                }
                catch
                {
                    return;
                }
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
                    LoadRexideMod ( text, true );
                }
            }
        }
        finally
        {
            FileLog.FlushBuffer ();
        }
    }
    public static void UnloadRexideMods ()
    {
        var list = Facepunch.Pool.GetList<RexideMod> ();
        list.AddRange ( _loadedMods );

        foreach ( var mod in list )
        {
            UnloadRexideMod ( mod.Name );
        }

        Facepunch.Pool.FreeList ( ref list );
    }
    public static bool LoadRexideMod ( string name, bool silent = false )
    {
        Log ( name, $"Pre load..." );

        var fileName = Path.GetFileName ( name );

        if ( fileName.EndsWith ( ".dll" ) )
        {
            fileName = fileName.Substring ( 0, fileName.Length - 4 );
        }

        UnloadRexideMod ( fileName, silent );

        var fullPath = Path.Combine ( _modPath, fileName + ".dll" );
        var domain = "com.rust.rexide." + fileName;

        Log ( domain, "Processing..." );

        try
        {
            Assembly assembly = LoadAssembly ( fullPath );
            if ( assembly == null )
            {
                LogError ( domain, $"Failed to load harmony mod '{fileName}.dll' from '{_modPath}'" );
                return false;
            }
            var mod = new RexideMod
            {
                Assembly = assembly,
                AllTypes = assembly.GetTypes (),
                Name = fileName
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
                    catch ( Exception arg ) { LogError ( mod.Name, string.Format ( "Failed to create hook instance {0}", arg ) ); }
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
                    hook.OnLoaded ( new OnHarmonyModLoadedArgs () );
                }
                catch ( Exception arg3 )
                {
                    LogError ( mod.Name, string.Format ( "Failed to call hook 'OnLoaded' {0}", arg3 ) );
                }
            }

            Log ( mod.Name, $"Processing plugin..." );
            ProcessPlugin ( mod );
            Log ( mod.Name, $"Processed." );

            _loadedMods.Add ( mod );
        }
        catch ( Exception e )
        {
            LogError ( domain, "Failed to load: " + fullPath );
            ReportException ( domain, e );
            return false;
        }
        return true;
    }
    public static bool UnloadRexideMod ( string name, bool silent = false )
    {
        RexideMod mod = GetMod ( name );
        if ( mod == null )
        {
            if ( !silent ) LogError ( "Couldn't unload mod '" + name + "': not loaded" );
            return false;
        }
        foreach ( IHarmonyModHooks harmonyModHooks in mod.Hooks )
        {
            try
            {
                harmonyModHooks.OnUnloaded ( new OnHarmonyModUnloadedArgs () );
            }
            catch ( Exception arg )
            {
                LogError ( mod.Name, string.Format ( "Failed to call hook 'OnLoaded' {0}", arg ) );
            }
        }
        UnloadMod ( mod );
        return true;
    }

    #region Rexide

    internal static FileSystemWatcher _folderWatcher { get; set; }

    public static void ProcessPlugin ( RexideMod mod )
    {
        foreach ( var type in mod.AllTypes )
        {
            try
            {
                if ( !type.FullName.StartsWith ( "Oxide.Plugins" ) ) return;

                if ( type == null || !type.IsSubclassOf ( typeof ( RustPlugin ) ) ) continue;

                var instance = Activator.CreateInstance ( type, true );

                type?.GetMethod ( "SetupMod", BindingFlags.Instance | BindingFlags.Public )?.Invoke ( instance, new object [] { mod } );
                type?.GetMethod ( "OnServerInitialized", BindingFlags.Instance | BindingFlags.NonPublic )?.Invoke ( instance, null );

                var plugin = instance as RustPlugin;
                mod.Plugins.Add ( plugin );
                _processMethods ( plugin );

                Debug.Log ( $"Loaded: {mod.Name}" );
            }
            catch ( Exception ex ) { Rexide.Error ( $"Failed loading '{mod.Name}'", ex ); }
        }
    }
    public static void StalkPluginFolder ()
    {
        if ( _folderWatcher != null )
        {
            _folderWatcher.Dispose ();
            _folderWatcher = null;
        }

        _folderWatcher = new FileSystemWatcher ( Rexide.GetPluginsFolder () )
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.LastAccess,
            Filter = "*.dll"
        };
        _folderWatcher.Changed += _onChanged;
        _folderWatcher.Created += _onChanged;
        _folderWatcher.Renamed += ( sender, e ) =>
        {
            UnloadRexideMod ( e.OldFullPath, true );
            LoadRexideMod ( e.FullPath );
        };
        _folderWatcher.Deleted += _onRemoved;
        _folderWatcher.EnableRaisingEvents = true;
    }

    internal static void _processMethods ( RustPlugin plugin )
    {
        var type = plugin.GetType ();
        var methods = type.GetMethods ( BindingFlags.NonPublic | BindingFlags.Instance );

        foreach ( var method in methods )
        {
            var chatCommand = method.GetCustomAttribute<ChatCommandAttribute> ();
            var consoleCommand = method.GetCustomAttribute<ConsoleCommandAttribute> ();

            if ( chatCommand != null )
            {
                Rexide.Instance.CorePlugin.cmd.AddChatCommand ( chatCommand.Name, plugin, method.Name );
            }

            if ( consoleCommand != null )
            {
                Rexide.Instance.CorePlugin.cmd.AddConsoleCommand ( consoleCommand.Name, plugin, method.Name );
            }
        }

        Facepunch.Pool.Free ( ref methods );
    }

    internal static void _onChanged ( object sender, FileSystemEventArgs e )
    {
        LoadRexideMod ( e.FullPath, true );
    }
    internal static void _onRemoved ( object sender, FileSystemEventArgs e )
    {
        UnloadRexideMod ( e.FullPath );
    }

    #endregion

    internal static void UnloadMod ( RexideMod mod )
    {
        Log ( mod.Name, "Unpatching hooks..." );
        mod.Harmony.UnpatchAll ( null );
        _loadedMods.Remove ( mod );
        Log ( mod.Name, "Unloaded mod" );
    }
    internal static RexideMod GetMod ( string name )
    {
        return _loadedMods.FirstOrDefault ( ( RexideMod x ) => x.Name.Equals ( name, StringComparison.OrdinalIgnoreCase ) );
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
        Rexide.Log ( $"[{harmonyId}] {message}" );
    }
    internal static void LogError ( string harmonyId, object message )
    {
        Rexide.Error ( $"[{harmonyId}] {message}" );
    }
    internal static void LogError ( object message )
    {
        Rexide.Error ( message );
    }

    internal static string _modPath;

    internal static List<RexideMod> _loadedMods = new List<RexideMod> ();

    public class RexideMod
    {
        public string Name { get; set; }
        public HarmonyInstance Harmony { get; set; }
        public Assembly Assembly { get; set; }
        public Type [] AllTypes { get; set; }
        public List<IHarmonyModHooks> Hooks { get; } = new List<IHarmonyModHooks> ();
        public List<RustPlugin> Plugins { get; set; } = new List<RustPlugin> ();
    }
}