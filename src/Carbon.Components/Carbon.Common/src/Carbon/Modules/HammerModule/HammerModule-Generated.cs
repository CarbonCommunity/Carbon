using Facepunch;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;
using Timer = Oxide.Plugins.Timer;

namespace Carbon.Modules;
public partial class HammerModule
{
    public override object InternalCallHook(uint hook, object[] args)
    {
        var length = args?.Length;
        var narg0 = length > 0 ? args[0] : null;
        var narg1 = length > 1 ? args[1] : null;
        var narg2 = length > 2 ? args[2] : null;
        var narg3 = length > 3 ? args[3] : null;
        try
        {
            switch (hook)
            {
                // DestroyEntitiesOverTime aka 3176354530
                case 3176354530:
                {
                    var narg0_0 = narg0 is HammerEditor or null;
                    var arg0_0 = narg0_0 ? (HammerEditor)(narg0 ?? (HammerEditor)default) : (HammerEditor)default;
                    var narg1_0 = narg1 is List<BaseEntity> or null;
                    var arg1_0 = narg1_0 ? (List<BaseEntity>)(narg1 ?? (List<BaseEntity>)default) : (List<BaseEntity>)default;
                    if (narg0_0 && narg1_0)
                    {
                        return DestroyEntitiesOverTime(arg0_0, arg1_0);
                    }

                    break;
                }

                // EditOption aka 3750013587
                case 3750013587:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        EditOption(arg0_0);
                        return null;
                    }

                    break;
                }

                // MoveEntityRoutine aka 1994070204
                case 1994070204:
                {
                    var narg0_0 = narg0 is HammerEditor or null;
                    var arg0_0 = narg0_0 ? (HammerEditor)(narg0 ?? (HammerEditor)default) : (HammerEditor)default;
                    var narg1_0 = narg1 is BaseEntity or null;
                    var arg1_0 = narg1_0 ? (BaseEntity)(narg1 ?? (BaseEntity)default) : (BaseEntity)default;
                    if (narg0_0 && narg1_0)
                    {
                        return MoveEntityRoutine(arg0_0, arg1_0);
                    }

                    break;
                }

                // OnActiveItemChanged aka 2268037981
                case 2268037981:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg1_0 = narg1 is Item or null;
                    var arg1_0 = narg1_0 ? (Item)(narg1 ?? (Item)default) : (Item)default;
                    if (narg0_0 && narg1_0)
                    {
                        OnActiveItemChanged(arg0_0, arg1_0);
                        return null;
                    }

                    break;
                }

                // OnCuiDraggableDrag aka 1614693435
                case 1614693435:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg1_0 = narg1 is string or null;
                    var arg1_0 = narg1_0 ? (string)(narg1 ?? (string)default) : (string)default;
                    var narg2_0 = narg2 is Vector3 or null;
                    var arg2_0 = narg2_0 ? (Vector3)(narg2 ?? (Vector3)default) : (Vector3)default;
                    var narg3_0 = narg3 is CommunityEntity.DraggablePositionSendType or null;
                    var arg3_0 = narg3_0 ? (CommunityEntity.DraggablePositionSendType)(narg3 ?? (CommunityEntity.DraggablePositionSendType)default) : (CommunityEntity.DraggablePositionSendType)default;
                    if (narg0_0 && narg1_0 && narg2_0 && narg3_0)
                    {
                        OnCuiDraggableDrag(arg0_0, arg1_0, arg2_0, arg3_0);
                        return null;
                    }

                    break;
                }

                // OnHammerHit aka 4229965862
                case 4229965862:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg1_0 = narg1 is HitInfo or null;
                    var arg1_0 = narg1_0 ? (HitInfo)(narg1 ?? (HitInfo)default) : (HitInfo)default;
                    if (narg0_0 && narg1_0)
                    {
                        return OnHammerHit(arg0_0, arg1_0);
                    }

                    break;
                }

                // OnPlayerDeath aka 3560982762
                case 3560982762:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0)
                    {
                        OnPlayerDeath(arg0_0);
                        return null;
                    }

                    break;
                }

                // OnPlayerDisconnected aka 72085565
                case 72085565:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0)
                    {
                        OnPlayerDisconnected(arg0_0);
                        return null;
                    }

                    break;
                }

                // OnPlayerInput aka 3411611961
                case 3411611961:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg1_0 = narg1 is InputState or null;
                    var arg1_0 = narg1_0 ? (InputState)(narg1 ?? (InputState)default) : (InputState)default;
                    if (narg0_0 && narg1_0)
                    {
                        OnPlayerInput(arg0_0, arg1_0);
                        return null;
                    }

                    break;
                }

                // OnPlayerSleep aka 4058415132
                case 4058415132:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0)
                    {
                        OnPlayerSleep(arg0_0);
                        return null;
                    }

                    break;
                }

                // ReconstructEntity aka 3047062756
                case 3047062756:
                {
                    var narg0_0 = narg0 is BaseEntity or null;
                    var arg0_0 = narg0_0 ? (BaseEntity)(narg0 ?? (BaseEntity)default) : (BaseEntity)default;
                    if (narg0_0)
                    {
                        ReconstructEntity(arg0_0);
                        return null;
                    }

                    break;
                }

                // RepairEntitiesOverTime aka 1897255792
                case 1897255792:
                {
                    var narg0_0 = narg0 is HammerEditor or null;
                    var arg0_0 = narg0_0 ? (HammerEditor)(narg0 ?? (HammerEditor)default) : (HammerEditor)default;
                    var narg1_0 = narg1 is List<BaseCombatEntity> or null;
                    var arg1_0 = narg1_0 ? (List<BaseCombatEntity>)(narg1 ?? (List<BaseCombatEntity>)default) : (List<BaseCombatEntity>)default;
                    if (narg0_0 && narg1_0)
                    {
                        return RepairEntitiesOverTime(arg0_0, arg1_0);
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