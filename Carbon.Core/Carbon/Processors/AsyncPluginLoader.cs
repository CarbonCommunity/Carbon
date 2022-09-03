using CSharpCompiler;
using Humanlights.Unity.Compiler;
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CodeCompiler = CSharpCompiler.CodeCompiler;

namespace Carbon.Core
{
    public class AsyncPluginLoader : ThreadedJob
    {
        public string Source;
        public Assembly Assembly;
        public Exception Exception;

        internal CodeCompiler _compiler;
        internal CompilerParameters _parameters;
        internal int _retries;

        internal static string [] _defaultReferences = new string [] { "System.dll", "mscorlib.dll" };

        internal void _addReferences ()
        {
            _parameters.ReferencedAssemblies.Clear ();
            _parameters.ReferencedAssemblies.AddRange ( _defaultReferences );

            var assemblies = AppDomain.CurrentDomain.GetAssemblies ();
            var lastCarbon = ( Assembly )null;
            foreach ( var assembly in assemblies )
            {
                if ( CarbonLoader.AssemblyCache.Any ( x => x == assembly ) ) continue;

                if ( !assembly.FullName.StartsWith ( "Carbon" ) )
                {
                    if ( assembly.ManifestModule is ModuleBuilder builder )
                    {
                        if ( !builder.IsTransient () )
                        {
                            _parameters.ReferencedAssemblies.Add ( assembly.GetName ().Name );
                        }
                    }
                    else
                    {
                        _parameters.ReferencedAssemblies.Add ( assembly.GetName ().Name );
                    }
                }
                else if ( assembly.FullName.StartsWith ( "Carbon" ) )
                {
                    lastCarbon = assembly;
                }
            }

            if ( lastCarbon != null )
            {
                _parameters.ReferencedAssemblies.Add ( lastCarbon.GetName ().Name );
            }
        }

        public override void Start ()
        {
            base.Start ();

            _compiler = new CodeCompiler ();
            _parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };

            _addReferences ();
        }

        public override void ThreadFunction ()
        {
            try
            {
                var result = _compiler.CompileAssemblyFromSource ( _parameters, Source );
                Assembly = result.CompiledAssembly;

                if(result.Errors.Count > 0 )
                {
                    var error = result.Errors [ 0 ];
                    throw new Exception ( $"{error.ErrorText} ({error.FileName} {error.Column} line {error.Line})" );
                }
            }
            catch ( Exception exception )
            {
                if ( _retries <= 2 )
                {
                    _retries++;
                    ThreadFunction ();
                }
                else
                {
                    Exception = exception;
                }
            }
        }

    }
}