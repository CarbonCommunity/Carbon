namespace API.Assembly;

public interface IFileWatcherManager
{
	public void Watch(WatchFolder item);
	public void Unwatch(string directory);
}
