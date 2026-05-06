namespace Carbon.Contracts;

public interface IZipDevScriptProcessor : IScriptProcessor, IDisposable
{
	public interface IZipDebugScript : IProcess
	{
		IScriptLoader Loader { get; set; }
	}
}
