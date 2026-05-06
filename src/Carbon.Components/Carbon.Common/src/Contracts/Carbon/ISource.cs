namespace Carbon.Contracts;

public interface ISource : IDisposable
{
	string ContextFilePath { get; set; }
	string ContextFileName { get; set; }
	string FilePath { get; set; }
	string FileName { get; set; }
	string Content { get; set; }
}
