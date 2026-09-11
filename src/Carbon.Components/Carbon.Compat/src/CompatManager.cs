using System.Diagnostics;
using System.Runtime.CompilerServices;
using API.Abstracts;
using API.Assembly;
using API.Events;
using AsmResolver;
using AsmResolver.DotNet.Serialized;
using Carbon.Compat.Converters;
using Carbon.Extensions;
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

public class CompatManager : CarbonBehaviour, ICompatManager
{
	private readonly BaseConverter oxideConverter = new OxideConverter();

	private readonly BaseConverter harmonyConverter = new HarmonyConverter();

	private static readonly ModuleReaderParameters readerArgs = new ModuleReaderParameters(EmptyErrorListener.Instance);

	private static readonly Version zeroVersion = new Version(0,0,0,0);

    public static readonly AssemblyReference SDK = new AssemblyReference("Carbon.SDK", zeroVersion);

    public static readonly AssemblyReference Common = new AssemblyReference("Carbon.Common", zeroVersion);

    public static readonly AssemblyReference Newtonsoft = new AssemblyReference("Newtonsoft.Json", zeroVersion);

    public static readonly AssemblyReference protobuf = new AssemblyReference("protobuf-net", zeroVersion);

    public static readonly AssemblyReference protobufCore = new AssemblyReference("protobuf-net.Core", zeroVersion);

    public static readonly AssemblyReference wsSharp = new AssemblyReference("websocket-sharp", zeroVersion);

    private bool ConvertAssembly(ModuleDefinition md, BaseConverter converter, ref byte[] buffer, bool noEntrypoint = false)
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

    ConversionResult ICompatManager.AttemptOxideConvert(ref byte[] data)
    {
	    ModuleDefinition asm = ModuleDefinition.FromBytes(data, readerArgs);

	    if (!asm.AssemblyReferences.Any(Helpers.IsOxideASM))
	    {
		    return ConversionResult.Skip;
	    }

	    return ConvertAssembly(asm, oxideConverter, ref data) ? ConversionResult.Success : ConversionResult.Fail;
    }

    bool ICompatManager.ConvertHarmonyMod(ref byte[] data, bool noEntrypoint)
    {
	    return ConvertAssembly(ModuleDefinition.FromBytes(data, readerArgs), harmonyConverter, ref data, noEntrypoint);
    }

    public void Init()
    {
	    Community.Runtime.Events.Subscribe(CarbonEvent.HookFetchStart, args =>
	    {
		    HookProcessor.HookClear();
	    });

	    Community.Runtime.Events.Subscribe(CarbonEvent.HookFetchEnd, args =>
	    {
		    HookProcessor.HookReload();
	    });
    }
}
