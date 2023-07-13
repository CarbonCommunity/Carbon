using Network;
using Connection = Network.Connection;
using Net = Network.Net;
using Time = UnityEngine.Time;

/*
 *
 * Copyright (c) 2023 Carbon Community 
 * Copyright (c) 2023 Patrette
 * All rights reserved.
 *
 */

namespace Carbon.Components;

public class ClientEntity : IDisposable
{
	#region Statics

	internal static bool _isPatched { get; set; }

	public static Dictionary<uint, ClientEntity> entities { get; private set; } = new();

	public static ClientEntity Create(
		string prefabName,
		Vector3 position,
		Quaternion rotation,
		ProtoBuf.Entity proto = null,
		uint netId = 0,
		uint group = 0)
	{
		var prefabId = (uint)default;

		if (StringPool.toNumber.TryGetValue(prefabName, out var value)) prefabId = value;

		if (prefabId == 0)
		{
			var lookup = ConVar.Entity.GetSpawnEntityFromName(prefabName);

			if (string.IsNullOrEmpty(lookup.PrefabName))
			{
				Logger.Warn($"ClientEntity creation failed: '{prefabName}' does not exist.");
				return null;
			}

			prefabId = StringPool.Get(lookup.PrefabName);
		}

		var client = new ClientEntity(proto, prefabId, netId, group)
		{
			Position = position,
			Rotation = rotation.eulerAngles
		};

		return client;
	}

	internal static void ServerRPCUnknown(uint netID, uint rpcID, Message packet)
	{
		if (entities.TryGetValue(netID, out var entity) && entity.watchers.Contains(packet.connection))
		{
			entity.OnRpc(StringPool.Get(rpcID), packet);
		}
	}

	internal static void HookCheck()
	{
		if (!_isPatched && entities.Count > 0)
		{
			_isPatched = true;
			Community.Runtime.HookManager.Subscribe("IServerMgrOnRPCMessage", "ClientEntity.HookCheck");
		}
	}

	#endregion

	public ClientEntity(ProtoBuf.Entity proto = null, uint prefabId = 0, uint netId = 0, uint group = 0) : base()
	{
		Proto = proto ?? new();
		Proto.baseNetworkable ??= new ProtoBuf.BaseNetworkable();
		Proto.baseEntity ??= new ProtoBuf.BaseEntity();

		NetID = netId == 0 ? Net.sv.TakeUID() : netId;

		Proto.baseNetworkable.group = group;

		if (prefabId != 0) Proto.baseNetworkable.prefabID = prefabId;

		entities[NetID] = this;
		HookCheck();
	}

	public ProtoBuf.Entity Proto
	{
		get;
		set;
	}
	public uint NetID
	{
		get => Proto.baseNetworkable.uid;
		private set => Proto.baseNetworkable.uid = value;
	}
	public uint Prefab
	{
		get => Proto.baseNetworkable.prefabID;
		set
		{
			Proto.baseNetworkable.prefabID = value;

			var temporaryWatchers = Facepunch.Pool.GetList<Connection>();
			temporaryWatchers.AddRange(watchers);

			KillAll();
			SpawnAll(temporaryWatchers);

			Facepunch.Pool.FreeList(ref temporaryWatchers);
		}
	}
	public uint ParentID
	{
		get => _parentId;
		set { _parentId = value; SendNetworkUpdate(); }
	}
	public BaseEntity.Flags Flags
	{
		get => (BaseEntity.Flags)Proto.baseEntity.flags;
		set { Proto.baseEntity.flags = (int)value; SendNetworkUpdate_Flags(); }
	}
	public Vector3 Position
	{
		get => Proto.baseEntity.pos;
		set => Proto.baseEntity.pos = value;
	}
	public Vector3 Rotation
	{
		get => Proto.baseEntity.rot;
		set => Proto.baseEntity.rot = value;
	}

	public virtual void SpawnFor(Connection connection)
	{
		_sendNetworkUpdateImmediate(connection);
	}
	public virtual void SpawnAll(IList<Connection> connections)
	{
		_sendNetworkUpdateImmediate(connections);
	}
	public virtual void KillFor(Connection connection, BaseNetworkable.DestroyMode mode = BaseNetworkable.DestroyMode.None)
	{
		if (watchers.Contains(connection)) watchers.Remove(connection);

		using var writer = Net.sv.StartWrite();
		writer.PacketID(Message.Type.EntityDestroy);
		writer.EntityID(NetID);
		writer.UInt8((byte)mode);
		writer.Send(new SendInfo(connection));
	}
	public virtual void KillAll(BaseNetworkable.DestroyMode mode = BaseNetworkable.DestroyMode.None)
	{
		using var writer = Net.sv.StartWrite();
		writer.PacketID(Message.Type.EntityDestroy);
		writer.EntityID(NetID);
		writer.UInt8((byte)mode);
		writer.Send(new SendInfo(watchers));

		watchers.Clear();
	}

