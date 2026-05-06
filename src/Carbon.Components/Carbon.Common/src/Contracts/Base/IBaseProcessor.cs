namespace Carbon.Contracts;

public interface IBaseProcessor
{
	void Start();

	T Get<T>(string fileName) where T : IProcess;

	Dictionary<string, IProcess> InstanceBuffer { get; set; }
	List<string> IgnoreList { get; set; }

	public string Name { get; }
	public string Folder { get; }
	public string Extension { get; }

	void Prepare(string path);
	void Prepare(string name, string path);
	void Ignore(string path);
	bool Exists(string path);
	void Clear(IEnumerable<string> except = null);
	void ClearIgnore(string path);
	bool IsBlacklisted(string path);
	bool IncludeSubdirectories { get; set; }

	void RefreshRate();

	public interface IProcess : IDisposable
	{
		bool IsRemoved { get; }
		bool IsDirty { get; }

		string File { get; set; }

		void Clear();
		void Execute(IBaseProcessor processor);
		void MarkDirty();
		void MarkDeleted();
	}

	public interface IParser
	{
		void Process(string file, string input, out string output);
	}
}
