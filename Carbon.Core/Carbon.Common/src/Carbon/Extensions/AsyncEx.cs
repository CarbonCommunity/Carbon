namespace Carbon.Extensions;

public class AsyncEx
{
	public static async Task NextTick()
	{
		var tcs = new TaskCompletionSource<bool>();

		Community.Runtime.CorePlugin.NextTick(() =>
		{
			tcs.SetResult(true);
		});

		await tcs.Task;
		tcs = null;
	}
	public static async Task NextFrame()
	{
		await NextTick();
	}
	public static async Task WaitForSeconds(float seconds)
	{
		await Task.Delay((int)(seconds * 1000f));
	}
}
