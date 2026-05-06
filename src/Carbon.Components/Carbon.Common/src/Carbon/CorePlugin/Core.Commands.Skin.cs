namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("skin", "Allowing you to get/change the skin ID of a deployed entity you're looking at."), AuthLevel(1)]
	private void Skin(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();
		var raycast = Physics.Raycast(player.eyes.HeadRay(), out var hit, 10f, ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
		var entity = raycast ? hit.GetEntity() : null;
		if (entity.IsValid())
		{
			arg.ReplyWith($"{entity} skin: {entity.skinID}");
			entity.skinID = arg.GetULong(0, entity.skinID);
			entity.SendNetworkUpdate();
			return;
		}
		arg.ReplyWith("Couldn't find entity");
	}
}
