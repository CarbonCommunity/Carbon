namespace Carbon.Contracts;

public interface IZipScriptProcessor : IScriptProcessor, IDisposable
{
	public interface IZipScript : IProcess
	{
		IScriptLoader Loader { get; set; }
	}
}
