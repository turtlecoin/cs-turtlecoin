//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Newtonsoft.Json;

namespace Canti.CryptoNote.Wallet
{
    // TODO - Does this really need to be its own class, or pulled from other transactions when serializing??
    public class SentTransaction
    {
        // The hash of the transaction this key belongs to
        [JsonProperty(PropertyName = "transactionHash")]
        public string Hash { get; set; }

        // The private key of this transaction
        [JsonProperty(PropertyName = "txPrivateKey")]
        public string Key { get; set; }
    }
}
