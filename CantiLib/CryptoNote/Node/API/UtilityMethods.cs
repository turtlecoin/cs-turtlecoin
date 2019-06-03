//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Canti.CryptoNote
{
    // API method context for a node's API server
    internal sealed class UtilityMethods : IMethodContext
    {
        #region Properties and Fields

        #region Private

        // A reference to the associated node
        private Node Node { get; set; }

        #endregion

        #endregion

        #region Methods

        // Verifies a request's version
        public bool CheckVersion(int Version)
        {
            // Returns true if version is at least the minimum version, but not higher than the current version
            return Version >= Node.Globals.API_MINIMUM_VERSION && Version <= Node.Globals.API_CURRENT_VERSION;
        }

        #endregion

        #region API Methods

        // TODO - these are all test methods
        [ApiMethod("PeerList", RequiresValidation = true)]
        public string GetPeerList()
        {
            lock (Node.PeerList)
            {
                // Write peer list to array
                var PeerList = new JArray();
                foreach (var Peer in Node.PeerList)
                {
                    var PeerListEntry = new JObject
                    {
                        ["incoming"] = Peer.P2pPeer.Direction == PeerDirection.IN,
                        ["last_seen"] = DateTimeOffset.FromUnixTimeSeconds((long)Peer.LastSeen).ToString("dd/MM/yy hh:mm:ss"),
                        ["height"]= Peer.Height,
                        ["validated"] = Peer.Validated,
                    };
                    if (Peer.Validated)
                    {
                        PeerListEntry["id"] = Peer.Id;
                    }
                    PeerList.Add(PeerListEntry);
                }

                // Form response
                var Response = new JObject
                {
                    ["peers"] = PeerList
                };
                return Response.ToString();
            }
        }

        [ApiMethod("Diagnostics", RequiresValidation = true)]
        public string GetDiagnostics()
        {
            // Get current process
            var CurrentProcess = Process.GetCurrentProcess();

            // Form response
            var Response = new JObject
            {
                ["local_time"] = DateTime.Now,
                ["memory_usage"] = CurrentProcess.WorkingSet64,
                ["peak_memory_usage"] = CurrentProcess.PeakWorkingSet64
            };
            return Response.ToString();
        }

        // Example of a POST-only method
        [ApiMethod("Test", Type = ApiRequestType.POST)]
        public string PostTest(string Hi, string Lo)
        {
            // Form response
            var Response = new JObject
            {
                ["hi"] = Hi,
                ["lo"] = Lo
            };
            return Response.ToString();
        }

        #endregion

        #region Constructors

        // Initializes this API method context
        internal UtilityMethods(Node Node)
        {
            this.Node = Node;
        }

        #endregion
    }
}
