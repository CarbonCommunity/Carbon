using Carbon.Pooling;
using UnityEngine;

public static class InternalHooks
{
	public static int OnEntitySaved;

	public static void Handle(string hookName, bool subscribed)
	{
		switch (hookName)
		{
			case nameof(OnEntitySaved):
			{
				OnEntitySaved += subscribed ? 1 : -1;
				break;
			}
		}
	}
}
