///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Carbon.Extensions;
using UnityEngine;

namespace Carbon.Core;

[Serializable]
public class Defines
{
	public static Assembly Carbon { get; internal set; }

	public static void Initialize()
	{
		Carbon = typeof(Community).Assembly;

		GetRootFolder();
		GetConfigsFolder();
		GetModulesFolder();
		GetDataFolder();
		GetPluginsFolder();
		GetHarmonyFolder();
		GetLogsFolder();
		GetLangFolder();
		GetReportsFolder();
		OsEx.Folder.DeleteContents(GetTempFolder());
		Logger.Log("Loaded folders");
	}

	#region Paths

	public const string Name = "Carbon";
	public const string DllName = "Carbon.dll";

	public static string DllLoaderPath => Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HarmonyMods", "Carbon.Loader.dll"));
	public static string DllManagedPath => Path.GetFullPath(Path.Combine(GetManagedFolder(), "Carbon.dll"));
	public static string DllManagedPdbPath => Path.GetFullPath(Path.Combine(GetManagedFolder(), "Carbon.pdb"));

	public static string GetConfigFile()
	{
		return Path.Combine(GetRootFolder(), "config.json");
	}

	public static string GetRootFolder()
	{
		var folder = Path.GetFullPath(Path.Combine($"{Application.dataPath}/..", "carbon"));
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetConfigsFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "configs");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetModulesFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "modules");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetDataFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "data");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetPluginsFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "plugins");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetHarmonyFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "harmony");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetManagedFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "managed");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetLogsFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "logs");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetLangFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "lang");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetTempFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "temp");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetReportsFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "reports");
		Directory.CreateDirectory(folder);

		return folder;
	}

	#endregion

	internal static readonly IReadOnlyList<string> ReferenceList = new List<string>
	{
		"mscorlib",
		"netstandard",

		"System.Core",
		"System",
		"System.Runtime",

		"Carbon",

		"1Harmony",
		"Assembly-CSharp-firstpass",
		"Assembly-CSharp",
		"Facepunch.Console",
		"Facepunch.Network",
		"Facepunch.Rcon",
		"Facepunch.Sqlite",
		"Facepunch.System",
		"Facepunch.Unity",
		"Facepunch.UnityEngine",
		"Fleck",
		"Newtonsoft.Json",
		"Rust.Data",
		"Rust.Global",
		"Rust.Harmony",
		"Rust.Localization",
		"Rust.Platform.Common",
		"Rust.World",
		"UnityEngine.AIModule",
		"UnityEngine.CoreModule",
		"UnityEngine",
		"UnityEngine.ImageConversionModule",
		"UnityEngine.PhysicsModule",
		"UnityEngine.SharedInternalsModule",
		"UnityEngine.TerrainModule",
		"UnityEngine.TextRenderingModule",
		"UnityEngine.UI",
		"UnityEngine.UnityWebRequestAssetBundleModule",
		"UnityEngine.UnityWebRequestAudioModule",
		"UnityEngine.UnityWebRequestModule",
		"UnityEngine.UnityWebRequestTextureModule",
		"UnityEngine.UnityWebRequestWWWModule",
	};
}