	public bool HasFlag(BaseEntity.Flags flag)
	{
		return (Flags & flag) == flag;
	}
	public void SetFlag(BaseEntity.Flags flag, bool wants, bool update = true)
	{
		if (wants)
		{
			if (HasFlag(flag)) return;

			Flags |= flag;
		}
		else
		{
			if (!HasFlag(flag)) return;

			Flags &= ~flag;
		}

		if (update)
		{
			SendNetworkUpdate_Flags();
		}
	}
	public virtual void SendNetworkUpdate()
	{
		var connections = Facepunch.Pool.GetList<Network.Connection>();
		foreach (var connection in watchers) connections.Add(connection);

		_sendNetworkUpdateImmediate(connections);
		Facepunch.Pool.FreeList(ref connections);
	}
	public virtual void SendNetworkUpdate_Flags()
	{
		if (watchers.Count == 0) return;

		using var writer = Net.sv.StartWrite();
		writer.PacketID(Message.Type.EntityFlags);
		writer.EntityID(NetID);
		writer.Int32((int)Flags);
		writer.Send(new SendInfo(watchers));
	}
	public virtual void SendNetworkUpdate_Position()
	{
		if (watchers.Count == 0) return;

		using var writer = Net.sv.StartWrite();
		writer.PacketID(Message.Type.EntityPosition);
		writer.EntityID(NetID);
		writer.Vector3(in Proto.baseEntity.pos);
		writer.Vector3(in Proto.baseEntity.rot);
		writer.Float(Time.time);

		if (ParentID != 0) writer.EntityID(ParentID);

		var sendInfo = new SendInfo(watchers)
		{
			method = SendMethod.ReliableUnordered,
			priority = Priority.Immediate
		};

		writer.Send(sendInfo);
	}

	public virtual void OnRpc(string rpc, Message message)
	{

	}

	#region Internals

	internal List<Connection> watchers = new();
	internal uint _parentId;

	internal void _sendNetworkUpdateImmediate(Connection connection)
	{
		var data = Proto.ToProtoBytes();

		if (connection.player is BasePlayer)
		{
			_sendSnapshot(connection, data);
		}
	}
	internal void _sendNetworkUpdateImmediate(IList<Connection> connections)
	{
		var data = Proto.ToProtoBytes();

		foreach (var connection in connections)
		{
			if (connection.player is BasePlayer)
			{
				_sendSnapshot(connection, data);
			}
		}
	}
	internal void _sendSnapshot(Connection connection, byte[] data)
	{
		using var writer = Network.Net.sv.StartWrite();
		connection.validate.entityUpdates++;

		writer.PacketID(Message.Type.Entities);
		writer.UInt32(connection.validate.entityUpdates);
		writer.Write(data, 0, data.Length);
		writer.Send(new SendInfo(connection));

		if (!watchers.Contains(connection)) watchers.Add(connection);
	}

	#endregion

	#region RPC

	internal NetWrite RPCWriteStart(Connection sourceConnection, string funcName)
	{
		var writer = Net.sv.StartWrite();
		writer.PacketID(Message.Type.RPCMessage);
		writer.EntityID(NetID);
		writer.UInt32(StringPool.Get(funcName));
		writer.UInt64(sourceConnection?.userid ?? 0);

		return writer;
	}

	public void ClientRPC(Connection sourceConnection, string funcName)
	{
		using var writer = RPCWriteStart(sourceConnection, funcName);
		writer.Send(new SendInfo(watchers));
	}
	public void ClientRPC<T1>(Connection sourceConnection, string funcName, T1 arg1)
	{
		using var writer = RPCWriteStart(sourceConnection, funcName);
		writer.WriteObject(arg1);
		writer.Send(new SendInfo(watchers));
	}
	public void ClientRPC<T1, T2>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		using var writer = RPCWriteStart(sourceConnection, funcName);
		writer.WriteObject(arg1);
		writer.WriteObject(arg2);
		writer.Send(new SendInfo(watchers));
	}
	public void ClientRPC<T1, T2, T3>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		using var writer = RPCWriteStart(sourceConnection, funcName);
		writer.WriteObject(arg1);
		writer.WriteObject(arg2);
		writer.WriteObject(arg3);
		writer.Send(new SendInfo(watchers));
	}
	public void ClientRPC<T1, T2, T3, T4>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		using var writer = RPCWriteStart(sourceConnection, funcName);
		writer.WriteObject(arg1);
		writer.WriteObject(arg2);
		writer.WriteObject(arg3);
		writer.WriteObject(arg4);
		writer.Send(new SendInfo(watchers));
	}
	public void ClientRPC<T1, T2, T3, T4, T5>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		using var writer = RPCWriteStart(sourceConnection, funcName);
		writer.WriteObject(arg1);
		writer.WriteObject(arg2);
		writer.WriteObject(arg3);
		writer.WriteObject(arg4);
		writer.WriteObject(arg5);
		writer.Send(new SendInfo(watchers));
	}

	#endregion

	public virtual void Dispose()
	{
		if (Proto == null) return;

		entities.Remove(NetID);

		KillAll(BaseNetworkable.DestroyMode.None);
		Proto?.Dispose();
		Proto = null;
		watchers?.Clear();
		watchers = null;

		HookCheck();
	}
}
