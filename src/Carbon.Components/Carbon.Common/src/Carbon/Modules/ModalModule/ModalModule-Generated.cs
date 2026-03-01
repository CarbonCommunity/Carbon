using Oxide.Game.Rust.Cui;
using static Carbon.Components.CUI;
using static ConsoleSystem;

namespace Carbon.Modules;
public partial class ModalModule
{
    public override object InternalCallHook(uint hook, object[] args)
    {
        var length = args?.Length;
        var narg0 = length > 0 ? args[0] : null;
        try
        {
            switch (hook)
            {
                // ModalAction aka 104934377
                case 104934377:
                {
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        ModalAction(arg0_0);
                        return null;
                    }

                    break;
                }

                // ModalCancel aka 1752466352
                case 1752466352:
                {
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        ModalCancel(arg0_0);
                        return null;
                    }

                    break;
                }

                // ModalConfirm aka 3879514865
                case 3879514865:
                {
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        ModalConfirm(arg0_0);
                        return null;
                    }

                    break;
                }

                // ModalPage aka 4001517656
                case 4001517656:
                {
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        ModalPage(arg0_0);
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