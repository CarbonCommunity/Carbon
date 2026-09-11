namespace Carbon.Base;

public class BaseSource : ISource
{
	public string ContextFilePath { get; set; }
	public string ContextFileName { get; set; }
	public string FilePath { get; set; }
	public string FileName { get; set; }
	public string Content { get; set; }

	public void Dispose()
	{
		ContextFilePath = null;
		ContextFileName = null;
		FilePath = null;
		FileName = null;
		Content = null;
	}
}
