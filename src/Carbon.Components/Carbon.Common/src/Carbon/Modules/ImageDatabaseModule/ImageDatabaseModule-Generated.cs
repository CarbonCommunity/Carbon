using System.Drawing.Imaging;
using System.Management.Instrumentation;
using System.Net;
using Facepunch;
using ProtoBuf;
using QRCoder;
using static Carbon.Modules.ImageDatabaseModule;
using Color = System.Drawing.Color;
using Defines = Carbon.Core.Defines;
using Timer = Oxide.Plugins.Timer;

namespace Carbon.Modules;
public partial class ImageDatabaseModule
{
    public override object InternalCallHook(uint hook, object[] args)
    {
        var length = args?.Length;
        var narg0 = length > 0 ? args[0] : null;
        try
        {
            switch (hook)
            {
                // _getProtoDataPath aka 948911233
                case 948911233:
                {
                    return _getProtoDataPath();
                    break;
                }

                // ClearInvalid aka 3338439234
                case 3338439234:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ClearInvalid(arg0_0);
                        return null;
                    }

                    break;
                }

                // DeleteImg aka 2641566827
                case 2641566827:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        DeleteImg(arg0_0);
                        return null;
                    }

                    break;
                }

                // LoadDefaultImages aka 986171947
                case 986171947:
                {
                    LoadDefaultImages(narg0 is bool arg0_0 ? arg0_0 : (bool)default);
                    return null;
                    break;
                }

                // LoadDefaults aka 2550838889
                case 2550838889:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        LoadDefaults(arg0_0);
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