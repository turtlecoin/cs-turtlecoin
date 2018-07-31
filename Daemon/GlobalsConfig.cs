//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Utilities;

namespace Daemon
{
    // Global daemon config
    partial class Daemon
    {
        // Default p2p port
        public static int P2pPort = 8091;

        // Default rpc port
        public static int RpcPort = 8092;

        // Default log level
        public static Level LogLevel = Level.INFO;

        // Default log file (null = no log file)
        public static string LogFile = null;
    }
}
