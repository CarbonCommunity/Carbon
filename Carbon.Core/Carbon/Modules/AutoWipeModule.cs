///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

namespace Carbon.Core.Modules
{
    public class AutoWipeModule : BaseModule<AutoWipeConfig>
    {
        public override string Name => "AutoWipe";
    }

    public class AutoWipeConfig
    {
        public int NextWipeSeed = -1;
    }
}
