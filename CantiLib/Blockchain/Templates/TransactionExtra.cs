//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace CantiLib.Blockchain.Templates
{
    public struct TransactionExtra
    {
        public string PublicKey { get; set; }
        public byte[] Nonce { get; set; }
        public byte[] RawData { get; set; }
    }
}
