using Humanlights.Extensions;
using System.IO;
using System.IO.Compression;

namespace Carbon.Patch
{
    internal class Program
    {
        static void Main ( string [] args )
        {
            try { OsEx.Folder.DeleteContents ( "Release" ); } catch { }
            OsEx.Folder.Create ( $"Release" );

            OsEx.File.Copy ( "../../Carbon.Core/Carbon/bin/Release/net48/Carbon.dll", "Release/Carbon.dll" );
            OsEx.File.Copy ( "../../Carbon.Core/Carbon/bin/ReleaseUnix/net48/Carbon.dll", "Release/Carbon-Unix.dll" );

            //
            // Windows patch
            //
            using ( var memoryStream = new MemoryStream () )
            {
                using ( var archive = new ZipArchive ( memoryStream, ZipArchiveMode.Create, true ) )
                {
                    archive.CreateEntryFromFile ( "../../Carbon.Core/Carbon/bin/Release/net48/Carbon.dll", "HarmonyMods/Carbon.dll" );
                    archive.CreateEntryFromFile ( "../../Carbon.Core/Carbon.Doorstop/bin/Release/net48/Carbon.Doorstop.dll", "RustDedicated_Data/Managed/Carbon.Doorstop.dll" );
                    archive.CreateEntryFromFile ( "../Helpers/doorstop_config.ini", "doorstop_config.ini" );
                    archive.CreateEntryFromFile ( "../UnityDoorStop/winhttp.dll", "winhttp.dll" );
                    archive.CreateEntryFromFile ( "../NStrip/NStrip/bin/Release/net452/NStrip.exe", "carbon/tools/NStrip.exe" );
                }

                var output = $"Release/Carbon.Patch.zip";
                OsEx.Folder.Create ( $"Release" );
                OsEx.File.Delete ( output );
                OsEx.File.Create ( output, new byte [ 0 ] );
                using ( var fileStream = new FileStream ( output, FileMode.Open ) )
                {
                    memoryStream.Seek ( 0, SeekOrigin.Begin );
                    memoryStream.CopyTo ( fileStream );
                }
            }

            //
            // Linux patch
            //
            using ( var memoryStream = new MemoryStream () )
            {
                using ( var archive = new ZipArchive ( memoryStream, ZipArchiveMode.Create, true ) )
                {
                    archive.CreateEntryFromFile ( "../../Carbon.Core/Carbon/bin/ReleaseUnix/net48/Carbon.dll", "HarmonyMods/Carbon-Unix.dll" );
                    archive.CreateEntryFromFile ( "../../Tools/Scripts/linux_prepatch.sh", "carbon_prepatch.sh" );
                    archive.CreateEntryFromFile ( "../../Tools/NStrip/NStrip/bin/Release/net452/NStrip.exe", "carbon/tools/NStrip" );
                }

                var output = $"Release/Carbon.Patch-Unix.zip";
                OsEx.File.Delete ( output );
                OsEx.File.Create ( output, new byte [ 0 ] );
                using ( var fileStream = new FileStream ( output, FileMode.Open ) )
                {
                    memoryStream.Seek ( 0, SeekOrigin.Begin );
                    memoryStream.CopyTo ( fileStream );
                }
            }
        }
    }
}