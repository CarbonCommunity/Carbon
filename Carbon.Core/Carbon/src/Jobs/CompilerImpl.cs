using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Carbon.Core;
using Carbon.Extensions;
using Path = System.IO.Path;

namespace Carbon.Jobs;

#pragma warning disable CS0208
#pragma warning disable CS8500

public interface ICompiler
{
    public (bool, CompilerResult) Compile(CompilerArgs args);

    public void InjectReference(string name, byte[] data);
}

internal sealed class MonoCompiler : ICompiler
{
    public unsafe (bool, CompilerResult) Compile(CompilerArgs args)
    {
        CompilerResult result = new CompilerResult();

        return (Compiler.ScriptCompiler.CompileAssemblyManaged(&args, &result), result);
    }

    public void InjectReference(string name, byte[] data)
    {
	    throw new NotImplementedException();
    }
}

internal static class CompilerManager
{
    public static ICompiler Compiler;

    public static void Init(bool useNetCore)
    {
        if (useNetCore)
        {
	        if (NetCoreCompiler.Initialize())
	        {
		        Logger.Log("CoreCLR Runtime initialized");
		        Compiler = new NetCoreCompiler();
		        goto end;
	        }
	        Logger.Error("Failed to initialize CoreCLR Runtime... Falling back to mono");
        }
        Compiler = new MonoCompiler();
        end:
        CompilerManager.Compiler.Compile(new CompilerArgs());
    }
}

[SuppressUnmanagedCodeSecurity]
internal sealed unsafe class NetCoreCompiler : ICompiler
{
    [DllImport("carbon_native")]
    private static extern bool initialize_runtime([MarshalAs(UnmanagedType.LPUTF8Str)] string runtime, [MarshalAs(UnmanagedType.LPUTF8Str)] string asm);
    [DllImport("carbon_native")]
    private static extern bool compile_script(ref MonoCompilerArgs p_args);
    [DllImport("carbon_native")]
    private static extern bool register_mono_callbacks(delegate*<API.Logger.Severity, int, char*, int, void> logger,
                                                       delegate*<ref MonoCompilerResult, void> compiler);
    [DllImport("carbon_native")]
    private static extern bool inject_reference(char* str_ptr, int str_len, byte* data_ptr, int data_len);
    private static void CompilerLog(API.Logger.Severity level, int verbosity, char* ptr, int len)
    {
	    string msg = Encoding.Unicode.GetString((byte*)ptr, len*2);
        Logger.Write(level, $"[CoreCLR Compiler] {msg}", verbosity: verbosity);
    }
    private static void CompilerCallback(ref MonoCompilerResult result)
    {
	    byte[] data;
	    if (result.data_len > 0)
	    {
		    data = new byte[result.data_len];
		    fixed (byte* be = data)
		    {
			    Buffer.MemoryCopy(result.data_ptr, be, data.Length, result.data_len);
		    }
		    Logger.Log($"Got {data.Length} bytes from CoreCLR");
		    result.Interop->data = data;
	    }
    }

    public static bool Initialize()
    {
	    string runtime_path = Defines.GetCompilerFolder();
	    string asm_path = Path.Combine(Defines.GetManagedFolder(), "Carbon.Compiler.dll");
	    if (!Directory.Exists(runtime_path) || !OsEx.File.Exists(asm_path))
	    {
		    return false;
	    }
        bool valid;
        try
        {
	        valid = initialize_runtime(runtime_path, asm_path);
        }
        catch (Exception ex)
        {
	        Logger.Error("NetCore runtime init failed", ex);
	        return false;
        }
        if (valid)
        {
	        register_mono_callbacks(&CompilerLog, &CompilerCallback);
        }

        return valid;
    }

    public unsafe (bool, CompilerResult) Compile(CompilerArgs args)
    {
        CompilerResult result = new CompilerResult();
        MonoCompilerArgs m_args = new MonoCompilerArgs();

        CompilerInterop interop = new CompilerInterop();
        m_args.interop = &interop;
        bool valid = compile_script(ref m_args);
        result.data = interop.data;
        return (valid, result);
    }

    public void InjectReference(string name, byte[] data)
    {
	    fixed (byte* be = data)
	    {
		    fixed (char* ch = name)
		    {
			    inject_reference(ch, name.Length, be, data.Length);
		    }
	    }
    }
}

public struct CompilerArgs
{
    // filename/array or strings or something idk
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoCompilerArgs
{
	public struct dstr
	{
		public char* ptr;
		public int length;
	}

	public CompilerInterop* interop;
	public dstr* ptr;
	public int length;
}

public struct CompilerResult
{
	public byte[] data;
}

public struct CompilerInterop
{
	public byte[] data;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoCompilerResult
{
	public CompilerInterop* Interop;
	public byte* data_ptr;
	public int data_len;
	// byte array and stuff
}
