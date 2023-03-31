using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Ext.Discord.Logging;
using Oxide.Ext.Discord.Rest.Multipart;
using RequestMethod = Oxide.Ext.Discord.Entities.Api.RequestMethod;
using Time = Oxide.Ext.Discord.Helpers.Time;

namespace Oxide.Ext.Discord.Rest
{
    /// <summary>
    /// Represent a Discord API request
    /// </summary>
    public class Request
    {
        /// <summary>
        /// HTTP request method
        /// </summary>
        public RequestMethod Method { get; }

        /// <summary>
        /// Route on the API
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Full Request URl to the API
        /// </summary>
        public string RequestUrl => UrlBase + "/" + ApiVersion + Route;

        /// <summary>
        /// Data to be sent with the request
        /// </summary>
        public object Data { get; }
        
        /// <summary>
        /// Data serialized to bytes 
        /// </summary>
        public byte[] Contents { get; set; }

        /// <summary>
        /// Attachments for a request
        /// </summary>
        internal List<IMultipartSection> MultipartSections { get; set; }
        
        /// <summary>
        /// Required If Multipart Form Request
        /// </summary>
        public bool MultipartRequest { get; }

        /// <summary>
        /// Multipart Boundary
        /// </summary>
        public string Boundary { get; set; }

        /// <summary>
        /// Response from the request
        /// </summary>
        public RestResponse Response { get; private set; }

        /// <summary>
        /// Callback to call if the request completed successfully
        /// </summary>
        public Action<RestResponse> Callback { get; }
        
        /// <summary>
        /// Callback to call if the request errored with the last error message
        /// </summary>
        public Action<RestError> OnError { get; }

        /// <summary>
        /// The DateTime the request was started
        /// Used for request timeout
        /// </summary>
        public DateTime? StartTime { get; private set; }

        /// <summary>
        /// Returns if the request is currently in progress
        /// </summary>
        public bool InProgress { get; set; }

        internal Bucket Bucket;
        
        /// <summary>
        /// Base URL for Discord
        /// </summary>
        public const string UrlBase = "https://discord.com/api";
        
        /// <summary>
        /// API Version for Rest requests
        /// </summary>
        public const string ApiVersion = "v9";
        
        private const int TimeoutDuration = 15;

        private readonly string _authHeader;
        private byte _retries;
        
        private readonly ILogger _logger;
        private RestError _lastError;
        private bool _success;
        
        private static readonly byte[] NewLine = Encoding.UTF8.GetBytes("\r\n");
        private static readonly byte[] Separator = Encoding.UTF8.GetBytes("--");

        /// <summary>
        /// Creates a new request
        /// </summary>
        /// <param name="method">HTTP method to call</param>
        /// <param name="route">Route to call on the API</param>
        /// <param name="data">Data for the request</param>
        /// <param name="authHeader">Authorization Header</param>
        /// <param name="callback">Callback once the request completes successfully</param>
        /// <param name="onError">Callback when the request errors</param>
        /// <param name="logger">Logger for the request</param>
        public Request(RequestMethod method, string route, object data, string authHeader, Action<RestResponse> callback, Action<RestError> onError, ILogger logger)
        {
            Method = method;
            Route = route;
            Data = data;
            _authHeader = authHeader;
            Callback = callback;
            OnError = onError;
            _logger = logger;
            MultipartRequest = Data is IFileAttachments attachments && attachments.FileAttachments != null && attachments.FileAttachments.Count != 0;
        }

        /// <summary>
        /// Fires the request off
        /// </summary>
        public void Fire()
        {
            InProgress = true;
            StartTime = DateTime.UtcNow;

            HttpWebRequest req = CreateRequest();
            
            try
            {
                //Can timeout while writing request data
                WriteRequestData(req);

                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        ParseResponse(response);
                    }
                }

