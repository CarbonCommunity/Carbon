/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Extensions;

public class Extension
{
	public virtual string Name { get; set; }
	public virtual string Author { get; set; }
	public virtual VersionNumber Version { get; set; }
	public virtual string Filename { get; set; }
	public virtual string Branch { get; set; } = "public";

	public virtual bool IsCoreExtension { get; }
	public virtual bool IsGameExtension { get; }
	public virtual bool SupportsReloading { get; }

	public virtual void Load() { }
	public virtual void Unload() { }
	public virtual void LoadPluginWatchers(string pluginDirectory) { }
	public virtual void OnModLoad() { }
	public virtual void OnShutdown() { }

	public ExtensionManager Manager { get; } = new ();

	public Extension() { }
	public Extension(ExtensionManager manager) { Manager = manager ?? new (); }
}
