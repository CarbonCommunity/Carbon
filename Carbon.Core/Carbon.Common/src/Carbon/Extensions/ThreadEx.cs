namespace Carbon.Extensions;

public class ThreadEx
{
	internal static readonly Thread MainThread = Thread.CurrentThread;

	public static bool IsOnMainThread()
	{
		return Thread.CurrentThread == MainThread;
	}
}
