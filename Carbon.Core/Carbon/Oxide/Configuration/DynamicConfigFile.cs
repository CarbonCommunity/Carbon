using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Oxide.Core.Configuration
{
    public class DynamicConfigFile : ConfigFile, IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        public JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings ();

        public DynamicConfigFile ( string filename ) : base ( filename )
        {
            this._keyvalues = new Dictionary<string, object> ();
            this._settings = new JsonSerializerSettings ();
            this._settings.Converters.Add ( new KeyValuesConverter () );
            this._chroot = Interface.Oxide.InstanceDirectory;
        }

        public override void Load ( string filename = null )
        {
            filename = this.CheckPath ( filename ?? base.Filename );
            string value = File.ReadAllText ( filename );
            this._keyvalues = JsonConvert.DeserializeObject<Dictionary<string, object>> ( value, this._settings );
        }

        public T ReadObject<T> ( string filename = null )
        {
            filename = this.CheckPath ( filename ?? base.Filename );
            T t;
            if ( this.Exists ( filename ) )
            {
                t = JsonConvert.DeserializeObject<T> ( File.ReadAllText ( filename ), this.Settings );
            }
            else
            {
                t = Activator.CreateInstance<T> ();
                this.WriteObject<T> ( t, false, filename );
            }
            return t;
        }

        public override void Save ( string filename = null )
        {
            filename = this.CheckPath ( filename ?? base.Filename );
            string directoryName = Utility.GetDirectoryName ( filename );
            if ( directoryName != null && !Directory.Exists ( directoryName ) )
            {
                Directory.CreateDirectory ( directoryName );
            }
            File.WriteAllText ( filename, JsonConvert.SerializeObject ( this._keyvalues, Formatting.Indented, this._settings ) );
        }

        public void WriteObject<T> ( T config, bool sync = false, string filename = null )
        {
            filename = this.CheckPath ( filename ?? base.Filename );
            string directoryName = Utility.GetDirectoryName ( filename );
            if ( directoryName != null && !Directory.Exists ( directoryName ) )
            {
                Directory.CreateDirectory ( directoryName );
            }
            string text = JsonConvert.SerializeObject ( config, Formatting.Indented, this.Settings );
            File.WriteAllText ( filename, text );
            if ( sync )
            {
                this._keyvalues = JsonConvert.DeserializeObject<Dictionary<string, object>> ( text, this._settings );
            }
        }

        public bool Exists ( string filename = null )
        {
            filename = this.CheckPath ( filename ?? base.Filename );
            string directoryName = Utility.GetDirectoryName ( filename );
            return ( directoryName == null || Directory.Exists ( directoryName ) ) && File.Exists ( filename );
        }

        private string CheckPath ( string filename )
        {
            filename = DynamicConfigFile.SanitizeName ( filename );
            string fullPath = Path.GetFullPath ( filename );
            if ( !fullPath.StartsWith ( this._chroot, StringComparison.Ordinal ) )
            {
                CarbonCore.Log ( $"{fullPath}" );
                throw new Exception ( "Only access to Carbon directory!\nPath: " + fullPath );
            }
            return fullPath;
        }

        public static string SanitizeName ( string name )
        {
            if ( string.IsNullOrEmpty ( name ) )
            {
                return string.Empty;
            }
            name = name.Replace ( '\\', Path.DirectorySeparatorChar ).Replace ( '/', Path.DirectorySeparatorChar );
            name = Regex.Replace ( name, "[" + Regex.Escape ( new string ( Path.GetInvalidPathChars () ) ) + "]", "_" );
            name = Regex.Replace ( name, "\\.+", "." );
            return name.TrimStart ( new char []
            {
                '.'
            } );
        }

        [Obsolete ( "SanitiseName is deprecated, use SanitizeName instead" )]
        public static string SanitiseName ( string name )
        {
            return DynamicConfigFile.SanitizeName ( name );
        }

        public void Clear ()
        {
            this._keyvalues.Clear ();
        }

        public void Remove ( string key )
        {
            this._keyvalues.Remove ( key );
        }

        public object this [ string key ]
        {
            get
            {
                object result;
                if ( !this._keyvalues.TryGetValue ( key, out result ) )
                {
                    return null;
                }
                return result;
            }
            set
            {
                this._keyvalues [ key ] = value;
            }
        }

        public object this [ string keyLevel1, string keyLevel2 ]
        {
            get
            {
                return this.Get ( new string []
                {
                    keyLevel1,
                    keyLevel2
                } );
            }
            set
            {
                this.Set ( new object []
                {
                    keyLevel1,
                    keyLevel2,
                    value
                } );
            }
        }

        public object this [ string keyLevel1, string keyLevel2, string keyLevel3 ]
        {
            get
            {
                return this.Get ( new string []
                {
                    keyLevel1,
                    keyLevel2,
                    keyLevel3
                } );
            }
            set
            {
                this.Set ( new object []
                {
                    keyLevel1,
                    keyLevel2,
                    keyLevel3,
                    value
                } );
            }
        }

        public object ConvertValue ( object value, Type destinationType )
        {
            if ( !destinationType.IsGenericType )
            {
                return Convert.ChangeType ( value, destinationType );
            }
            if ( destinationType.GetGenericTypeDefinition () == typeof ( List<> ) )
            {
                Type conversionType = destinationType.GetGenericArguments () [ 0 ];
                IList list = ( IList )Activator.CreateInstance ( destinationType );
                foreach ( object value2 in ( ( IList )value ) )
                {
                    list.Add ( Convert.ChangeType ( value2, conversionType ) );
                }
                return list;
            }
            if ( destinationType.GetGenericTypeDefinition () == typeof ( Dictionary<,> ) )
            {
                Type conversionType2 = destinationType.GetGenericArguments () [ 0 ];
                Type conversionType3 = destinationType.GetGenericArguments () [ 1 ];
                IDictionary dictionary = ( IDictionary )Activator.CreateInstance ( destinationType );
                foreach ( object obj in ( ( IDictionary )value ).Keys )
                {
                    dictionary.Add ( Convert.ChangeType ( obj, conversionType2 ), Convert.ChangeType ( ( ( IDictionary )value ) [ obj ], conversionType3 ) );
                }
                return dictionary;
            }
            throw new InvalidCastException ( "Generic types other than List<> and Dictionary<,> are not supported" );
        }

        public T ConvertValue<T> ( object value )
        {
            return ( T )( ( object )this.ConvertValue ( value, typeof ( T ) ) );
        }

        public object Get ( params string [] path )
        {
            if ( path.Length < 1 )
            {
                throw new ArgumentException ( "path must not be empty" );
            }
            object obj;
            if ( !this._keyvalues.TryGetValue ( path [ 0 ], out obj ) )
            {
                return null;
            }
            for ( int i = 1; i < path.Length; i++ )
            {
                Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
                if ( dictionary == null || !dictionary.TryGetValue ( path [ i ], out obj ) )
                {
                    return null;
                }
            }
            return obj;
        }

        public T Get<T> ( params string [] path )
        {
            return this.ConvertValue<T> ( this.Get ( path ) );
        }

        public void Set ( params object [] pathAndTrailingValue )
        {
            if ( pathAndTrailingValue.Length < 2 )
            {
                throw new ArgumentException ( "path must not be empty" );
            }
            string [] array = new string [ pathAndTrailingValue.Length - 1 ];
            for ( int i = 0; i < pathAndTrailingValue.Length - 1; i++ )
            {
                array [ i ] = ( string )pathAndTrailingValue [ i ];
            }
            object value = pathAndTrailingValue [ pathAndTrailingValue.Length - 1 ];
            if ( array.Length == 1 )
            {
                this._keyvalues [ array [ 0 ] ] = value;
                return;
            }
            object obj;
            if ( !this._keyvalues.TryGetValue ( array [ 0 ], out obj ) )
            {
                obj = ( this._keyvalues [ array [ 0 ] ] = new Dictionary<string, object> () );
            }
            for ( int j = 1; j < array.Length - 1; j++ )
            {
                if ( !( obj is Dictionary<string, object> ) )
                {
                    throw new ArgumentException ( "path is not a dictionary" );
                }
                Dictionary<string, object> dictionary = ( Dictionary<string, object> )obj;
                if ( !dictionary.TryGetValue ( array [ j ], out obj ) )
                {
                    obj = ( dictionary [ array [ j ] ] = new Dictionary<string, object> () );
                }
            }
            ( ( Dictionary<string, object> )obj ) [ array [ array.Length - 1 ] ] = value;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator ()
        {
            return this._keyvalues.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return this._keyvalues.GetEnumerator ();
        }

        private Dictionary<string, object> _keyvalues;

        private readonly JsonSerializerSettings _settings;

        private readonly string _chroot;
    }
}
