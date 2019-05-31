//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti;
using Canti.CryptoNote;
using System;
using static Canti.Utils;

namespace CSTurtleCoin
{
    partial class Daemon
    {
        // This is the network configuration we will pass to our node instance when creating it
        private static readonly NodeConfig Configuration = new NodeConfig
        {
            #region Network

            NETWORK_ID = new byte[] {
                0xb5, 0x0c, 0x4a, 0x6c, 0xcf, 0x52, 0x57, 0x41,
                0x65, 0xf9, 0x91, 0xa4, 0xb6, 0xc1, 0x43, 0xe9
            },

            ASCII_ART = "" +
            "\n                                                                            \n" +
            " █████╗ █████╗        ████████╗██╗  ██╗██████╗ ████████╗██╗    ██████╗ █████╗ █████╗ ██╗███╗   ██╗\n" +
            "██╔═══╝██╔═══╝        ╚══██╔══╝██║  ██║██╔══██╗╚══██╔══╝██║    ██╔═══╝██╔═══╝██╔══██╗██║████╗  ██║\n" +
            "██║    ██████╗ █████╗    ██║   ██║  ██║██████╔╝   ██║   ██║    ████╗  ██║    ██║  ██║██║██╔██╗ ██║\n" +
            "██║        ██║ ╚════╝    ██║   ██║  ██║██╔══██╗   ██║   ██║    ██╔═╝  ██║    ██║  ██║██║██║╚██╗██║\n" +
            "╚█████╗█████╔╝           ██║   ╚█████╔╝██║  ██║   ██║   ██████╗██████╗╚█████╗╚█████╔╝██║██║ ╚████║\n" +
            " ╚════╝╚════╝            ╚═╝    ╚════╝ ╚═╝  ╚═╝   ╚═╝   ╚═════╝╚═════╝ ╚════╝ ╚════╝ ╚═╝╚═╝  ╚═══╝\n",

            SEED_NODES = new PeerCandidate[]
            {
                new PeerCandidate("206.189.142.142", 11897),
                new PeerCandidate("145.239.88.119", 11999),
                new PeerCandidate("142.44.242.106", 11897),
                new PeerCandidate("165.227.252.132", 11897)
            },

            #endregion

            #region STORAGE

            DATABASE_TYPE = DatabaseType.SQLITE,
            LOCAL_STORAGE_DIRECTORY = GetAppDataPath("CS-TurtleCoin"),
            DATABASE_LOCATION = "cs-turtlecoin.db",

            #endregion

            #region P2P

            P2P_DEFAULT_PORT = 8090,
            P2P_WORKERS = 4,
            P2P_CURRENT_VERSION = 5,
            P2P_MINIMUM_VERSION = 4,
            P2P_MIN_PEER_CONNECTIONS = 4,
            P2P_MAX_PEER_CONNECTIONS = 50,
            P2P_DISCOVERY_ENABLED = true,
            P2P_DISCOVERY_INTERVAL = 5,
            P2P_CONNECTION_TIMEOUT = 2000,
            P2P_POLLING_INTERVAL = 100,
            P2P_TIMED_SYNC_INTERVAL = 60,
            P2P_HANDSHAKE_TIME_DELTA = 2,
            P2P_LAST_SEEN_DELTA = 600, // TODO - <-- Possibly remove

            #endregion

            #region API

            API_ENABLED = true,
            API_PASSWORD = "12345",
            API_DEFAULT_PORT = 8091,
            API_WORKERS = 5,
            API_CURRENT_VERSION = 1,
            API_MINIMUM_VERSION = 0,

            #endregion

            #region Logger

            LOG_FILE = "CS-TurtleCoin.log",
            LOG_LEVEL = LogLevel.MAX,
            INFO_COLOR = ConsoleColor.White,
            IMPORTANT_COLOR = ConsoleColor.Green,
            DEBUG_COLOR = ConsoleColor.DarkGray,
            WARNING_COLOR = ConsoleColor.Yellow,
            ERROR_COLOR = ConsoleColor.Red,

            #endregion
        };
    }
}
