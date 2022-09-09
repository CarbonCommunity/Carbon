using Facepunch;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using CodeCompiler = CSharpCompiler.CodeCompiler;

namespace Carbon.Core
{
    public class AsyncPluginLoader : ThreadedJob
    {
        public string FilePath;
        public string Source;
        public string [] References;
        public string [] Requires;
        public Assembly Assembly;
        public List<CompilerException> Exceptions = new List<CompilerException> ();
        internal int Retries;

        internal static CodeCompiler _compiler = new CodeCompiler ();
        internal CompilerParameters _parameters;
        internal static string [] _defaultReferences = new string [] { "System.dll", "mscorlib.dll", "protobuf-net.dll", "protobuf-net.Core.dll" };
        internal void _addReferences ()
        {
            _parameters.ReferencedAssemblies.Clear ();
            _parameters.ReferencedAssemblies.AddRange ( _defaultReferences );

            var assemblies = AppDomain.CurrentDomain.GetAssemblies ();
            var lastCarbon = ( Assembly )null;
            foreach ( var assembly in assemblies )
            {
                if ( CarbonLoader.AssemblyCache.Contains ( assembly ) ) continue;

                var name = assembly.GetName ().Name;

                if ( !name.StartsWith ( "Carbon" ) )
                {
                    if ( assembly.ManifestModule is ModuleBuilder builder )
                    {
                        if ( !builder.IsTransient () )
                        {
                            _parameters.ReferencedAssemblies.Add ( name );
                        }
                    }
                    else
                    {
                        _parameters.ReferencedAssemblies.Add ( assembly.GetName ().Name );
                    }
                }
                else
                {
                    lastCarbon = assembly;
                }
            }

            if ( lastCarbon != null )
            {
                _parameters.ReferencedAssemblies.Add ( lastCarbon.GetName ().Name );
            }

            foreach ( var reference in References )
            {
                _parameters.ReferencedAssemblies.Add ( reference );
            }
        }
        internal bool _addRequires ()
        {
            if ( Requires == null ) return true;

            foreach ( var require in Requires )
            {
                if ( !CarbonLoader.AssemblyDictionaryCache.TryGetValue ( require, out var assembly ) ) return false;

                if ( assembly != null ) _parameters.ReferencedAssemblies.Add ( assembly.GetName ().Name );
            }

            return true;
        }

        public class CompilerException : Exception
        {
            public string FilePath;
            public CompilerError Error;
            public CompilerException ( string filePath, CompilerError error ) { FilePath = filePath; Error = error; }

            public override string ToString ()
            {
                return $"{Error.ErrorText}\n ({FilePath} {Error.Column} line {Error.Line})";
            }
        }

        public override void Start ()
        {
            _parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false,
                IncludeDebugInformation = false,
                WarningLevel = -1
            };

            _addReferences ();
            if ( !_addRequires () )
            {
                Exceptions.Add ( new CompilerException ( FilePath, new CompilerError { ErrorText = "Couldn't find all required references." } ) );
                return;
            }

            base.Start ();
        }

        public override void ThreadFunction ()
        {
            try
            {
                Exceptions.Clear ();

                var result = _compiler.CompileAssemblyFromSource ( _parameters, Source );
                Assembly = result.CompiledAssembly;

                foreach ( CompilerError error in result.Errors )
                {
                    Exceptions.Add ( new CompilerException ( FilePath, error ) );
                }

                if ( Exceptions.Count > 0 ) throw null;
            }
            catch
            {

                if ( Retries <= 2 )
                {
                    Retries++;
                    ThreadFunction ();
                    return;
                }

                if ( Exceptions.Count > 0 )
                {
                    var exception = Exceptions [ 0 ];
                    if ( exception.Error.ErrorText.Contains ( "Mono.CSharp.CSharpParser" ) ||
                        exception.Error.ErrorText.Contains ( "Index was outside the bounds of the array." ) )
                    {
                        Retries++;
                        ThreadFunction ();
                    }
                }
            }
        }
    }
}