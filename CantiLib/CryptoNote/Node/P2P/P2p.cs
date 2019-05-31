//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Threading;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        #region Properties and Fields

        #region Public

        /// <summary>
        /// The port this node's P2P server will bind to
        /// </summary>
        public int P2pPort { get; set; }

        #endregion

        #region Private

        // This node's P2P server
        private P2pServer P2pServer { get; set; }

        #endregion

        #endregion

        #region Methods

        // Starts all P2P operations
        private bool StartP2p()
        {
            // Assign P2P callbacks
            AssignCallbacks();
            Logger.Debug("P2P callbacks assigned");

            // Start peer list
            StartPeerList();
            Logger.Debug("Peer list started");

            // Set to default port if not specified
            if (P2pPort == 0) P2pPort = Globals.P2P_DEFAULT_PORT;

            // Start the P2P server
            try
            {
                P2pServer.Start(P2pPort, Globals.P2P_POLLING_INTERVAL);
            }
            catch (Exception e)
            {
                Logger.Error($"Could not start P2P server: {e.Message}");
                return false;
            }
            Logger.Debug("P2P server started");

            // Add seed nodes
            AddSeedNodes();
            Logger.Debug("Seed nodes added");

            // Start peer discovery
            StartDiscovery();
            Logger.Debug("Peer discovery started");

            // P2P started
            return true;
        }

        // Stops all P2P operations
        private void StopP2p()
        {
            // Stop peer discovery
            StopDiscovery();
            Logger.Debug("Peer discovery stopped");

            // Disposes of the peer list
            StopPeerList();
            Logger.Debug("Peer list stopped");

            // Stops the P2P server
            P2pServer.Stop();
            Logger.Debug("P2p server stopped");
        }

        // Adds seed nodes
        private void AddSeedNodes()
        {
            // Start connections on a new thread to prevent blocking of main thread
            new Thread(delegate ()
            {
                // Iterate through all seed nodes
                foreach (PeerCandidate Seed in Globals.SEED_NODES)
                {
                    // Check if we should stop looking
                    if (Stopped) return;

                    // Start connections on a new thread to prevent blocking of main thread
                    AddPeer(Seed.Address, (int)Seed.Port);
                }

                // Stop node if no seeds were added
                if (PeerList.Count == 0)
                {
                    Logger.Warning("No seed nodes could be added, all connection attempts failed");
                }
            }).Start();
        }

        #endregion
    }
}
