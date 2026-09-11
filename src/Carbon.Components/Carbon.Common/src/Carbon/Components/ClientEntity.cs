using Network;
using Connection = Network.Connection;
using Net = Network.Net;
using Time = UnityEngine.Time;

/*
 *
 * Copyright (c) 2023 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

namespace Carbon.Components;

/// <summary>
/// Client entity will allow you to manage 'braindead' entity objects which can be selectively networked to specific clients without reducing performance.
/// The client entities have no specified logic but the class can be inherited and the virtual methods overriden with your own custom logic.
/// </summary>
public class ClientEntity : IDisposable
{
	#region Statics

	internal static bool _isPatched { get; set; }

	public static Dictionary<ulong, ClientEntity> entities { get; private set; } = new();

	public static ClientEntity Create(
		string prefabName,
		Vector3 position,
		Quaternion rotation,
		ProtoBuf.Entity proto = null,
		ulong netId = 0,
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

	internal static void ServerRPCUnknown(NetworkableId netID, uint rpcID, Message packet)
	{
		if (entities.TryGetValue(netID.Value, out var entity) && entity.watchers.Contains(packet.connection))
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

	public ClientEntity(ProtoBuf.Entity proto = null, uint prefabId = 0, ulong netId = 0, uint group = 0) : base()
	{
		Proto = proto ?? new();
		Proto.baseNetworkable ??= new ProtoBuf.BaseNetworkable();
		Proto.baseEntity ??= new ProtoBuf.BaseEntity();

		NetID = new(netId == 0 ? Net.sv.TakeUID() : netId);

		Proto.baseNetworkable.group = group;

		if (prefabId != 0) Proto.baseNetworkable.prefabID = prefabId;

		entities[NetID.Value] = this;
		HookCheck();
	}

	public ProtoBuf.Entity Proto
	{
		get;
		set;
	}
	public NetworkableId NetID
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

			var temp = Facepunch.Pool.Get<List<Connection>>();
			temp.AddRange(watchers);

			KillAll();
			SpawnAll(temp);

			Facepunch.Pool.FreeUnmanaged(ref temp);
		}
	}
	public NetworkableId ParentID
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

	/// <summary>
	/// Spawns this client-side entity to a specific connected client.
	/// </summary>
	public virtual void SpawnFor(Connection connection)
	{
		_sendNetworkUpdateImmediate(connection);
	}

	/// <summary>
	/// Spawns this client-side entity to a multitude of connected clients.
	/// </summary>
	public virtual void SpawnAll(IList<Connection> connections)
	{
		_sendNetworkUpdateImmediate(connections);
	}

	/// <summary>
	/// Destroys this entity client-side for a specified connection.
	/// </summary>
	/// <param name="mode">Destruction mode. Gib or sudden.</param>
	public virtual void KillFor(Connection connection, BaseNetworkable.DestroyMode mode = BaseNetworkable.DestroyMode.None)
	{
		if (watchers.Contains(connection)) watchers.Remove(connection);

		using var writer = Net.sv.StartWrite();
		writer.PacketID(Message.Type.EntityDestroy);
		writer.EntityID(NetID);
		writer.UInt8((byte)mode);
		writer.Send(new SendInfo(connection));
	}

	/// <summary>
	/// Destroys this entity client-side for all current client entity watchers.
	/// </summary>
	/// <param name="mode">Destruction mode. Gib or sudden.</param>
	public virtual void KillAll(BaseNetworkable.DestroyMode mode = BaseNetworkable.DestroyMode.None)
	{
		using var writer = Net.sv.StartWrite();
		writer.PacketID(Message.Type.EntityDestroy);
		writer.EntityID(NetID);
		writer.UInt8((byte)mode);
		writer.Send(new SendInfo(watchers));

		watchers.Clear();
	}

	/// <summary>
	/// Checks if this client-side entity has a flag assigned.
	/// </summary>
	public bool HasFlag(BaseEntity.Flags flag)
	{
		return (Flags & flag) == flag;
	}

	/// <summary>
	/// Sets a flag (if not already set) and sends it as a network update to all client-side entity watchers.
	/// </summary>
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

	/// <summary>
	/// Send a network update to all current watchers of the client-side entity.
	/// </summary>
	public virtual void SendNetworkUpdate()
	{
		var connections = Facepunch.Pool.Get<List<Connection>>();
		foreach (var connection in watchers) connections.Add(connection);

		_sendNetworkUpdateImmediate(connections);
		Facepunch.Pool.FreeUnmanaged(ref connections);
	}

	/// <summary>
	/// Send a network update flags packet to all current watchers of the client-side entity.
	/// </summary>
	public virtual void SendNetworkUpdate_Flags()
	{
		if (watchers.Count == 0) return;

		using var writer = Net.sv.StartWrite();
		writer.PacketID(Message.Type.EntityFlags);
		writer.EntityID(NetID);
		writer.Int32((int)Flags);
		writer.Send(new SendInfo(watchers));
	}

	/// <summary>
	/// Send a network update position packet to all current watchers of the client-side entity.
	/// </summary>
	public virtual void SendNetworkUpdate_Position()
	{
		if (watchers.Count == 0) return;

		using var writer = Net.sv.StartWrite();
		writer.PacketID(Message.Type.EntityPosition);
		writer.EntityID(NetID);
		writer.Vector3(in Proto.baseEntity.pos);
		writer.Vector3(in Proto.baseEntity.rot);
		writer.Float(Time.time);

		if (ParentID.IsValid) writer.EntityID(ParentID);

		var sendInfo = new SendInfo(watchers)
		{
			method = SendMethod.ReliableUnordered,
			priority = Priority.Immediate
		};

		writer.Send(sendInfo);
	}

	/// <summary>
	/// Gets fired whenever a client is interacting with the entity client-side. Allowing you to create custom behavior for the entity.
	/// </summary>
	public virtual void OnRpc(string rpc, Message message)
	{

	}

	#region Internals

	internal List<Connection> watchers = new();
	internal NetworkableId _parentId;

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
		writer.PacketID(Message.Type.Entities);
		connection.validate.entityUpdates++;

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

	/// <summary>
	/// Completely disposes and terminates all logic of this client-side entity.
	/// </summary>
	public virtual void Dispose()
	{
		if (Proto == null) return;

		entities.Remove(NetID.Value);

		KillAll(BaseNetworkable.DestroyMode.None);
		Proto?.Dispose();
		Proto = null;
		watchers?.Clear();
		watchers = null;

		HookCheck();
	}
}