                _success = true;
                Interface.Oxide.NextTick(() =>
                {
                    Callback?.Invoke(Response);
                });
                Close();
            }
            catch (WebException ex)
            {
                using (HttpWebResponse httpResponse = ex.Response as HttpWebResponse)
                {
                    _lastError = new RestError(ex, req.RequestUri, Method, Data);
                    if (httpResponse == null)
                    {
                        Bucket.ErrorDelayUntil = Time.TimeSinceEpoch() + 1;
                        Close(false);
                        _logger.Exception($"A web request exception occured (internal error) [RETRY={_retries.ToString()}/3].\nRequest URL: [{req.Method}] {req.RequestUri}", ex);
                        return;
                    }

                    int statusCode = (int) httpResponse.StatusCode;
                    _lastError.HttpStatusCode = statusCode;
                        
                    string message = ParseResponse(ex.Response);
                    _lastError.Message = message;
                        
                    bool isRateLimit = statusCode == 429;
                    if (isRateLimit)
                    {
                        _logger.Warning($"Discord rate limit reached. (Rate limit info: remaining: [{req.Method}] Route: {req.RequestUri} Content-Type: {req.ContentType} Remaining: {Bucket.RateLimitRemaining.ToString()} Limit: {Bucket.RateLimit.ToString()}, Reset In: {Bucket.RateLimitReset.ToString()}, Current Time: {Time.TimeSinceEpoch().ToString()}");
                        Close(false);
                        return;
                    }

                    DiscordApiError apiError = Response.ParseData<DiscordApiError>();
                    _lastError.DiscordError = apiError;
                    if (apiError != null && apiError.Code != 0)
                    {
                        _logger.Error($"Discord API has returned error Discord Code: {apiError.Code.ToString()} Discord Error: {apiError.Message} Request: [{req.Method}] {req.RequestUri} (Response Code: {httpResponse.StatusCode.ToString()}) Content-Type: {req.ContentType}" +
                                      $"\nDiscord Errors: {apiError.Errors}" +
                                      $"\nRequest Body:\n{(Contents != null ? Encoding.UTF8.GetString(Contents) : "Contents is null")}");
                    }
                    else
                    {
                        _logger.Error($"An error occured whilst submitting a request: Exception Status: {ex.Status.ToString()} Request: [{req.Method}] {req.RequestUri} (Response Code: {httpResponse.StatusCode.ToString()}): {message}");
                    }

                    Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Exception($"An exception occured for request: [{req.Method}] {req.RequestUri}", ex);
                Close();
            }
        }

        private HttpWebRequest CreateRequest()
        {
            SetRequestBody();
            
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(RequestUrl);
            req.Method = Method.ToString();
            req.UserAgent = $"DiscordBot (https://github.com/Kirollos/Oxide.Ext.Discord, {DiscordExtension.GetExtensionVersion})";
            req.Timeout = TimeoutDuration * 1000;
            req.ContentLength = 0;
            req.Headers.Set("Authorization", _authHeader);
            req.ContentType = MultipartRequest ? $"multipart/form-data;boundary=\"{Boundary}\"" : "application/json" ;

            return req;
        }

        private byte[] GetMultipartFormData()
        {
            StringBuilder sb = new StringBuilder();
            byte[] boundary = Encoding.UTF8.GetBytes(Boundary);

            List<byte> data = new List<byte>();

            foreach (IMultipartSection section in MultipartSections)
            {
                AddMultipartSection(sb, section, data, boundary);
            }

            data.AddRange(NewLine);
            data.AddRange(Separator);
            data.AddRange(boundary);
            data.AddRange(Separator);
            data.AddRange(NewLine);
            
            return data.ToArray();
        }

        private void AddMultipartSection(StringBuilder sb, IMultipartSection section, List<byte> data, byte[] boundary)
        {
            sb.Length = 0;
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append(section.SectionName);
            sb.Append("\"");
            if (section.FileName != null)
            {
                sb.Append("; filename=\"");
                sb.Append(section.FileName);
                sb.Append("\"");
            }

            if (!string.IsNullOrEmpty(section.ContentType))
            {
                sb.AppendLine();
                sb.Append("Content-Type: ");
                sb.Append(section.ContentType);
            }

            sb.AppendLine();
            
            data.AddRange(NewLine);
            data.AddRange(Separator);
            data.AddRange(boundary);
            data.AddRange(NewLine);
            data.AddRange(Encoding.UTF8.GetBytes(sb.ToString()));
            data.AddRange(NewLine);
            data.AddRange(section.Data);
        }
        
