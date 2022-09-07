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
        public string FileName;
        public string Source;
        public string [] References;
        public Assembly Assembly;
        public List<CompilerException> Exceptions = new List<CompilerException> ();

        internal CodeCompiler _compiler;
        internal CompilerParameters _parameters;
        internal int _retries;
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

        public class CompilerException : Exception
        {
            public string FileName;
            public CompilerError Error;
            public CompilerException ( string fileName, CompilerError error ) { FileName = fileName; Error = error; }

            public override string ToString ()
            {
                return $"{Error.ErrorText}\n ({FileName} {Error.Column} line {Error.Line})";
            }
        }

        public override void Start ()
        {
            _compiler = new CodeCompiler ();
            _parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };

            _addReferences ();

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
                    Exceptions.Add ( new CompilerException ( FileName, error ) );
                }

                if ( Exceptions.Count > 0 ) throw null;
            }
            catch
            {
                if ( _retries <= 2 )
                {
                    _retries++;
                    ThreadFunction ();
                }
            }
        }
    }
}