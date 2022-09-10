using UnityEngine;

namespace Carbon.Core.Extensions
{
    public static class ConsoleArgEx
    {
        public static bool IsPlayerCalledAndAdmin ( this ConsoleSystem.Arg arg )
        {
            return arg.Player () == null || arg.IsAdmin;
        }
    }
}