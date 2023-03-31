/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Extensions;

public class Extension
{
	public  string Name { get; set; }
	public  string Author { get; set; }
	public  VersionNumber Version { get; set; }
	public string Filename { get; set; }
	public string Branch { get; set; } = "public";

	public bool IsCoreExtension { get; }
	public bool IsGameExtension { get; }
	public bool SupportsReloading { get; }

	public virtual void Load() { }
	public virtual void Unload() { }
	public virtual void LoadPluginWatchers(string pluginDirectory) { }
	public virtual void OnModLoad() { }
	public virtual void OnShutdown() { }

	public Extension() { }
	public Extension(ExtensionManager manager) { }
}
