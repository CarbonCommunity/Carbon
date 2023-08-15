using Carbon.Client;

namespace Carbon.Extensions;

public static class ClientEx
{
	public static CarbonClient ToCarbonClient(this BasePlayer player)
	{
		return CarbonClient.Get(player);
	}
}
