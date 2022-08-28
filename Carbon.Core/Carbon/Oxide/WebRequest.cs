using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PrefabAttribute;

namespace Oxide.Core.Libraries
{
    public enum RequestMethod
    {
        DELETE,
        GET,
        PATCH,
        POST,
        PUT
    }

    public class WebRequests
    {
        public void Enqueue ( string url, string body, Action<int, string> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f )
        {
            var request = new WebClient ();
            if ( !string.IsNullOrEmpty ( body ) ) callback?.Invoke ( 200, request.UploadString ( url, body ) );
            else callback?.Invoke ( 200, request.DownloadString ( url ) );
        }
    }
}
