//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Utilities;
using Canti.Data;
using Canti.Blockchain.P2P;
using System;
using System.Collections.Generic;
using Canti.Blockchain;
using Canti.Blockchain.Commands;

namespace Daemon
{
    class Program
    {
        static int Port = Daemon.P2pPort;
        static void Main(string[] args)
        {
            // Parse commandline arguments
            if (args.Length >= 1) Port = int.Parse(args[0]);

            // Create a daemon connection
            Daemon Daemon = new Daemon(Port);

            // Start daemon
            Daemon.Start();
        }
    }
}
