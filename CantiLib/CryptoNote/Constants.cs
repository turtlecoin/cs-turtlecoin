//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.CryptoNote
{
    // Contains a set of global constants
    internal sealed class Constants
    {
        // A 32 byte empty hash string
        internal const string NULL_HASH = "0000000000000000000000000000000000000000000000000000000000000000";

        // Block major versions
        internal const int BLOCK_MAJOR_VERSION_1 = 1;
        internal const int BLOCK_MAJOR_VERSION_2 = 2;
        internal const int BLOCK_MAJOR_VERSION_3 = 3;

        // Block minor version
        internal const int BLOCK_MINOR_VERSION = 0;

        // Transaction version
        internal const int TRANSACTION_VERSION = 1;

        // Merged mining block version
        internal const int MERGED_MINING_BLOCK_VERSION = 2;
    }
}
