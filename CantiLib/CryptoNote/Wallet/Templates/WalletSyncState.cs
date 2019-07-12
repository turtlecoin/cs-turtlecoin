//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Newtonsoft.Json;

namespace Canti.CryptoNote.Wallet
{
    public class WalletSyncState
    {
        // The private view key used to decrypt transactions
        [JsonProperty(PropertyName = "privateViewKey")]
        public string PrivateViewKey { get; set; }

        // The height to begin requesting blocks from. Ignored if syncStartTimestamp != 0
        [JsonProperty(PropertyName = "startHeight")]
        public int StartHeight { get; set; }

        // The timestamp to begin request blocks from. Ignored if syncStartHeight != 0
        [JsonProperty(PropertyName = "startTimestamp")]
        public ulong StartTimestamp { get; set; }

        // The synchronization status of transactions
        [JsonProperty(PropertyName = "transactionSynchronizerStatus")]
        public TransactionSyncState TransactionsSyncState { get; set; }

        // Constructor
        public WalletSyncState()
        {
            PrivateViewKey = Constants.NULL_HASH;
            StartHeight = 0;
            StartTimestamp = 0;
            TransactionsSyncState = new TransactionSyncState();
        }
    }
}
