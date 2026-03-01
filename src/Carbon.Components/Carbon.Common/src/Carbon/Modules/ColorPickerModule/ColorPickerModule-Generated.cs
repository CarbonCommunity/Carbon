using Oxide.Game.Rust.Cui;
using static Carbon.Components.CUI;
using static Carbon.Modules.AdminModule;
using static ConsoleSystem;

namespace Carbon.Modules;
public partial class ColorPickerModule
{
    public override object InternalCallHook(uint hook, object[] args)
    {
        var length = args?.Length;
        var narg0 = length > 0 ? args[0] : null;
        var narg1 = length > 1 ? args[1] : null;
        try
        {
            switch (hook)
            {
                // CloseColorPickerUI aka 3488114070
                case 3488114070:
                {
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        CloseColorPickerUI(arg0_0);
                        return null;
                    }

                    break;
                }

                // Draw aka 3586261805
                case 3586261805:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg1_0 = narg1 is Action<string, string, float> or null;
                    var arg1_0 = narg1_0 ? (Action<string, string, float>)(narg1 ?? (Action<string, string, float>)default) : (Action<string, string, float>)default;
                    if (narg0_0 && narg1_0)
                    {
                        Draw(arg0_0, arg1_0);
                        return null;
                    }

                    break;
                }

                // DrawCursorLocker aka 3895172637
                case 3895172637:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0)
                    {
                        DrawCursorLocker(arg0_0);
                        return null;
                    }

                    break;
                }

                // PickAlphaColorPickerUI aka 877006401
                case 877006401:
                {
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PickAlphaColorPickerUI(arg0_0);
                        return null;
                    }

                    break;
                }

                // PickColorPickerUI aka 2210168706
                case 2210168706:
                {
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PickColorPickerUI(arg0_0);
                        return null;
                    }

                    break;
                }

                // PickHexColorPickerUI aka 1750458046
                case 1750458046:
                {
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PickHexColorPickerUI(arg0_0);
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