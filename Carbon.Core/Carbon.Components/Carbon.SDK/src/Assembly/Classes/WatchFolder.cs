using System.IO;

namespace API.Assembly;

public class WatchFolder
{
	public bool InitialEvent = true;

	public bool IncludeSubFolders { get; set; }
	public FileSystemWatcher Handler { get; set; }
	public string Directory { get; set; }
	public string Extension { get; set; }

	public OnFileChangedEvent OnFileChanged { get; set; }
	public delegate void OnFileChangedEvent(object sender, string path);

	public OnFileCreatedEvent OnFileCreated { get; set; }
	public delegate void OnFileCreatedEvent(object sender, string path);

	public OnFileDeletedEvent OnFileDeleted { get; set; }
	public delegate void OnFileDeletedEvent(object sender, string path);

	public void TriggerAll(WatcherChangeTypes type)
	{
		foreach (string file in System.IO.Directory.GetFiles(Handler.Path, Handler.Filter))
		{
			switch (type)
			{
				case WatcherChangeTypes.Created:
					OnFileCreated.Invoke(Handler, file);
					break;

				case WatcherChangeTypes.Changed:
					OnFileChanged.Invoke(Handler, file);
					break;

				case WatcherChangeTypes.Deleted:
					OnFileDeleted.Invoke(Handler, file);
					break;
			}
		}

		Handler.EnableRaisingEvents = true;
	}
}
