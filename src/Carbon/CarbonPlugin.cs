namespace Carbon.Plugins;

public class CarbonPlugin : RustPlugin
{
	public CUI.Handler CuiHandler { get; set; }

	public override void Setup(string name, string author, VersionNumber version, string description)
	{
		base.Setup(name, author, version, description);

		CuiHandler = new CUI.Handler();
	}

	#region CUI

	public CUI CreateCUI()
	{
		return new CUI(CuiHandler);
	}

	#endregion

	#region Command Cooldown

	internal static Dictionary<BasePlayer, List<CooldownInstance>> CommandCooldownBuffer = [];

	public static bool IsCommandCooledDown(
		BasePlayer player, string command,
		int time,
		out float timeLeft,
		bool doCooldownIfNot = true,
		float appendMultiplier = 0.5f,
		bool doCooldownPenalty = false)
	{
		timeLeft = -1;
		if (time == 0 || player == null)
		{
			return false;
		}

		if (!CommandCooldownBuffer.TryGetValue(player, out var pairs))
		{
			CommandCooldownBuffer.Add(player, pairs = []);
		}

		var lookupCommand = pairs.FirstOrDefault(x => x.Command == command);
		if (lookupCommand == null)
		{
			pairs.Add(lookupCommand = new CooldownInstance { Command = command });
		}

		var timePassed = (DateTime.Now - lookupCommand.LastCall);
		if (timePassed.TotalMilliseconds >= time)
		{
			if (doCooldownIfNot)
			{
				lookupCommand.LastCall = DateTime.Now;
			}

			return false;
		}

		timeLeft = (float)((time - timePassed.TotalMilliseconds) * 0.001f);

		if (doCooldownPenalty)
		{
			lookupCommand.LastCall = lookupCommand.LastCall.AddMilliseconds(time * appendMultiplier);
		}

		return true;
	}

	internal sealed class CooldownInstance
	{
		public string Command;
		public DateTime LastCall;
	}

	#endregion
}
