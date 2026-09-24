using System;
using System.Threading.Tasks;
using Carbon.Test;
using Network;
using UnityEngine;
using PlayerLibrary = Oxide.Game.Rust.Libraries.Player;
using RustPlayer = Oxide.Game.Rust.Libraries.Covalence.RustPlayer;

namespace Carbon.Plugins;

public partial class Tests
{
	public class PlayerTests
	{
		[Integrations.Test.Assert(Timeout = 5000)]
		public Task library_rename_preserves_player_state(Integrations.Test.Assert test)
		{
			return VerifyRename(test, (player, name) => new PlayerLibrary().Rename(player, name));
		}

		[Integrations.Test.Assert(Timeout = 5000)]
		public Task covalence_rename_preserves_player_state(Integrations.Test.Assert test)
		{
			return VerifyRename(test, (player, name) => new RustPlayer(player).Rename(name));
		}

		[Integrations.Test.Assert(Timeout = 5000)]
		public void rename_without_connection_does_not_throw(Integrations.Test.Assert test)
		{
			var player = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", new Vector3(0, 1000, 0)) as BasePlayer;
			try
			{
				player.enableSaving = false;
				player.Spawn();

				new PlayerLibrary().Rename(player, "SleeperLibrary");
				test.IsTrue(player.displayName == "SleeperLibrary", "library rename updated display name");

				new RustPlayer(player).Rename("SleeperCovalence");
				test.IsTrue(player.displayName == "SleeperCovalence", "covalence rename updated display name");
			}
			finally
			{
				if (player != null && !player.IsDestroyed)
				{
					player.Kill();
				}
			}
			test.Complete();
		}

		private static async Task VerifyRename(Integrations.Test.Assert test, Action<BasePlayer, string> rename)
		{
			var parent = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", new Vector3(0, 1000, 0));
			var player = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", new Vector3(0, 1000, 0)) as BasePlayer;
			try
			{
				parent.enableSaving = false;
				parent.Spawn();
				player.enableSaving = false;
				player.Spawn();
				player.net.OnConnected(new Connection());
				player.SetParent(parent);
				var position = player.transform.position;

				rename(player, "RenamedPlayer");
				test.IsTrue(player.displayName == "RenamedPlayer", "display name updated");
				test.IsTrue(player.net.connection.username == "RenamedPlayer", "connection name updated");
				test.IsTrue(player.GetParentEntity() == parent, "parent preserved");
				test.IsTrue(player.transform.position == position, "position preserved");

				player.syncPosition = false;
				player.limitNetworking = true;
				rename(player, "HiddenPlayer");
				test.IsFalse(player.syncPosition, "disabled position updates preserved");
				test.IsTrue(player.limitNetworking, "existing networking restriction preserved");

				player.limitNetworking = false;
				player.syncPosition = true;
				rename(player, "FirstRename");
				rename(player, "SecondRename");
				test.IsTrue(player.syncPosition, "rename does not pause position updates");
				test.IsFalse(player.limitNetworking, "rename does not pause networking");
				player.limitNetworking = true;
				player.syncPosition = false;

				var nextTick = new TaskCompletionSource<bool>();
				Oxide.Core.Interface.Oxide.NextTick(() => nextTick.SetResult(true));
				await nextTick.Task;
				test.IsTrue(player.limitNetworking, "later networking restriction preserved");
				test.IsFalse(player.syncPosition, "later position update setting preserved");
			}
			finally
			{
				if (player != null && !player.IsDestroyed)
				{
					player.SetParent(null, true, false);
					player.net?.OnDisconnected();
					player.Kill();
				}
				if (parent != null && !parent.IsDestroyed) parent.Kill();
			}
			test.Complete();
		}
	}
}
