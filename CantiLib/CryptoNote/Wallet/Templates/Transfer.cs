//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Newtonsoft.Json;

namespace Canti.CryptoNote.Wallet
{
    public class Transfer
    {
        // The amount in this transfer (in atomic units)
        [JsonProperty(PropertyName = "amount")]
        public ulong Amount { get; set; }

        // This transfer's public key
        [JsonProperty(PropertyName = "publicKey")]
        public string PublicKey { get; set; }
    }
}
