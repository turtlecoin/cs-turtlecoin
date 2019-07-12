//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Text;

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

        #region Wallet

        // Wallet file version
        internal const int WALLET_FILE_VERSION = 0;

        // Wallet file type identifier
        internal static readonly byte[] WALLET_FILE_IDENTIFIER =
        {
            0x49, 0x66, 0x20, 0x49, 0x20, 0x70, 0x75, 0x6c, 0x6c, 0x20, 0x74,
            0x68, 0x61, 0x74, 0x20, 0x6f, 0x66, 0x66, 0x2c, 0x20, 0x77, 0x69,
            0x6c, 0x6c, 0x20, 0x79, 0x6f, 0x75, 0x20, 0x64, 0x69, 0x65, 0x3f,
            0x0a, 0x49, 0x74, 0x20, 0x77, 0x6f, 0x75, 0x6c, 0x64, 0x20, 0x62,
            0x65, 0x20, 0x65, 0x78, 0x74, 0x72, 0x65, 0x6d, 0x65, 0x6c, 0x79,
            0x20, 0x70, 0x61, 0x69, 0x6e, 0x66, 0x75, 0x6c, 0x2e
        };

        // Wallet file correct password identifier
        internal static readonly byte[] WALLET_PASSWORD_IDENTIFIER =
        {
            0x59, 0x6f, 0x75, 0x27, 0x72, 0x65, 0x20, 0x61, 0x20, 0x62, 0x69,
            0x67, 0x20, 0x67, 0x75, 0x79, 0x2e, 0x0a, 0x46, 0x6f, 0x72, 0x20,
            0x79, 0x6f, 0x75, 0x2e
        };

        // Wallet file encoding method
        internal static readonly Encoding WALLET_ENCODING_METHOD = Encoding.UTF8;

        #endregion
    }
}
