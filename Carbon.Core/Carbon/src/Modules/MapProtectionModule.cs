using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Carbon.Base;
using Carbon.Core;
using Carbon.Extensions;
using ProtoBuf;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class MapProtectionModule : CarbonModule<MapProtectionConfig, MapProtectionData>
{
	public override string Name => "Map Protection";
	public override Type Type => typeof(MapProtectionModule);
	public override bool EnabledByDefault => true;

	private void IOnWorldSerializationLoaded(string fileName, WorldSerialization serialization)
	{
		if (!OsEx.File.Exists(Config.Key)) return;

		try
		{
			var key = Key.Deserialize(Config.Key);

			Puts($"Unlocking map with key '{Path.GetFileName(Config.Key)}'. Processing {key.points.Count:n0} points...");

			serialization.world.size = World.Size = key.size;
			ConVar.Server.worldsize = (int)key.size;

			var entitiesRemoved = 0;
			var prefabs = serialization.world.prefabs.ToArray();
			var logGuesstimate = key.points.Count.Scale(0, 50000, 0, 2500);

			for (int i = 0; i < key.points.Count; i++)
			{
				var point = key.points[i];

				foreach (var prefab in prefabs)
				{
					if (prefab.position.x == point.x && prefab.position.y == point.y && prefab.position.z == point.z)
					{
						serialization.world.prefabs.Remove(prefab);
						entitiesRemoved++;

						if (i % logGuesstimate == 0)
						{
							UnityEngine.Debug.Log($" Progress... {i.Scale(0, key.points.Count, 0, 100):n0}%");
						}
						break;
					}
				}
			}

			Puts($"Successfully unlocked map! Removed {entitiesRemoved:n0} / {key.points.Count:n0} obfuscated entities from the loaded map.");
		}
		catch (Exception exception) { PutsWarn($"Failed to successfully unlock map. Please report the following error:\n{exception}"); }
	}

	private void DestroyEntities()
	{
		if (!OsEx.File.Exists(Config.Key)) return;

		try
		{
			var key = Key.Deserialize(Config.Key);

			World.Serialization.world.size = World.Size = key.size;
			ConVar.Server.worldsize = (int)key.size;

			Community.Runtime.CorePlugin.persistence.StartCoroutine(_destroyEntities(key));
		}
		catch (Exception exception) { PutsWarn($"Failed to successfully unlock map. Please report the following error:\n{exception}"); }
	}

	internal IEnumerator _destroyEntities(Key key)
	{
		Puts($"Unlocking map with key '{Path.GetFileName(Config.Key)}'. Processing {key.points.Count:n0} points...");

		var temporaryEntities = BaseNetworkable.serverEntities.ToArray();
		var entitiesDestroyed = 0;
		var timeTook = new Stopwatch();
		timeTook.Start();

		var serverExhaleEvery = temporaryEntities.Length.Scale(0, 50000, 0, 2500);

		for (int i = 0; i < temporaryEntities.Length; i++)
		{
			var entity = temporaryEntities[i];
			if (entity == null || entity.IsDestroyed) continue;

			if (i % serverExhaleEvery == 0)
			{
				yield return CoroutineEx.waitForEndOfFrame;

				var percent = i.Scale(0, temporaryEntities.Length, 0, 100);
				UnityEngine.Debug.Log($" Progress... {percent:n0}%");

				yield return CoroutineEx.waitForEndOfFrame;
			}

			try
			{
				for (int w = 0; w < key.points.Count; w++)
				{
					if (key.points[w] == entity.transform.position)
					{
						entity.Kill();
						entitiesDestroyed++;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				PutsError($"Failed at {entity}:", ex);
			}
		}

		Array.Clear(temporaryEntities, 0, temporaryEntities.Length);
		temporaryEntities = null;

		Puts($"Successfully unlocked map! Destroying {entitiesDestroyed:n0} entities took {timeTook.Elapsed.TotalSeconds:0.0}s.");
	}

	[ProtoBuf.ProtoContract]
	public class Key : IDisposable
	{
		[ProtoMember(1)]
		public List<Vector> points;

		[ProtoMember(2)]
		public uint size;

		public void Serialize(string file)
		{
			using (var stream = new MemoryStream())
			{
				Serializer.Serialize(stream, this);
				System.IO.File.WriteAllBytes(file, stream.ToArray());
			}
		}

		public static Key Deserialize(string file)
		{
			using (var stream = System.IO.File.Open(file, FileMode.Open))
			{
				return Serializer.Deserialize<Key>(stream);
			}
		}

		public void Dispose()
		{
			points?.Clear();
			points = null;
		}

		[ProtoBuf.ProtoContract]
		public class Vector
		{
			[ProtoMember(1)]
			public float x;

			[ProtoMember(2)]
			public float y;

			[ProtoMember(3)]
			public float z;

			public Vector() { }
			public Vector(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public static implicit operator Vector(Vector3 v)
			{
				return new Vector(v.x, v.y, v.z);
			}
			public static implicit operator Vector(Quaternion q)
			{
				return q.eulerAngles;
			}
			public static implicit operator Vector3(Vector v)
			{
				return new Vector3(v.x, v.y, v.z);
			}
			public static implicit operator Quaternion(Vector v)
			{
				return Quaternion.Euler(v);
			}
		}
	}
}

public class MapProtectionConfig
{
	public string Key { get; set; }
}
public class MapProtectionData
{

}
