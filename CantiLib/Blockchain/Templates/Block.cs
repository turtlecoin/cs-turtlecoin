//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain
{
    // Block template
    public struct Block
    {
        // Block header
        public byte MajorVersion { get; set; }
        public byte MinorVersion { get; set; }
        public ulong Timestamp { get; set; }
        public string PreviousBlockHash { get; set; }
        public uint Nonce { get; set; }

        // Block data
        public bool IsAlternative { get; set; }
        public uint Height { get; set; }
        public string Hash { get; set; }
        public ulong Difficulty { get; set; }
        public ulong Reward { get; set; }
        public ulong BaseReward { get; set; }
        public ulong BlockSize { get; set; }
        public ulong TransactionCumulativeSize { get; set; }
        public ulong AlreadyGeneratedCoins { get; set; }
        public ulong AlreadyGeneratedTransactions { get; set; }
        public ulong SizeMedian { get; set; }
        public double Penalty { get; set; }
        public ulong TotalFeeAmount { get; set; }
        public Transaction[] Transactions { get; set; }
    }
}
