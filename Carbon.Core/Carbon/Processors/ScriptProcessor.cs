using System;
using System.IO;

namespace Carbon.Core.Processors
{
    public class ScriptProcessor : BaseProcessor
    {
        public override string Folder => CarbonCore.GetPluginsFolder ();
        public override string Extension => ".cs";
        public override Type IndexedType => typeof ( Script );

        public class Script : Instance
        {
            internal ScriptLoader _loader;

            public override void Dispose ()
            {
                try
                {
                    _loader?.Clear ();
                }
                catch ( Exception ex )
                {
                    CarbonCore.Error ( $"Error disposing {File}", ex );
                }

                _loader = null;
            }
            public override void Execute ()
            {
                try
                {
                    _loader = new ScriptLoader ();
                    _loader.Files.Add ( File );
                    _loader.Load ( true );
                }
                catch ( Exception ex )
                {
                    CarbonCore.Warn ( $"Failed processing {Path.GetFileNameWithoutExtension ( File )}:\n{ex}" );
                }
            }
        }
    }
}