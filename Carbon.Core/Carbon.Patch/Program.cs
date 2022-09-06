using Carbon.Core;
using Humanlights.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Patch
{
    internal class Program
    {
        static void Main ( string [] args )
        {
            Process.Start ( "update_rust.bat" ).WaitForExit ();

            using ( var memoryStream = new MemoryStream () )
            {
                using ( var archive = new ZipArchive ( memoryStream, ZipArchiveMode.Create, true ) )
                {
                    archive.CreateEntryFromFile ( "Carbon.Core/Carbon/bin/Release/Carbon.dll", "HarmonyMods/Carbon.dll" );
                    archive.CreateEntryFromFile ( "Carbon.Core/Carbon.Doorstop/bin/Release/Carbon.Doorstop.dll", "RustDedicated_Data/Managed/Carbon.Doorstop.dll" );
                    archive.CreateEntryFromFile ( "Tools/doorstop_config.ini", "doorstop_config.ini" );
                    archive.CreateEntryFromFile ( "Tools/winhttp.dll", "winhttp.dll" );
                    archive.CreateEntryFromFile ( "Tools/NStrip.exe", "carbon/tools/NStrip.exe" );
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
                OsEx.File.Copy ( "Carbon.Core/Carbon/bin/Release/Carbon.dll", "Release/Carbon.dll" );
                OsEx.File.Copy ( "Carbon.Core/Carbon.Extended/bin/Release/Carbon.Extended.dll", "Release/Carbon.Extended.dll" );
            }
        }
    }
}