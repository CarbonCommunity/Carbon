using Facepunch;
using ProtoBuf;

namespace Carbon.Components;

public static class CustomVitalManager
{
	private static VitalDictionary<SharedIdentifiableVital> sharedVitals = new();
	private static ListDictionary<ulong, VitalDictionary<PlayerIdentifiableVital>> playerVitals = [];

	public static CustomVitalInfo RentVitalInfo(
		string icon = null, Color iconColor = default,
		Color backgroundColor = default,
		string leftText = null, Color leftTextColor = default,
		string rightText = null, Color rightTextColor = default,
		int timeLeft = 0, bool active = true)
	{
		var vitalInfo = Pool.Get<CustomVitalInfo>();
		vitalInfo.icon = icon;
		vitalInfo.iconColor = iconColor;
		vitalInfo.backgroundColor = backgroundColor;
		vitalInfo.leftText = leftText;
		vitalInfo.leftTextColor = leftTextColor;
		vitalInfo.rightText = rightText;
		vitalInfo.rightTextColor = rightTextColor;
		vitalInfo.active = active;
		vitalInfo.timeLeft = timeLeft;
		return vitalInfo;
	}

	/// <summary>
	/// Use RentVitalInfo to get a vital instance to add it to a player
	/// </summary>
	public static T AddVital<T>(BasePlayer player, CustomVitalInfo vital, float expiry = 0, bool sendUpdate = true) where T : PlayerIdentifiableVital
	{
		if (!playerVitals.TryGetValue(player.userID, out var vitals))
		{
			playerVitals.Add(player.userID, vitals = new VitalDictionary<PlayerIdentifiableVital>());
		}
		var identifiableVital = vitals.AddVital(vital, expiry);
		identifiableVital.playerId = player.userID;
		if (sendUpdate)
		{
			SendVitals(player);
		}
		return identifiableVital as T;
	}

	public static PlayerIdentifiableVital AddVital(BasePlayer player, CustomVitalInfo vital, float expiry = 0, bool sendUpdate = true) => AddVital<PlayerIdentifiableVital>(player, vital, expiry, sendUpdate);

	/// <summary>
	/// Use RentVitalInfo to get a vital instance to add it for all connected players (shared vital)
	/// </summary>
	public static T AddSharedVital<T>(CustomVitalInfo vital, float expiry = 0, bool sendUpdate = true) where T : SharedIdentifiableVital
	{
		var identifiableVital = sharedVitals.AddVital(vital, expiry);
		if (sendUpdate)
		{
			SendVitalsToEveryone();
		}
		return identifiableVital as T;
	}

	public static SharedIdentifiableVital AddSharedVital(CustomVitalInfo vital, float expiry = 0, bool sendUpdate = true) => AddSharedVital<SharedIdentifiableVital>(vital, expiry, sendUpdate);

	public static VitalDictionary<SharedIdentifiableVital> GetSharedVitals() => sharedVitals;

	public static VitalDictionary<PlayerIdentifiableVital> GetPlayerVitals(ulong playerId)
	{
		if (playerVitals.TryGetValue(playerId, out var vitals))
		{
			return vitals;
		}
		return null;
	}

	public static VitalDictionary<PlayerIdentifiableVital> GetPlayerVitals(BasePlayer player) => GetPlayerVitals(player.userID);

	public static int GetTotalPlayerVitalCount(ulong playerId) => GetSharedVitals().Count + (GetPlayerVitals(playerId)?.Count ?? 0);

	public static bool TryGetVital<T>(uint id, out T vital) where T : PlayerIdentifiableVital
	{
		var values = playerVitals.Values;
		for (int i = 0; i < playerVitals.Count; i++)
		{
			if (values[i].TryGetVital(id, out var playerVital))
			{
				vital = playerVital as T;
				return true;
			}
		}
		vital = null;
		return false;
	}

