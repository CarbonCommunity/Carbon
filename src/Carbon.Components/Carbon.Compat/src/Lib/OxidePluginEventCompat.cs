using API.Events;
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
        Community.Runtime.Events.Subscribe(CarbonEvent.PluginLoaded, args =>
        {
            HandlePluginIO(true, (CarbonEventArgs)args);
        });
        Community.Runtime.Events.Subscribe(CarbonEvent.PluginUnloaded, args =>
        {
            HandlePluginIO(false, (CarbonEventArgs)args);
        });
    }
    internal static void HandlePluginIO(bool loaded, CarbonEventArgs args)
    {
        RustPlugin plugin = (RustPlugin)args.Payload;
        PluginManagerEvent ev;

        ev = (loaded ? plugin.OnAddedToManager : plugin.OnRemovedFromManager) as PluginManagerEvent;
        ev?.Invoke(plugin, plugin.Manager);
    }

    public class PluginManagerEvent : OxideEvents.Event<Plugin, PluginManager>
    {

    }

    public static PluginManagerEvent OnAddedToManagerCompat(Plugin plugin)
    {
        if (plugin.OnAddedToManager is not PluginManagerEvent)
            plugin.OnAddedToManager = new PluginManagerEvent();

        return (PluginManagerEvent)plugin.OnAddedToManager;
    }

    public static PluginManagerEvent OnRemovedFromManagerCompat(Plugin plugin)
    {
        if (plugin.OnRemovedFromManager is not PluginManagerEvent)
            plugin.OnRemovedFromManager = new PluginManagerEvent();

        return (PluginManagerEvent)plugin.OnRemovedFromManager;
    }
}
