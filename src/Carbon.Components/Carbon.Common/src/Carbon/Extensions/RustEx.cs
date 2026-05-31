public static class RustEx
{
	public static void SendFullSnapshot(this BasePlayer player) => player.SendCompleteSnapshot();
	public static void SendChangesToClient(this PlayerMetabolism metabolism) => metabolism.SendChanges();
}
