using Oxide.Game.Rust.Cui;
using UnityEngine.UI;
using static Carbon.Components.CUI;
using static ConsoleSystem;

namespace Carbon.Modules;
public partial class FileModule
{
    public override object InternalCallHook(uint hook, object[] args)
    {
        var length = args?.Length;
        var narg0 = length > 0 ? args[0] : null;
        try
        {
            switch (hook)
            {
                // FileAction aka 2520824062
                case 2520824062:
                {
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        FileAction(arg0_0);
                        return null;
                    }

                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Carbon.Logger.Error($"Failed to call internal hook '{Carbon.Pooling.HookStringPool.GetOrAdd(hook)}' on module '{this.Name} v{this.Version}' [{hook}]", ex);
            OnException(hook);
        }

        return (object)null;
    }
}