using Harmony;
using Carbon.Core.Harmony;
using System.IO;

[HarmonyPatch(typeof(SaveRestore), "Load")]
public class SaveRestore_OnNewSave
{
    public static void Prefix(string strFilename = "", bool allowOutOfDateSaves = false)
    {
        if (strFilename == "")
        {
            strFilename = string.Concat(World.SaveFolderName, "/", World.SaveFileName);
        }
        if (!File.Exists(strFilename))
        {
            HookExecutor.CallStaticHook("OnNewSave", strFilename);
        }
    }
}