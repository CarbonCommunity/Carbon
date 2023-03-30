using System.Collections.Generic;
using System.Text;

namespace Oxide.Ext.Discord.Extensions
{
    /// <summary>
    /// String Extension methods
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// Parses the specified command into uMod command format
        /// Sourced from CommandHandler.cs of uMod (https://gitlab.com/umod/core/core/-/blob/develop/src/Command/CommandHandler.cs)
        /// </summary>
        /// <param name="argStr"></param>
        /// <param name="command"></param>
        /// <param name="args"></param>
        public static void ParseCommand(this string argStr, out string command, out string[] args)
        {
            List<string> argList = new List<string>();
            StringBuilder stringBuilder  = new StringBuilder();
            bool inLongArg = false;

            for (int index = 0; index < argStr.Length; index++)
            {
                char c = argStr[index];
                if (c == '"')
                {
                    if (inLongArg)
                    {
                        string arg = stringBuilder.ToString().Trim();
                        if (!string.IsNullOrEmpty(arg))
                        {
                            argList.Add(arg);
                        }

                        stringBuilder.Length = 0;
                        inLongArg = false;
                    }
                    else
                    {
                        inLongArg = true;
                    }
                }
                else if (char.IsWhiteSpace(c) && !inLongArg)
                {
                    string arg = stringBuilder.ToString().Trim();
                    if (!string.IsNullOrEmpty(arg))
                    {
                        argList.Add(arg);
                    }

                    stringBuilder.Length = 0;
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }

            if (stringBuilder.Length > 0)
            {
                string arg = stringBuilder.ToString().Trim();
                if (!string.IsNullOrEmpty(arg))
                {
                    argList.Add(arg);
                }
            }

            if (argList.Count == 0)
            {
                command = null;
                args = null;
                return;
            }

            command = argList[0].ToLower();
            argList.RemoveAt(0);
            args = argList.ToArray();
        }
    }
}