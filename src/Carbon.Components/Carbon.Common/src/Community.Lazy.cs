using API.Analytics;
using API.Assembly;
using API.Commands;
using API.Contracts;
using API.Events;

namespace Carbon;

public partial class Community
{
	private readonly Lazy<IAnalyticsManager> _analyticsManager = new(GameObject.GetComponent<IAnalyticsManager>);
	private readonly Lazy<IAssemblyManager> _assemblyEx = new(GameObject.GetComponent<IAssemblyManager>);
	private readonly Lazy<ICommandManager> _commandManager = new(GameObject.GetComponent<ICommandManager>);
	private readonly Lazy<IDownloadManager> _downloadManager = new(GameObject.GetComponent<IDownloadManager>);
	private readonly Lazy<IEventManager> _eventManager = new(GameObject.GetComponent<IEventManager>);
	private readonly Lazy<ICompatManager> _compatManager = new(GameObject.GetComponent<ICompatManager>);
}
