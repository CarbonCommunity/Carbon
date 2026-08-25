public static class RustEx
{
	public static void SendFullSnapshot(this BasePlayer player) => player.SendCompleteSnapshot();
	public static void SendChangesToClient(this PlayerMetabolism metabolism) => metabolism.SendChanges();
	public static void SetFlag(this BaseEntity entity, BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true)
	{
		using var flags = entity.StartSetFlags(networkupdate ? BaseEntity.FlagsUpdateMode.SendNetworkUpdate : BaseEntity.FlagsUpdateMode.Local);
		flags.Set(f, b, recursive);
	}
}
