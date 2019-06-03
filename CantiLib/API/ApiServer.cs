//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using static Canti.Utils;

// TODO - Configurable rate limiter
// TODO - HTTPS/SSL support?
namespace Canti
{
    /// <summary>
    /// The type of HTTP request this method expects
    /// </summary>
    public enum ApiRequestType
    {
        GET,
        POST
    }

    /// <summary>
    /// This is used to label api methods in our source
    /// </summary>
    public sealed class ApiMethod : Attribute
    {
        /// <summary>
        /// The name of this method request
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// The type of HTTP request this method expects (default = get)
        /// </summary>
        public ApiRequestType Type { get; set; }

        /// <summary>
        /// Whether or not this method requires validation (default = false)
        /// </summary>
        public bool RequiresValidation { get; set; }

        /// <summary>
        /// Specifies a function is an API method
        /// </summary>
        /// <param name="MethodName">The name of this method request</param>
        /// <param name="RequiresValidation">Whether or not this method requires validation (default = false)</param>
        /// <param name="Type">The type of HTTP request this method expects (default = get)</param>
        public ApiMethod(string MethodName, bool RequiresValidation = false, ApiRequestType Type = ApiRequestType.GET)
        {
            this.MethodName = MethodName.ToUpper();
            this.RequiresValidation = RequiresValidation;
            this.Type = Type;
        }
    }

    // Contains information about invoke-able methods found in a context list
    internal struct InvokeableMethod
    {
        internal IMethodContext Context { get; set; }
        internal MethodInfo Method { get; set; }
        internal bool RequiresValidation { get; set; }
        internal ApiRequestType Type { get; set; }
    }

    /// <summary>
    /// Contains a list of API method contexts
    /// </summary>
    public sealed class ApiMethodContext
    {
        #region Properties and Fields

        // Contains the raw list of API method contexts
        List<IMethodContext> ContextList;

        #endregion

        #region Operators

        /// <summary>
        /// Adds an API method context to the list
        /// </summary>
        /// <param name="List">The list to add the API method context to</param>
        /// <param name="Context">The API method context you wish to add</param>
        /// <returns>The list containing the newly added API method context</returns>
        public static ApiMethodContext operator +(ApiMethodContext List, IMethodContext Context)
        {
            if (List == null) List = new ApiMethodContext();
            lock (List.ContextList)
            {
                if (!List.ContextList.Contains(Context)) List.ContextList.Add(Context);
            }
            return List;
        }

        #endregion

        #region Methods

        // Tries to find a method by name from the list of contexts
        // TODO - Could probably replace all these foreachs with linq
        internal List<InvokeableMethod> GetMethods(string MethodName)
        {
            lock (ContextList)
            {
                // Create a list of invokeable methods
                var Methods = new List<InvokeableMethod>();

                // Loop through all given context lists
                foreach (var MethodContext in ContextList)
                {
                    // Loop through methods in the current context
                    foreach (var MethodInfo in MethodContext.GetType().GetMethods())
                    {
                        // Loop through custom attributes
                        foreach (var Attribute in MethodInfo.GetCustomAttributes(true))
                        {
                            // Check if this attribute matches the method name
                            if (!(Attribute is ApiMethod)|| ((ApiMethod)Attribute).MethodName != MethodName)
                            {
                                // Not applicable, continue
                                continue;
                            }

                            // Suitable method found, add to list
                            Methods.Add(new InvokeableMethod
                            {
                                Context = MethodContext,
                                RequiresValidation = ((ApiMethod)Attribute).RequiresValidation,
                                Type = ((ApiMethod)Attribute).Type,
                                Method = MethodInfo
                            });
                        }
                    }
                }

                // Return result
                return Methods;
            }
        }

        #endregion

        #region Constructors

        // Initializes an empty context list
        internal ApiMethodContext()
        {
            ContextList = new List<IMethodContext>();
        }

        #endregion
    }

    /// <summary>
    /// A standalone rest API server
    /// </summary>
    public sealed class ApiServer
    {
        #region Properties and Fields

        #region Public

        /// <summary>
        /// The logger this server will use to report errors
        /// </summary>
        public Logger Logger { get; set; }

