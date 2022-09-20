using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Carbon.Core.Oxide.Extensions
{
    public static class HttpWebRequestExtensions
    {
        static HttpWebRequestExtensions ()
        {
            var typeFromHandle = typeof ( HttpWebRequest );

            foreach ( string text in RestrictedHeaders )
            {
                HeaderProperties [ text ] = typeFromHandle.GetProperty ( text.Replace ( "-", "" ) );
            }
        }

        public static void SetRawHeaders ( this WebRequest request, Dictionary<string, string> headers )
        {
            foreach ( KeyValuePair<string, string> keyValuePair in headers )
            {
                request.SetRawHeader ( keyValuePair.Key, keyValuePair.Value );
            }
        }
        public static void SetRawHeader ( this WebRequest request, string name, string value )
        {
            if ( !HeaderProperties.ContainsKey ( name ) )
            {
                request.Headers [ name ] = value;
                return;
            }
            var propertyInfo = HeaderProperties [ name ];
            if ( propertyInfo.PropertyType == typeof ( DateTime ) )
            {
                propertyInfo.SetValue ( request, DateTime.Parse ( value ), null );
                return;
            }
            if ( propertyInfo.PropertyType == typeof ( bool ) )
            {
                propertyInfo.SetValue ( request, bool.Parse ( value ), null );
                return;
            }
            if ( propertyInfo.PropertyType == typeof ( long ) )
            {
                propertyInfo.SetValue ( request, long.Parse ( value ), null );
                return;
            }
            propertyInfo.SetValue ( request, value, null );
        }

        private static readonly string [] RestrictedHeaders = new string []
        {
            "Accept",
            "Connection",
            "Content-Length",
            "Content-Type",
            "Date",
            "Expect",
            "Host",
            "If-Modified-Since",
            "Keep-Alive",
            "Proxy-Connection",
            "Range",
            "Referer",
            "Transfer-Encoding",
            "User-Agent"
        };

        private static readonly Dictionary<string, PropertyInfo> HeaderProperties = new Dictionary<string, PropertyInfo> ( StringComparer.OrdinalIgnoreCase );
    }
}
