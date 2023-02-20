using System;
using System.Collections.Generic;
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

	private void OnServerInitialized()
	{
		Process();
	}

	private void Process()
	{
		if (!OsEx.File.Exists(Config.Key)) return;

		try
		{
			var key = Key.Deserialize(Config.Key);
			var temporaryEntities = BaseNetworkable.serverEntities.ToArray();
			var entitiesObliterated = 0;

			Puts($"Unlocking map with key '{Path.GetFileName(Config.Key)}'. Processing {key.points.Count:n0} points...");

			using (TimeMeasure.New("MapProtectionModule.Process"))
			{
				foreach (var entity in temporaryEntities)
				{
					for (int i = 0; i < key.points.Count; i++)
					{
						if (key.points[i] == entity.transform.position)
						{
							if (!entity.IsDestroyed)
							{
								entity.Kill();
								entitiesObliterated++;
							}

							break;
						}
					}
				}
			}

			Array.Clear(temporaryEntities, 0, temporaryEntities.Length);
			temporaryEntities = null;

			Puts($"Successfully unlocked map.");
		}
		catch (Exception exception) { PutsWarn($"Failed to successfully unlock map. Please report the following error:\n{exception}"); }
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
