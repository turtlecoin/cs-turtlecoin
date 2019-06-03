//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.CryptoNote
{
    // Block template for local storage
    internal class Block
    {
        internal uint Height { get; set; }
        internal string Hash { get; set; }
        internal ulong Size { get; set; }
        internal ulong Timestamp { get; set; }
        internal uint Nonce { get; set; }
        internal byte MajorVersion { get; set; }
        internal byte MinorVersion { get; set; }
        internal ulong BaseReward { get; set; }
        internal ulong TotalFees { get; set; }
        internal string BaseTransaction { get; set; }

        // Initializes an empty block
        internal Block()
        {
            Height = 0;
            Hash = Constants.NULL_HASH;
            Size = 0;
            Timestamp = 0;
            Nonce = 0;
            MajorVersion = 0;
            MinorVersion = 0;
            BaseReward = 0;
            TotalFees = 0;
            BaseTransaction = Constants.NULL_HASH;
        }
    }
}
