using UnityEngine;

namespace Carbon.Core.Extensions
{
    public static class ConsoleArgEx
    {
        public static char [] CommandSpacing = new char [] { ' ' };

        public static bool IsPlayerCalledAndAdmin ( this ConsoleSystem.Arg arg )
        {
            return arg.Player () == null || arg.IsAdmin;
        }
    }
}