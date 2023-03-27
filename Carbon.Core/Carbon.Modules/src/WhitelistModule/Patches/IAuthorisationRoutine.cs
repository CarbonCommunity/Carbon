using System.Collections;
using API.Hooks;
using Network;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class WhitelistModule
{
	[HookAttribute.Patch("IAuthorisationRoutine", "IAuthorisationRoutine", typeof(ConnectionAuth), "AuthorisationRoutine", new System.Type[] { typeof(Connection) })]
	[HookAttribute.Identifier("f188a3b34dcb43278f1997df11471111")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class ConnectionAuth_AuthorisationRoutine_f188a3b34dcb43278f1997df11471111 : API.Hooks.Patch
	{
		internal static WhitelistModule Whitelist = GetModule<WhitelistModule>();

		public static bool Prefix(Connection connection, ConnectionAuth __instance, ref IEnumerator __result)
		{
			__result = AuthPatch(connection, __instance);
			return false;
		}
		private static IEnumerator AuthPatch(Connection connection, ConnectionAuth auth)
		{
			yield return auth.StartCoroutine(Auth_Steam.Run(connection));
			yield return auth.StartCoroutine(Auth_EAC.Run(connection));
			yield return auth.StartCoroutine(Auth_CentralizedBans.Run(connection));

			if (!Whitelist.CanBypass(connection.userid.ToString()) && Whitelist.IsPasswordEnabled())
			{
				yield return auth.StartCoroutine(Whitelist.Run(connection));
			}

			if (!connection.rejected && connection.active)
			{
				if (auth.IsAuthed(connection.userid))
				{
					ConnectionAuth.Reject(connection, "You are already connected as a player!");
				}
				else
				{
					auth.Approve(connection);
				}
			}
		}
	}
}
