namespace Carbon.Contracts;

public interface ICarbonProcessor : IDisposable
{
	object CurrentFrameLock { get; set; }
	List<Action> CurrentFrameQueue { get; set; }
	List<Action> PreviousFrameQueue { get; set; }
}
