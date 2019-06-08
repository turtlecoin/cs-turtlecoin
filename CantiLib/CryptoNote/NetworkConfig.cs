//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.CryptoNote
{
    /// <summary>
    /// A configuration class that needs to be specified when initializing a node/blockchain cache
    /// </summary>
    public class NetworkConfig
    {
        #region NETWORK

        /// <summary>
        /// This is the byte array value that uniquely identifies the desired network
        /// </summary>
        public byte[] NETWORK_ID { get; set; }

        /// <summary>
        /// (OPTIONAL) A bit of vanity text that will be shown when a node is first initialized
        /// </summary>
        public string ASCII_ART { get; set; }

        /// <summary>
        /// A list of seed node peers for initial P2P connections
        /// </summary>
        public PeerCandidate[] SEED_NODES { get; set; }

        #endregion

        #region CURRENCY

        /// <summary>
        /// The name of the currency
        /// </summary>
        public string CURRENCY_NAME { get; set; }

        /// <summary>
        /// The currency's difficulty target time (in seconds)
        /// </summary>
        public int CURRENCY_DIFFICULTY_TARGET { get; set; }

        /// <summary>
        /// The total supply units ultimately available
        /// </summary>
        public ulong CURRENCY_TOTAL_SUPPLY { get; set; }

        /// <summary>
        /// The factor used to determine the rate at which rewards are emitted
        /// </summary>
        public int CURRENCY_EMISSION_FACTOR { get; set; }

        /// <summary>
        /// The reward amount of the genesis block (premine)
        /// </summary>
        public ulong CURRENCY_GENESIS_REWARD { get; set; }

        /// <summary>
        /// The hex string representing the genesis transaction
        /// </summary>
        public string CURRENCY_GENESIS_TRANSACTION { get; set; }

        #endregion

        #region STORAGE

        /// <summary>
        /// The type of database the node will utilize
        /// </summary>
        public DatabaseType DATABASE_TYPE { get; set; }

        /// <summary>
        /// (OPTIONAL) Specifies a directory where any database files will be saved
        /// </summary>
        public string LOCAL_STORAGE_DIRECTORY { get; set; }

        /// <summary>
        /// The location of the database
        /// </summary>
        public string DATABASE_LOCATION { get; set; }

        #endregion

        #region P2P

        /// <summary>
        /// The default P2P communications port
        /// </summary>
        public int P2P_DEFAULT_PORT { get; set; }

        /// <summary>
        /// The number of worker threads to listen for new peers on
        /// </summary>
        public int P2P_WORKERS { get; set; }

        /// <summary>
        /// The expected minimum number of peers
        /// </summary>
        public int P2P_MIN_PEER_CONNECTIONS { get; set; }

        /// <summary>
        /// The max number of peer connections to be allowed
        /// </summary>
        public int P2P_MAX_PEER_CONNECTIONS { get; set; }

        /// <summary>
        /// Whether or not the node will automatically search for new peers
        /// </summary>
        public bool P2P_DISCOVERY_ENABLED { get; set; }

        /// <summary>
        /// The current version of the P2P protocol being used
        /// </summary>
        public int P2P_CURRENT_VERSION { get; set; }

        /// <summary>
        /// The minimum P2P protocol version a peer must be using to be considered valid
        /// </summary>
        public int P2P_MINIMUM_VERSION { get; set; }

        /// <summary>
        /// How often peers should try to be discovered from the candidate list (in seconds)
        /// </summary>
        public int P2P_DISCOVERY_INTERVAL { get; set; }

        /// <summary>
        /// How often the list of recently tried peer candidates is emptied (in seconds)
        /// </summary>
        public int P2P_DISCOVERY_TIMEOUT { get; set; }

        /// <summary>
        /// The amount of time to wait for a connection to be made before giving up (in milliseconds)
        /// </summary>
        public int P2P_CONNECTION_TIMEOUT { get; set; }

        /// <summary>
        /// How often to poll peers for disconnects (in milliseconds)
        /// </summary>
        public int P2P_POLLING_INTERVAL { get; set; }

        /// <summary>
        /// How often to send timed sync packets to check where connected peers are at (in seconds)
        /// </summary>
        public int P2P_TIMED_SYNC_INTERVAL { get; set; }

        /// <summary>
        /// The max time difference allowed between a handshaking peer and the local time (in seconds)
        /// </summary>
        public ulong P2P_HANDSHAKE_TIME_DELTA { get; set; }

        /// <summary>
        /// The max time difference allowed between a peer candidate and the local time (in seconds)
        /// </summary>
        public ulong P2P_LAST_SEEN_DELTA { get; set; }

        #endregion

        #region API

        /// <summary>
        /// Whether or not the API server will be started
        /// </summary>
        public bool API_ENABLED { get; set; }

        /// <summary>
        /// The default port to listen for API requests on
        /// </summary>
        public int API_DEFAULT_PORT { get; set; }

        /// <summary>
        /// The password required for API requests that require validation
        /// </summary>
        public string API_PASSWORD { get; set; }

        /// <summary>
        /// The number of worker threads to listen for new requests on
        /// </summary>
        public int API_WORKERS { get; set; }

        /// <summary>
        /// The current API protocol version being used
        /// </summary>
        public int API_CURRENT_VERSION { get; set; }

        /// <summary>
        /// The minimum API protocol version a request must be using
        /// </summary>
        public int API_MINIMUM_VERSION { get; set; }

        #endregion

        #region LOGGING

        /// <summary>
        /// (OPTIONAL) A file where all logger output is also written to
        /// </summary>
        public string LOG_FILE { get; set; }

        /// <summary>
        /// What level of logging will be shown
        /// </summary>
        public LogLevel LOG_LEVEL { get; set; }

        /// <summary>
        /// (OPTIONAL) A custom prefix that will be shown before a label name, if showing prefixes
        /// </summary>
        public string CUSTOM_PREFIX { get; set; }

        /// <summary>
        /// The default color for logger output
        /// </summary>
        public ConsoleColor INFO_COLOR { get; set; }

        /// <summary>
        /// The color important messages will be shown in when logging
        /// </summary>
        public ConsoleColor IMPORTANT_COLOR { get; set; }

        /// <summary>
        /// The color debug messages will be shown in when logging
        /// </summary>
        public ConsoleColor DEBUG_COLOR { get; set; }

        /// <summary>
        /// The color warning messages will be shown in when logging
        /// </summary>
        public ConsoleColor WARNING_COLOR { get; set; }

        /// <summary>
        /// The color error messages will be shown in when logging
        /// </summary>
        public ConsoleColor ERROR_COLOR { get; set; }

        #endregion
    }
}
