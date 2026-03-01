namespace Carbon.Extensions;

public class ThreadEx
{
	public static readonly Thread MainThread = Thread.CurrentThread;

	public static bool IsOnMainThread()
	{
		return Thread.CurrentThread == MainThread;
	}
}
