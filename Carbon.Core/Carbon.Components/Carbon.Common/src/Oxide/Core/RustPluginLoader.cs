namespace Oxide.Game.Rust;

public class RustPluginLoader : PluginLoader
{
	public override Type[] CorePlugins
	{
		get
		{
			return new Type[] { typeof(CorePlugin) };
		}
	}
}
