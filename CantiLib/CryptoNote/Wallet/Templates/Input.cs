//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Newtonsoft.Json;

namespace Canti.CryptoNote.Wallet
{
    public class Input
    {
        // The value of this input, in atomic units
        [JsonProperty(PropertyName = "amount")]
        public ulong Amount { get; set; }

        // The block height this input was received at
        [JsonProperty(PropertyName = "blockHeight")]
        public int Height { get; set; }

        // The index of this input in the global database
        [JsonProperty(PropertyName = "globalOutputIndex")]
        public int GlobalOutputIndex { get; set; }

        // The key of this input
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        // The key image of this input
        [JsonProperty(PropertyName = "keyImage")]
        public string KeyImage { get; set; }

        // The transaction hash this input was received in
        [JsonProperty(PropertyName = "parentTransactionHash")]
        public string ParentTransactionHash { get; set; }

        // The height this input was spent at (0 if unspent)
        [JsonProperty(PropertyName = "spendHeight")]
        public int SpendHeight { get; set; }

        // The index of this input in the transaction it was received in
        [JsonProperty(PropertyName = "transactionIndex")]
        public int TransactionIndex { get; set; }

        // The public key of the transaction this input was received in
        [JsonProperty(PropertyName = "transactionPublicKey")]
        public string TransactionPublicKey { get; set; }

        // The time this input unlocks at. If >= 500000000, treated as a timestamp
        // Else, treated as a block height. Cannot be spent till unlocked
        [JsonProperty(PropertyName = "unlockTime")]
        public int UnlockTime { get; set; }
    }
}
