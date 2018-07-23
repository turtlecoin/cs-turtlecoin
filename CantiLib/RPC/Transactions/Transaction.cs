//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.RPC
{
    public class TransactionContainer
    {
        public List<string> Addresses { get; set; }
        public UInt64 Anonymity { get; set; }
        public string ChangeAddress { get; set; }
        public Extra Extra { get; set; }
        public UInt64 Fee { get; set; }
        public string PaymentId { get; set; }
        public List<(string Address, Int64 Amount)> Transfers { get; set; }
        public UInt64 UnlockTime { get; set; }
    }
}
