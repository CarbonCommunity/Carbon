public static class RustEx
{
	[Obsolete("Use BasePlayer.SendCompleteSnapshot instead.")]
	public static void SendFullSnapshot(this BasePlayer player) => player.SendCompleteSnapshot();

	[Obsolete("Use PlayerMetabolism.SendChanges instead.")]
	public static void SendChangesToClient(this PlayerMetabolism metabolism) => metabolism.SendChanges();

	[Obsolete("Use BaseEntity.StartSetFlags or BaseEntity.SetFlagLocal instead.")]
	public static void SetFlag(this BaseEntity entity, BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true)
	{
		using var flags = entity.StartSetFlags(networkupdate ? BaseEntity.FlagsUpdateMode.SendNetworkUpdate : BaseEntity.FlagsUpdateMode.Local);
		flags.Set(f, b, recursive);
	}
}