        /// <summary>
        /// The port this server is binded to
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// A list of API method contexts that requests will pull from
        /// </summary>
        public ApiMethodContext MethodContexts { get; set; }

        #endregion

        #region Private

        // The HTTP listener that listens for new requests
        private HttpListener Listener { get; set; }

        // The thread our listener runs on
        private Thread ListenerThread { get; set; }

        // An array of worker threads that handle incoming requests
        private Thread[] WorkerThreads { get; set; }

        // An event that is set when the server is stopped
        private ManualResetEvent StopEvent { get; set; }

        // An event that is set when the server detects a new request
        private ManualResetEvent ReadyEvent { get; set; }

        // A queue of incoming requests waiting to be processed
        private Queue<HttpListenerContext> ContextQueue { get; set; }

        // Password for API methods requiring credentials
        private string Password { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Starts listening for API requests
        /// </summary>
        /// <param name="Port">The port to listen for incoming requests on</param>
        public void Start(int Port)
        {
            // Store port
            this.Port = Port;

            // Start HTTP listener
            try
            {
                Listener = new HttpListener();
                Listener.Prefixes.Add($"http://+:{Port}/");
                Listener.Start();
            }
            catch
            {
                // TODO - better error handling
                Listener = new HttpListener();
                Listener.Prefixes.Add($"http://127.0.0.1:{Port}/");
                Listener.Start();
            }

            // Begin listener thread
            ListenerThread.Start();

            // Start our worker threads
            for (int i = 0; i < WorkerThreads.Length; i++)
            {
                WorkerThreads[i] = new Thread(AcceptContext);
                WorkerThreads[i].Start();
            }
        }

        /// <summary>
        /// Stops the server and ends all associated threads
        /// </summary>
        public void Stop()
        {
            StopEvent.Set();
            ListenerThread.Join();
            foreach (Thread worker in WorkerThreads) worker.Join();
            Listener.Stop();
        }

        #endregion

        #region Private

        // Listens for new requests and enqueues them
        private void Listen()
        {
            while (Listener.IsListening)
            {
                // Accept new connection context
                var Context = Listener.BeginGetContext((IAsyncResult Result) =>
                {
                    try
                    {
                        // Lock our context queue to prevent race conditions
                        lock (ContextQueue)
                        {
                            // Add new connection context to our context queue
                            ContextQueue.Enqueue(Listener.EndGetContext(Result));

                            // Signal that a context is ready to be accepted
                            ReadyEvent.Set();
                        }
                    }
                    catch { }
                }, null);

                // Wait for exit
                if (WaitHandle.WaitAny(new[] { StopEvent, Context.AsyncWaitHandle }) == 0) return;
            }
        }

        // Accepts new connection contexts
        private void AcceptContext()
        {
            // Create a wait handle array so we can cancel this thread if need be
            WaitHandle[] Wait = new[] { ReadyEvent, StopEvent };
            while (0 == WaitHandle.WaitAny(Wait))
            {
                // Lock our context queue to prevent race conditions
                lock (ContextQueue)
                {
                    // Context queue has entries, accept one
                    if (ContextQueue.Count > 0)
                    {
                        // Dequeue next context in line
                        var Context = ContextQueue.Dequeue();

                        // Handle this context
                        HandleRequest(Context);
                    }

                    // There are no entries in the connection queue
                    else
                    {
                        // No context in line, reset ready event
                        ReadyEvent.Reset();
                        continue;
                    }
                }
            }
        }

        // Handles an incoming request
        private void HandleRequest(HttpListenerContext HttpContext)
        {
            string Result = "";
            try
            {
                // Get request information
                var Request = HttpContext.Request;
                string RequestType = Request.HttpMethod;
                string Password = Request.Headers["Password"];
                string MethodName = Request.Url.Segments[2].Replace("/", "").ToUpper();
                int Version = int.Parse(Request.Url.Segments[1].Replace("/", ""));

                // Check if requested method exists
                var Methods = MethodContexts.GetMethods(MethodName);
                if (!Methods.Any())
                {
                    throw new InvalidOperationException("Invalid API method");
                }

                // Loop through found methods
                // TODO - also check # of params needed when getting methods
                foreach (var Entry in Methods)
                {
                    // Check request type
                    // TODO - This looks ugly... Clean it up
                    if ((Entry.Type == ApiRequestType.POST && RequestType != "POST") ||
                        (Entry.Type == ApiRequestType.GET && RequestType != "GET")) continue;

                    // Check request version against context version
                    if (!Entry.Context.CheckVersion(Version)) continue;

                    // Check request validation
                    if (Entry.RequiresValidation && Password != this.Password) continue;

                    // Get method parameters
                    var MethodParams = Entry.Method.GetParameters();

                    // Get request parameters
                    object[] Params = new object[MethodParams.Length];
                    if (RequestType == "GET")
                    {
                        // Get our request's params directly from URL string
                        string[] RequestParams = Request.Url.Segments.Skip(3).Select(x => x.Replace("/", "")).ToArray();

                        // Populate our parameters
                        for (int i = 0; i < MethodParams.Length; i++)
                        {
                            // Check if this parameter is present in the request
                            if (i < RequestParams.Length)
                            {
                                // Convert to method parameter type
                                Params[i] = Convert.ChangeType(RequestParams[i], MethodParams[i].ParameterType);
                            }

                            // Otherwise default to a null value
                            else continue;
                        }
                    }
                    else if (RequestType == "POST")
                    {
                        // Get our request's params from the stream data
                        var RequestParams = new Dictionary<string, dynamic>();
                        using (var Reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
                        {
                            var Json = JsonConvert.DeserializeObject<JObject>(Reader.ReadToEnd());
                            RequestParams = new Dictionary<string, object>(
                                Json.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);
                        }

                        // Populate our parameters
                        for (int i = 0; i < MethodParams.Length; i++)
                        {
                            // Check if this parameter is present in the request
                            if (RequestParams.ContainsKey(MethodParams[i].Name))
                            {
                                // Convert to method parameter type
                                Params[i] = Convert.ChangeType(RequestParams[MethodParams[i].Name], MethodParams[i].ParameterType);
                            }

                            // Otherwise default to a null value
                            else continue;
                        }
                    }

                    // Try invoke method
                    var Output = (string)Entry.Method.Invoke(Entry.Context, Params);

                    // TODO - better (more efficient) way to catch errors, yikes
                    if (JsonConvert.DeserializeObject<JObject>(Output).TryGetValue("error", out _))
                    {
                        continue;
                    }
                    Result = Output;
                    break;
                }

                // Check if result is empty
                if (string.IsNullOrEmpty(Result))
                {
                    throw new InvalidOperationException("Invalid API method");
                }
            }

            // A known error was caught
            catch (InvalidOperationException e)
            {
                Result = new JObject
                {
                    ["error"] = e.Message
                }.ToString();
            }

            // Unable to parse this request
            catch
            {
                Result = new JObject
                {
                    ["error"] = "Invalid request"
                }.ToString();
            }

            // Populate a new HTTP header and send our response
            var Buffer = Encoding.UTF8.GetBytes(Result);
            var Response = HttpContext.Response;
            Response.ContentLength64 = Buffer.Length;
            using (var st = Response.OutputStream)
                st.Write(Buffer, 0, Buffer.Length);
            Response.Close();
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new API server
        /// </summary>
        /// <param name="MaxWorkers">The maximum number of concurrent requests this server will allow</param>
        internal ApiServer(int MaxWorkers, string Password = "")
        {
            // Assign variables
            if (!string.IsNullOrEmpty(Password))
            {
                // Set given password
                this.Password = Password;
            }
            else
            {
                // Set to a random 32 byte hex string
                this.Password = ByteArrayToHexString(SecureRandom.Bytes(32));
            }

            // Setup threads and events
            WorkerThreads = new Thread[MaxWorkers];
            ContextQueue = new Queue<HttpListenerContext>();
            StopEvent = new ManualResetEvent(false);
            ReadyEvent = new ManualResetEvent(false);
            ListenerThread = new Thread(Listen);
        }

        #endregion
    }
}
