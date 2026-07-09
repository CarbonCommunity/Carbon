using System.Collections.Generic;
using API.Hooks;
using Facepunch;
using Network;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("CanSendEffect", "CanSendEffect", typeof(EffectNetwork), "Send", [typeof(Effect), typeof(EntityNetworkRange)])]

		[MetadataAttribute.Category("Engine")]
		[MetadataAttribute.Info("Called before an effect is sent to a connection. Return false to suppress the effect for that connection.")]
		[MetadataAttribute.Parameter("effect", typeof(Effect))]
		[MetadataAttribute.Parameter("connection", typeof(Connection))]
		[MetadataAttribute.Return(typeof(bool))]

		public class CanSendEffect : Patch
		{
			public static bool Prefix(Effect effect, EntityNetworkRange networkRange)
			{
				if (Net.sv == null) return true;
				if (effect == null) return true;

				// Resolve pooledstringid early, matching vanilla EffectNetwork.Send
				if (effect.pooledstringid == 0 && !string.IsNullOrEmpty(effect.pooledString))
				{
					effect.pooledstringid = StringPool.Get(effect.pooledString);
				}
				if (effect.pooledstringid == 0) return true;

				// Determine which connections would receive this effect.
				// NOTE: This mirrors vanilla EffectNetwork.Send subscriber resolution and must be
				// updated if the vanilla method changes, otherwise hooks may fire for wrong connections.
				List<Connection> subscribers = null;

				if (effect.broadcast)
				{
					subscribers = BaseNetworkable.GlobalNetworkGroup?.subscribers;
				}
				else if (effect.targets != null)
				{
					subscribers = effect.targets;
				}
				else if (effect.entity.Value > 0)
				{
					var baseEntity = BaseNetworkable.serverEntities.Find(effect.entity) as BaseEntity;
					if (baseEntity.IsValid())
					{
						subscribers = baseEntity.net?.group?.subscribers;
					}
					else
					{
						return true;
					}
				}
				else
				{
					var group = Net.sv.visibility.GetGroup(effect.worldPos, networkRange);
					subscribers = group?.subscribers;
				}

				if (subscribers == null || subscribers.Count == 0) return true;

				// Skip per-connection hook calls if no handler is registered for this hook
				if (Community.Runtime != null && Community.Runtime.ModuleProcessor != null)
				{
					var hasHandler = false;
					foreach (var _ in HookCaller.GetAllFor(2666115907u))
					{
						hasHandler = true;
						break;
					}
					if (!hasHandler) return true;
				}

				// Call hook for each subscriber, filtering out suppressed connections
				var suppressed = false;
				var recipients = Pool.Get<List<Connection>>();
				var connections = Pool.Get<List<Connection>>();

				try
				{
					recipients.AddRange(subscribers);

					for (int i = 0; i < recipients.Count; i++)
					{
						var conn = recipients[i];

						if (conn != null && conn.connected && conn.isAuthenticated)
						{
							var result = HookCaller.CallStaticHook(2666115907u, effect, conn);

							if (result is bool boolResult && !boolResult)
							{
								suppressed = true;
								continue;
							}

							connections.Add(conn);
						}
					}

					if (!suppressed)
					{
						return true;
					}

					if (connections.Count == 0)
					{
						return false;
					}

					// Partial suppression - manually send to remaining connections
					var write = Net.sv.StartWrite();
					write.PacketID(Message.Type.Effect);
					effect.WriteToStream(write);
					write.Send(new SendInfo(connections));

					return false;
				}
				finally
				{
					Pool.FreeUnmanaged(ref recipients);
					Pool.FreeUnmanaged(ref connections);
				}
			}
		}
	}
}
