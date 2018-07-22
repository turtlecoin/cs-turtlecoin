//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Data;
using System;
using System.Collections.Generic;

namespace Canti.Blockchain
{
    class Globals
    {
        // Daemon
        internal static IBlockchainCache[] DAEMON_CHAIN_LEAVES = new IBlockchainCache[] { };
        internal static uint DAEMON_BLOCK_HEIGHT = 1;
        internal static string DAEMON_TOP_ID = Encoding.HexStringToString("0000000000000000000000000000000000000000000000000000000000000000");
        internal static PeerlistEntry[] DAEMON_PEERLIST = new PeerlistEntry[0];

        /* What prefix does your address start with - see https://cryptonotestarter.org/tools.html */
        internal static ulong addressPrefix = 0x3bbb1d; /* == TRTL */
    }
}
