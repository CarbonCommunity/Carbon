using System.Diagnostics;
using System.Runtime.CompilerServices;
using Carbon.Events;
using AsmResolver;
using AsmResolver.DotNet.Serialized;
using Carbon.Compat.Converters;
using Carbon.Extensions;
using Carbon.Managers;
using Facepunch;
using Defines = Carbon.Core.Defines;

[assembly: InternalsVisibleTo("Carbon.Bootstrap")]

namespace Carbon.Compat;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public class CompatManager : FacepunchBehaviour
{
	private readonly BaseConverter harmonyConverter = new HarmonyConverter();

	public static readonly ModuleReaderParameters readerArgs = new ModuleReaderParameters(EmptyErrorListener.Instance);

	private static readonly Version zeroVersion = new Version(0,0,0,0);

    public static readonly AssemblyReference SDK = new AssemblyReference("Carbon.SDK", zeroVersion);

    public static readonly AssemblyReference Common = new AssemblyReference("Carbon.Common", zeroVersion);

    public static readonly AssemblyReference Newtonsoft = new AssemblyReference("Newtonsoft.Json", zeroVersion);

    public static readonly AssemblyReference protobuf = new AssemblyReference("protobuf-net", zeroVersion);

    public static readonly AssemblyReference protobufCore = new AssemblyReference("protobuf-net.Core", zeroVersion);

    public static readonly AssemblyReference wsSharp = new AssemblyReference("websocket-sharp", zeroVersion);

    /// <summary>Runs <paramref name="converter"/> over an assembly, replacing <paramref name="buffer"/> with the result.</summary>
    public static bool ConvertAssembly(ModuleDefinition md, BaseConverter converter, ref byte[] buffer, bool noEntrypoint = false)
    {
	    Stopwatch stopwatch = Pool.Get<Stopwatch>();
	    stopwatch.Restart();

	    md.DebugData.Clear();

	    BaseConverter.Context context = default;
	    context.Buffer = buffer;
	    context.NoEntrypoint = noEntrypoint;

	    try
	    {
		    buffer = converter.Convert(md, context); //, out BaseConverter.GenInfo info);
	    }
	    catch (Exception ex)
	    {
		    Logger.Error($"Failed to convert assembly {md.Name}", ex);
		    buffer = null;
			stopwatch.Reset();
			Pool.FreeUnsafe(ref stopwatch);
			return false;
	    }

	    if (buffer == context.Buffer)
	    {
		    Logger.Log($"{converter.Name} assembly doesn't need any conversion [for '{md.Name}'], skipping..");
	    }
	    else
	    {
		    Logger.Log($"{converter.Name} assembly conversion for '{md.Name}' took {stopwatch.ElapsedMilliseconds:0}ms");
	    }

	    stopwatch.Reset();
	    Pool.FreeUnsafe(ref stopwatch);

#if DEBUG
	    string dir = Path.Combine(Defines.GetTempFolder(), "compat_debug_gen");
	    Directory.CreateDirectory(dir);
	    OsEx.File.Create(Path.Combine(dir, md.Name), buffer);
#endif
	    return true;
    }

    internal bool ConvertHarmonyMod(ref byte[] data, bool noEntrypoint = false)
    {
	    return ConvertAssembly(ModuleDefinition.FromBytes(data, readerArgs), harmonyConverter, ref data, noEntrypoint);
    }

    private void Awake()
    {
	    Services.Assemblies.Converters.Add(new HarmonyModConverter(this));

	    Services.Events.Subscribe(CarbonEvent.HookFetchStart, _ => HookProcessor.HookClear());
	    Services.Events.Subscribe(CarbonEvent.HookFetchEnd, _ => HookProcessor.HookReload());
    }

    /// <summary>Makes Rust HarmonyMods loadable under Carbon.</summary>
    private sealed class HarmonyModConverter(CompatManager compat) : AssemblyConverter
    {
	    public override string Name => "HarmonyMod";
	    public override bool Handles(AddonType type) => type == AddonType.HarmonyMod;
	    public override ConversionResult Convert(string file, AddonType type, ref byte[] raw)
		    => compat.ConvertHarmonyMod(ref raw) ? ConversionResult.Success : ConversionResult.Fail;
    }
}
