using System;
using System.IO;

namespace Carbon.Core.Processors
{
    public class ScriptProcessor : BaseProcessor
    {
        public override string Folder => CarbonCore.GetPluginsFolder ();
        public override string Filter => "*.cs";
        public override Type IndexedType => typeof ( Script );

        public class Script : Instance
        {
            public override void Dispose ()
            {
                _loader?.Clear ();
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