//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain
{
    // Transaction template
    public struct Transaction
    {
        public string Hash { get; set; }
        public ulong Size { get; set; }
        public ulong Fee { get; set; }
        public ulong TotalInputs { get; set; }
        public ulong TotalOutputs { get; set; }
        public ulong Mixin { get; set; }
        public ulong UnlockTime { get; set; }
        public ulong Timestamp { get; set; }
        public string PaymentId { get; set; }
        public bool HasPaymentId { get; set; }
        public bool InBlockchain { get; set; }
        public string BlockHash { get; set; }
        public uint BlockIndex { get; set; }
        // public TransactionExtra Extra { get; set; }
        public string[] Signatures { get; set; }
        public Input[] Inputs { get; set; }
        public Output[] Outputs { get; set; }
    }
}
