///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Facepunch;
using Humanlights.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Application = UnityEngine.Application;
using LanguageVersion = Microsoft.CodeAnalysis.CSharp.LanguageVersion;

namespace Carbon.Core
{
    public class AsyncPluginLoader : ThreadedJob
    {
        public string FilePath;
        public string FileName;
        public string Source;
        public string [] References;
        public string [] Requires;
        public float CompileTime;
        public Assembly Assembly;
        public List<CompilerException> Exceptions = new List<CompilerException> ();
        internal RealTimeSince TimeSinceCompile;

        internal static int _assemblyIndex = 0;
        internal static bool _hasInit { get; set; }
        internal static void _doInit ()
        {
            if ( _hasInit ) return;
            _hasInit = true;

            _metadataReferences.Add ( MetadataReference.CreateFromStream ( new MemoryStream ( Properties.Resources.Humanlights_System ) ) );
            _metadataReferences.Add ( MetadataReference.CreateFromStream ( new MemoryStream ( Properties.Resources.Humanlights_Unity ) ) );
            _metadataReferences.Add ( MetadataReference.CreateFromStream ( new MemoryStream ( Properties.Resources.protobuf_net ) ) );
            _metadataReferences.Add ( MetadataReference.CreateFromStream ( new MemoryStream ( Properties.Resources.protobuf_net_Core ) ) );
            _metadataReferences.Add ( MetadataReference.CreateFromStream ( new MemoryStream ( OsEx.File.ReadBytes ( CarbonCore.DllPath ) ) ) );

            var assemblies = AppDomain.CurrentDomain.GetAssemblies ();

            foreach ( var assembly in assemblies )
            {
                if ( assembly.IsDynamic || !OsEx.File.Exists ( assembly.Location ) || CarbonLoader.AssemblyCache.Contains ( assembly ) ) continue;

                _metadataReferences.Add ( MetadataReference.CreateFromFile ( assembly.Location ) );
            }
        }

        internal static List<MetadataReference> _metadataReferences = new List<MetadataReference> ();
        internal static Dictionary<string, MetadataReference> _referenceCache = new Dictionary<string, MetadataReference> ();

        internal static MetadataReference _getReferenceFromCache ( string reference )
        {
            if ( !_referenceCache.TryGetValue ( reference, out var metaReference ) )
            {
                _referenceCache.Add ( reference, MetadataReference.CreateFromFile ( Path.Combine ( Application.dataPath, "..", "RustDedicated_Data", "Managed", $"{reference}.dll" ) ) );
            }

            return metaReference;
        }

        internal List<MetadataReference> _addReferences ()
        {
            var references = Pool.GetList<MetadataReference> ();
            references.AddRange ( _metadataReferences );

            foreach ( var reference in References )
            {
                if ( string.IsNullOrEmpty ( reference ) || _metadataReferences.Any ( x => x.Display.Contains ( reference ) ) ) continue;

                references.Add ( _getReferenceFromCache ( reference ) );
            }

            return references;
        }
        internal bool _addRequires ()
        {
            if ( Requires == null ) return true;

            foreach ( var require in Requires )
            {
                if ( !CarbonLoader.AssemblyDictionaryCache.TryGetValue ( require, out var assembly ) ) return false;

                // if ( assembly != null ) _options.ReferencedAssemblies.Add ( assembly.GetName ().Name );
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
            try
            {

                FileName = Path.GetFileNameWithoutExtension ( FilePath );

                _doInit ();

                if ( !_addRequires () )
                {
                    Exceptions.Add ( new CompilerException ( FilePath, new CompilerError { ErrorText = "Couldn't find all required references." } ) );
                    return;
                }
            }
            catch ( Exception ex ) { Console.WriteLine ( $"Couldn't compile '{FileName}'\n{ex}" ); }
           
            base.Start ();
        }

        public override void ThreadFunction ()
        {
            try
            {
                Exceptions.Clear ();

                TimeSinceCompile = 0;

                var references = _addReferences ();
                var tree = Pool.GetList<SyntaxTree> ();
                tree.Add ( CSharpSyntaxTree.ParseText ( Source ) );

                foreach ( var require in Requires )
                {
                    try
                    {
                        tree.Add ( CSharpSyntaxTree.ParseText ( OsEx.File.ReadText ( Path.Combine ( CarbonCore.GetPluginsFolder (), $"{require}.cs" ) ) ) );
                    }
                    catch ( Exception treeEx )
                    {
                        throw treeEx;
                    }
                }

                var options = new CSharpCompilationOptions ( OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, warningLevel: 4 );
                var compilation = CSharpCompilation.Create ( $"{FileName}_{RandomEx.GetRandomInteger ()}", tree, references, options );

                using ( var dllStream = new MemoryStream () )
                {
                    var emit = compilation.Emit ( dllStream );

                    foreach ( var error in emit.Diagnostics )
                    {
                        var span = error.Location.GetMappedLineSpan ().Span;
                        switch ( error.Severity )
                        {
                            case DiagnosticSeverity.Error:
                                Exceptions.Add ( new CompilerException ( FilePath, new CompilerError ( FileName, span.Start.Line + 1, span.Start.Character + 1, error.Id, error.GetMessage ( CultureInfo.InvariantCulture ) ) ) );
                                break;
                        }
                    }

                    if ( emit.Success )
                    {
                        Assembly = Assembly.Load ( dllStream.ToArray () );
                    }
                }

                CompileTime = TimeSinceCompile;

                Pool.FreeList ( ref references );
                Pool.FreeList ( ref tree );

                if ( Exceptions.Count > 0 ) throw null;
            }
            catch ( Exception ex ) { Console.WriteLine ( $"Couldn't compile '{FileName}'\n{ex}" ); }
        }

        private SyntaxTree GetSyntaxTree ( string code, params string [] defines )
        {
            return SyntaxFactory.ParseSyntaxTree ( SourceText.From ( code, Encoding.UTF8 ), GetOptions ( defines ) );
        }

        private CSharpParseOptions GetOptions ( string [] defines )
        {
            return new CSharpParseOptions ( languageVersion: LanguageVersion.CSharp9, preprocessorSymbols: defines );
        }
    }
}