        private void SetRequestBody()
        {
            if (Data == null || Contents != null)
            {
                return;
            }
            
            if (MultipartRequest)
            {
                IFileAttachments attachments = (IFileAttachments)Data;
                MultipartSections = new List<IMultipartSection> {new MultipartFormSection("payload_json", Data, "application/json")};
                for (int index = 0; index < attachments.FileAttachments.Count; index++)
                {
                    MessageFileAttachment fileAttachment = attachments.FileAttachments[index];
                    MultipartSections.Add(new MultipartFileSection($"files[{(index + 1).ToString()}]", fileAttachment.FileName, fileAttachment.Data, fileAttachment.ContentType));
                }

                Boundary = Guid.NewGuid().ToString().Replace("-", "");
                Contents = GetMultipartFormData();
            }
            else
            {
                Contents = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Data, DiscordExtension.ExtensionSerializeSettings));
            }
        }

        /// <summary>
        /// Closes the request and removes it from the bucket
        /// </summary>
        /// <param name="remove"></param>
        public void Close(bool remove = true)
        {
            _retries += 1;
            if (remove || _retries >= 3)
            {
                if (!_success)
                {
                    try
                    {
                        Interface.Oxide.NextTick(() =>
                        {
                            OnError?.Invoke(_lastError);
                        });
                    }
                    catch(Exception ex)
                    {
                        _logger.Exception($"An exception occured during OnError callback for request: [{Method.ToString()}] {RequestUrl}", ex);
                    }
                }

                Bucket.DequeueRequest(this);
            }
            else
            {
                InProgress = false;
                StartTime = null;
            }
        }

        /// <summary>
        /// Returns true if the request has timed out
        /// </summary>
        /// <returns></returns>
        public bool HasTimedOut()
        {
            if (!InProgress || StartTime == null)
            {
                return false;
            }

            return (DateTime.UtcNow - StartTime.Value).TotalSeconds > TimeoutDuration;
        }

        private void WriteRequestData(WebRequest request)
        {
            if (Contents == null || Contents.Length == 0)
            {
                return;
            }

            request.ContentLength = Contents.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(Contents, 0, Contents.Length);
            }
        }

        private string ParseResponse(WebResponse response)
        {
            using (Stream stream = response.GetResponseStream())
            {
                if (stream == null)
                {
                    return null;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string message = reader.ReadToEnd().Trim();
                    Response = new RestResponse(message);

                    ParseHeaders(response.Headers, Response);

                    return message;
                }
            }
        }

        private void ParseHeaders(WebHeaderCollection headers, RestResponse response)
        {
            string globalRetryAfterHeader = headers.Get("Retry-After");
            string isGlobalRateLimitHeader = headers.Get("X-RateLimit-Global");

            if (!string.IsNullOrEmpty(globalRetryAfterHeader) &&
                !string.IsNullOrEmpty(isGlobalRateLimitHeader) &&
                int.TryParse(globalRetryAfterHeader, out int globalRetryAfter) &&
                bool.TryParse(isGlobalRateLimitHeader, out bool isGlobalRateLimit) &&
                isGlobalRateLimit)
            {
                RateLimit limit = response.ParseData<RateLimit>();
                if (limit.Global)
                {
                    Bucket.Handler.RateLimit.ReachedRateLimit(globalRetryAfter);
                }
            }

            string bucketLimitHeader = headers.Get("X-RateLimit-Limit");
            string bucketRemainingHeader = headers.Get("X-RateLimit-Remaining");
            string bucketResetAfterHeader = headers.Get("X-RateLimit-Reset-After");
            string bucketNameHeader = headers.Get("X-RateLimit-Bucket");

            if (!string.IsNullOrEmpty(bucketLimitHeader) &&
                int.TryParse(bucketLimitHeader, out int bucketLimit))
            {
                Bucket.RateLimit = bucketLimit;
            }

            if (!string.IsNullOrEmpty(bucketRemainingHeader) &&
                int.TryParse(bucketRemainingHeader, out int bucketRemaining))
            {
                Bucket.RateLimitRemaining = bucketRemaining;
            }

            double timeSince = Time.TimeSinceEpoch();
            if (!string.IsNullOrEmpty(bucketResetAfterHeader) &&
                double.TryParse(bucketResetAfterHeader, out double bucketResetAfter))
            {
                double resetTime = timeSince + bucketResetAfter;
                if (resetTime > Bucket.RateLimitReset)
                {
                    Bucket.RateLimitReset = resetTime;
                }
            }
            
            _logger.Debug($"Method: {Method.ToString()} Route: {Route} Internal Bucket Id: {Bucket.BucketId} Limit: {Bucket.RateLimit.ToString()} Remaining: {Bucket.RateLimitRemaining.ToString()} Reset: {Bucket.RateLimitReset.ToString()} Time: {Time.TimeSinceEpoch().ToString()} Bucket: {bucketNameHeader}");
        }
    }
}