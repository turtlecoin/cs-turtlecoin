//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Newtonsoft.Json;

namespace Canti.CryptoNote.Wallet
{
    public class UnconfirmedAmount
    {
        // The value of this incoming amount
        [JsonProperty(PropertyName = "amount")]
        public ulong Amount { get; set; }

        // The key the corresponding input has. This can be used to remove this entry when the full input gets confirmed
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        // The transaction hash that contains this input
        [JsonProperty(PropertyName = "parentTransactionHash")]
        public string ParentTransactionHash { get; set; }
    }
}