	public static bool TryGetVital<T>(ulong playerId, uint id, out T vital) where T : PlayerIdentifiableVital
	{
		if (!playerVitals.TryGetValue(playerId, out var dictionary))
		{
			vital = null;
			return false;
		}

		if (dictionary.TryGetVital(id, out var playerVital))
		{
			vital = playerVital as T;
			return true;
		}
		vital = null;
		return false;
	}

	public static bool TryGetVital(uint id, out PlayerIdentifiableVital vital) => TryGetVital<PlayerIdentifiableVital>(id, out vital);

	public static bool TryGetVital(ulong playerId, uint id, out PlayerIdentifiableVital vital) => TryGetVital<PlayerIdentifiableVital>(playerId, id, out vital);

	public static bool TryGetSharedVital<T>(uint id, out T vital) where T : SharedIdentifiableVital
	{
		if(sharedVitals.TryGetVital(id, out var sharedVital))
		{
			vital = sharedVital as T;
			return true;
		}
		vital = null;
		return false;
	}

	public static bool TryGetSharedVital(uint id, out SharedIdentifiableVital vital) => TryGetSharedVital<SharedIdentifiableVital>(id, out vital);

	public static bool RemoveVital(BasePlayer player, IdentifiableVital vital, bool sendUpdate = true) => RemoveVital(player, vital.id, sendUpdate);

	public static bool RemoveVital(BasePlayer player, uint id, bool sendUpdate = true)
	{
		if (!player.IsValid() || !playerVitals.TryGetValue(player.userID, out var vitals))
		{
			return false;
		}
		if (!vitals.RemoveVital(id))
		{
			return false;
		}
		if (sendUpdate)
		{
			SendVitals(player);
		}
		return true;
	}

	public static bool RemoveSharedVital(IdentifiableVital vital, bool sendUpdate = true) => RemoveSharedVital(vital.id, sendUpdate);

	public static bool RemoveSharedVital(uint id, bool sendUpdate = true)
	{
		if (!sharedVitals.RemoveVital(id))
		{
			return false;
		}
		if (sendUpdate)
		{
			SendVitalsToEveryone();
		}
		return true;
	}

	public static void ClearVitals(BasePlayer player, bool sendUpdate = true)
	{
		if (!player.IsValid() || !playerVitals.TryGetValue(player.userID, out var vitals))
		{
			return;
		}
		vitals.ClearVitals();
		if (sendUpdate)
		{
			SendVitals(player);
		}
	}

	public static void ClearSharedVitals(bool sendUpdate = true)
	{
		sharedVitals.ClearVitals();
		if (sendUpdate)
		{
			SendVitalsToEveryone();
		}
	}

	public static void SendVitals(BasePlayer player)
	{
		if (!player.IsValid())
		{
			return;
		}
		var vitals = Pool.Get<CustomVitals>();
		vitals.vitals = Pool.Get<List<CustomVitalInfo>>();
		if (playerVitals.TryGetValue(player.userID, out var pv))
		{
			pv.AppendVitals(vitals);
		}
		sharedVitals.AppendVitals(vitals);
		CommunityEntity.ServerInstance.SendCustomVitals(player, vitals);
		Pool.Free(ref vitals.vitals);
		Pool.Free(ref vitals);
	}

	public static void SendVitalsToEveryone()
	{
		for (int i = 0; i < BasePlayer.activePlayerList.Count; i++)
		{
			SendVitals(BasePlayer.activePlayerList[i]);
		}
	}

	public class VitalDictionary<T> where T : IdentifiableVital, new()
	{
		private ListDictionary<uint, T> buffer = [];

		public static uint nextVitalId = 100;

		public int Count => buffer.Count;

		public bool HasAny() => Count > 0;

		public void GetVitals(List<T> vitals)
		{
			for (int i = 0; i < buffer.Count; i++)
			{
				vitals.Add(buffer.Values[i]);
			}
		}

