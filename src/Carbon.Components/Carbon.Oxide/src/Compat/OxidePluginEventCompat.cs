using Carbon.Events;
using Carbon.Compat.Legacy.EventCompat;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Plugins;

#pragma warning disable CS0618
#pragma warning disable CS0612

namespace Carbon.Compat.Lib;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public partial class OxideCompat
{
    static OxideCompat()
    {
        Carbon.Managers.Services.Events.Subscribe(CarbonEvent.PluginLoaded, args =>
        {
            HandlePluginIO(true, (CarbonEventArgs)args);
        });
        Carbon.Managers.Services.Events.Subscribe(CarbonEvent.PluginUnloaded, args =>
        {
            HandlePluginIO(false, (CarbonEventArgs)args);
        });
    }
    // Extensions subscribe to Plugin.OnAddedToManager/OnRemovedFromManager, the IL gets redirected here
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Plugin, PluginManagerEvent> _added = new();
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Plugin, PluginManagerEvent> _removed = new();

    internal static void HandlePluginIO(bool loaded, CarbonEventArgs args)
    {
        if (args.Payload is not Plugin plugin)
        {
            return;
        }

        var manager = (plugin as RustPlugin)?.Manager ?? Interface.Oxide.RootPluginManager;

        if ((loaded ? _added : _removed).TryGetValue(plugin, out var ev))
        {
            ev.Invoke(plugin, manager);
        }
    }

    public class PluginManagerEvent : OxideEvents.Event<Plugin, PluginManager>
    {

    }

    public static PluginManagerEvent OnAddedToManagerCompat(Plugin plugin) => _added.GetValue(plugin, _ => new PluginManagerEvent());

    public static PluginManagerEvent OnRemovedFromManagerCompat(Plugin plugin) => _removed.GetValue(plugin, _ => new PluginManagerEvent());
}
