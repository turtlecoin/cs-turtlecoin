//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;
using System.IO;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    /// <summary>
    /// A CryptoNote network node
    /// </summary>
    public sealed partial class Node : INode
    {
        #region Properties and Fields

        #region Public

        /// <summary>
        /// A unique identifier for this node
        /// </summary>
        public ulong Id { get; private set; }

        #endregion

        #region Internal

        // The logger object this node will use to output any information
        internal Logger Logger { get; private set; }

        // Holds configuration for everything on the network
        internal NetworkConfig Globals { get; private set; }

        // Whether the node has been stopped
        internal bool Stopped { get; private set; }

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Starts this node and any associated threads
        /// </summary>
        /// <returns>True if started successfully</returns>
        public bool Start()
        {
            // Check if we are stopped
            if (!Stopped) return false;

            // Set as running
            Stopped = false;

            // Start logger
            Logger.Start();

            // Show ascii art
            if (!string.IsNullOrEmpty(Globals.ASCII_ART))
            {
                Logger.ShowPrefix = false;
                Logger.Important(Globals.ASCII_ART);
                Logger.ShowPrefix = true;
            }

            // Create local storage directory if it doesn't exist
            if (!Directory.Exists(Globals.LOCAL_STORAGE_DIRECTORY))
            {
                Logger.Debug($"Creating directory {Globals.LOCAL_STORAGE_DIRECTORY}");
                Directory.CreateDirectory(Globals.LOCAL_STORAGE_DIRECTORY);
            }

            // Start blockchain cache
            if (!StartBlockchainCache()) return false;
            Logger.WriteLine($"Blockchain cache loaded: {DatabaseLocation}");

            // Start P2P server
            if (!StartP2p()) return false;
            Logger.WriteLine($"P2P server started on port {P2pServer.Port}");

            // Start API server
            if (Globals.API_ENABLED && !StartApi()) return false;
            Logger.WriteLine($"API server started on port {ApiServer.Port}");

            // Start syncing
            StartSync();
            Logger.WriteLine("Started syncing");

            // Node started
            Logger.Important($"Node started with ID {Id}");
            return true;
        }

        /// <summary>
        /// Stops this node and waits for all associated threads to exit
        /// </summary>
        public void Stop()
        {
            // Check that we aren't stopped already
            if (Stopped) return;

            // Set as stopped
            Stopped = true;
            Logger.Debug("Set stop call");

            // Stop sync
            StopSync();
            Logger.WriteLine("Syncing stopped");

            // Stop API server
            if (Globals.API_ENABLED) StopApi();
            Logger.WriteLine("Stopped API server");

            // Stop P2P server
            StopP2p();
            Logger.WriteLine("Stopped P2P server");

            // Stop blockchain cache
            StopBlockchainCache();
            Logger.WriteLine("Stopped blockchain cache");

            // Write that we're finishing up as we wait for child threads to finish
            Logger.Important("Finishing up...");

            // Stop logger
            Logger.Stop();
        }

        /// <summary>
        /// Adds a peer to the connection queue to be accepted when space is available
        /// </summary>
        /// <param name="Address">The remote peer's host address</param>
        /// <param name="Port">The remote peer's listening port</param>
        /// <returns>True if connection was made</returns>
        public bool AddPeer(string Address, int Port)
        {
            // Add peer to server
            return P2pServer.AddPeer(Address, Port);
        }

        #endregion

        #region Private

        // Serializes node data into a dictionary for packets
        private Dictionary<string, dynamic> GetNodeData()
        {
            return new Dictionary<string, dynamic>
            {
                ["network_id"] = Globals.NETWORK_ID,
                ["version"] = Globals.P2P_CURRENT_VERSION,
                ["peer_id"] = Id,
                ["local_time"] = GetTimestamp(),
                ["my_port"] = P2pPort
            };
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this node with the specified network configuration
        /// </summary>
        /// <param name="Configuration">A class containing all global information this node needs to operate</param>
        public Node(NetworkConfig Configuration)
        {
            // Assign configuration
            Globals = Configuration;

            // Generate identifier
            Id = SecureRandom.Integer<ulong>();

            // Setup and start logger
            Logger = new Logger()
            {
                LogFile = Globals.LOG_FILE,
                LogLevel = Globals.LOG_LEVEL,
                CustomPrefix = Globals.CUSTOM_PREFIX,
                ImportantColor = Globals.IMPORTANT_COLOR,
                InfoColor = Globals.INFO_COLOR,
                ErrorColor = Globals.ERROR_COLOR,
                WarningColor = Globals.WARNING_COLOR,
                DebugColor = Globals.DEBUG_COLOR
            };

            // Setup blockchain cache
            Blockchain = new BlockchainCache(Globals)
            {
                Logger = Logger
            };

            // Create our P2P server
            P2pServer = new P2pServer(Globals.P2P_WORKERS, Globals.P2P_MAX_PEER_CONNECTIONS)
            {
                ConnectionTimeout = Globals.P2P_CONNECTION_TIMEOUT
            };

            // Setup our API server
            ApiServer = new ApiServer(Globals.API_WORKERS, Globals.API_PASSWORD)
            {
                Logger = Logger
            };

            // Setup done, set node to stopped
            Stopped = true;
        }

        #endregion
    }
}
