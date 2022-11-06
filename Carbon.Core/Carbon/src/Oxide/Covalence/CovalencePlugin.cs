///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

using System.Collections.Generic;
using System.Linq;
using Carbon.Oxide;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
	public class CovalencePlugin : RustPlugin
	{
		public PlayerManager players = new PlayerManager();

		public struct PlayerManager : IPlayerManager
		{
			public IEnumerable<IPlayer> All => BasePlayer.allPlayerList.Select(x => x.AsIPlayer() as IPlayer);

			public IEnumerable<IPlayer> Connected => BasePlayer.activePlayerList.Select(x => x.AsIPlayer() as IPlayer);

			public IPlayer FindPlayer(string partialNameOrId)
			{
				return BasePlayer.FindAwakeOrSleeping(partialNameOrId).AsIPlayer() as IPlayer;
			}

			public IPlayer FindPlayerById(string id)
			{
				return BasePlayer.FindAwakeOrSleeping(id).AsIPlayer();
			}

			public IPlayer FindPlayerByObj(object obj)
			{
				return BasePlayer.FindAwakeOrSleeping(obj.ToString()).AsIPlayer();
			}

			public IEnumerable<IPlayer> FindPlayers(string partialNameOrId)
			{
				return BasePlayer.allPlayerList.Where(x => x.displayName.Contains(partialNameOrId) || x.UserIDString == partialNameOrId).Select(x => x.AsIPlayer() as IPlayer);
			}
		}
	}
}
