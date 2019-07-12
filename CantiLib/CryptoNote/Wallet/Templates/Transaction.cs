//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Canti.CryptoNote.Wallet
{
    public class Transaction
    {
        // The block height this transaction was included in
        [JsonProperty(PropertyName = "blockHeight")]
        public int Height { get; set; }

        // The fee used on this transaction (in atomic units)
        [JsonProperty(PropertyName = "fee")]
        public ulong Fee { get; set; }

        // The hash of this transaction
        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        // Is this transaction a 'coinbase'/miner reward transaction
        [JsonProperty(PropertyName = "isCoinbaseTransaction")]
        public bool IsCoinbase { get; set; }

        // The payment ID used in this transaction (may be the empty string)
        [JsonProperty(PropertyName = "paymentID")]
        public string PaymentId { get; set; }

        // The timestamp of the block this transaction was included in (unix style)
        [JsonProperty(PropertyName = "timestamp")]
        public ulong Timestamp { get; set; }

        // The amounts and destinations of the transaction.
        // Amounts can be positive and negative if sending from one container address to another
        [JsonProperty(PropertyName = "transfers")]
        public List<Transfer> Transfers { get; set; }

        // The time this transaction unlocks at. If >= 500000000, treated as a timestamp
        // Else, treated as a block height. Cannot be spent till unlocked
        [JsonProperty(PropertyName = "unlockTime")]
        public ulong UnlockTime { get; set; }
    }
}
