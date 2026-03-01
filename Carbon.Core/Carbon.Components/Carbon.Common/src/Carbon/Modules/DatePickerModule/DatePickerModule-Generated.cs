using static Carbon.Components.CUI;

namespace Carbon.Modules;
public partial class DatePickerModule
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
                // ActionDatePickerUI aka 388755728
                case 388755728:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ActionDatePickerUI(arg0_0);
                        return null;
                    }

                    break;
                }

                // CloseDatePickerUI aka 2637635139
                case 2637635139:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        CloseDatePickerUI(arg0_0);
                        return null;
                    }

                    break;
                }

                // Draw aka 3586261805
                case 3586261805:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg1_0 = narg1 is Action<DateTime> or null;
                    var arg1_0 = narg1_0 ? (Action<DateTime>)(narg1 ?? (Action<DateTime>)default) : (Action<DateTime>)default;
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