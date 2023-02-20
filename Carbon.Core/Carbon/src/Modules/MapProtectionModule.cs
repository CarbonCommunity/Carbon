using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Carbon.Base;
using Carbon.Extensions;
using Newtonsoft.Json;
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

	[ConsoleCommand("docleanup")]
	private void DoCleanup(ConsoleSystem.Arg arg)
	{
		Cleanup();
	}

	private void Cleanup()
	{
		if (!OsEx.File.Exists(Config.Key)) return;

		var key = Key.Deserialize(Config.Key);
		Puts($"Deserialized key: {Config.Key} with {key.points.Count:n0} points");

		var entKilled = 0;
		foreach (var entity in BaseNetworkable.serverEntities)
		{
			if (key.points.Any(x => x == entity.transform.position))
			{
				try { entity.Kill(); entKilled++; }
				catch
				{
					try { entity.AdminKill(); entKilled++; } catch { }
				}		
			}
		}
	}

	[ProtoContract]
	public class Key : IDisposable
	{
		[ProtoMember(1)]
		public List<VectorData> points;

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
	}
}

public class MapProtectionConfig
{
	public string Key { get; set; }
}
public class MapProtectionData
{

}