		public void AppendVitals(CustomVitals vitals)
		{
			var values = buffer.Values;
			for (int i = 0; i < Count; i++)
			{
				var identifiableVital = values[i];
				identifiableVital.info.timeLeft = Mathf.Max(identifiableVital.totalTimeLeft - (int)identifiableVital.sinceTimeLeftStarted, 0);
				vitals.vitals.Add(identifiableVital.info);
			}
		}

		public T AddVital(CustomVitalInfo vital, float expiry = 0)
		{
			T identifiableVital = Pool.Get<T>();
			identifiableVital.id = ++nextVitalId;
			identifiableVital.info = vital;
			identifiableVital.expiry = expiry;
			identifiableVital.SetTimeLeft(vital.timeLeft);
			identifiableVital.RestartExpiry();
			buffer.Add(identifiableVital.id, identifiableVital);
			return identifiableVital;
		}

		public bool RemoveVital(T vital)
		{
			if (!buffer.Remove(vital.id))
			{
				return false;
			}
			Pool.Free(ref vital);
			return true;
		}

		public bool RemoveVital(uint id)
		{
			if (!buffer.TryGetValue(id, out var vital))
			{
				return false;
			}
			Pool.Free(ref vital);
			return buffer.Remove(id);
		}

		public bool TryGetVital(uint id, out T vital)
		{
			return buffer.TryGetValue(id, out vital);
		}

		public void ClearVitals()
		{
			for (int i = 0; i < buffer.Count; i++)
			{
				var vitalInfo = buffer.Values[i].info;
				Pool.Free(ref vitalInfo);
			}
			buffer.Clear();
		}
	}

	public abstract class IdentifiableVital : Pool.IPooled
	{
		public uint id;
		public CustomVitalInfo info;
		public TimeSince sinceTimeLeftStarted;
		public int totalTimeLeft;
		public float expiry;

		protected bool _isPooled;
		protected Action _cachedExpiryAction;

		public bool IsPooled() => _isPooled;

		public virtual void SetTimeLeft(int timeLeft)
		{
			info.timeLeft = timeLeft;
			totalTimeLeft = info.timeLeft;
			sinceTimeLeftStarted = 0;
		}

		public abstract void SendUpdate();

		public abstract void RemoveSelf();

		public void RestartExpiry()
		{
			_cachedExpiryAction ??= RemoveSelf;

			if (ServerMgr.Instance.IsInvoking(_cachedExpiryAction))
			{
				ServerMgr.Instance.CancelInvoke(_cachedExpiryAction);
			}
			if (expiry > 0)
			{
				ServerMgr.Instance.Invoke(_cachedExpiryAction, expiry);
			}
		}

		public virtual void EnterPool()
		{
			id = 0;
			totalTimeLeft = 0;
			expiry = 0;
			if (info != null)
			{
				Pool.Free(ref info);
			}
			_isPooled = true;
		}

		public virtual void LeavePool()
		{
			sinceTimeLeftStarted = 0;
			_isPooled = false;
		}
	}

	public class SharedIdentifiableVital : IdentifiableVital
	{
		public override void RemoveSelf()
		{
			if (_isPooled)
			{
				return;
			}
			RemoveSharedVital(id);
		}

		public override void SendUpdate()
		{
			if (_isPooled)
			{
				return;
			}
			SendVitalsToEveryone();
		}
	}

	public class PlayerIdentifiableVital : IdentifiableVital
	{
		public ulong playerId;

		private BasePlayer player;

		public BasePlayer GetPlayer()
		{
			if (!player.IsValid())
			{
				player = BasePlayer.FindByID(playerId);
			}
			return player;
		}

		public override void RemoveSelf()
		{
			if (_isPooled || GetPlayer() is not BasePlayer player || !player.IsValid())
			{
				return;
			}
			RemoveVital(player, id);
		}

		public override void SendUpdate()
		{
			if (_isPooled)
			{
				return;
			}
			SendVitals(GetPlayer());
		}

		public override void EnterPool()
		{
			base.EnterPool();
			playerId = 0;
			player = null;
		}
	}
}
