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
                    foreach ( var directory in Directory.GetDirectories ( "Rust/RustDedicated_Data" ) )
                    {
                        var files = OsEx.Folder.GetFilesWithExtension ( directory, "*" );
                        foreach ( var file in files )
                        {
                            archive.CreateEntryFromFile ( file, file.Replace ( "Rust/", "" ) );
                        }
                    }

                    archive.CreateEntryFromFile ( "Carbon.Core/Carbon/bin/Release/Carbon.dll", "HarmonyMods/Carbon.dll" );
                }

                var output = $"Carbon.Core{CarbonCore.Version}.zip";
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