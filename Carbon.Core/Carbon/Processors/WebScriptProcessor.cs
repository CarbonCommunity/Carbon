using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Core.Processors
{
    public class WebScriptProcessor : BaseProcessor
    {
        public override string Extension => ".cs";
        public override Type IndexedType => typeof ( WebScript );

        public class WebScript : Instance
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
