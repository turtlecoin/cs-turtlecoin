//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Canti.CryptoNote.Wallet
{
    public class TransactionSyncState
    {
        // Block hash checkpoints taken by default every 5k blocks, useful for if a very deep fork occurs
        [JsonProperty(PropertyName = "blockHashCheckpoints")]
        public List<string> BlockHashCheckpoints { get; set; }

        // Block hash checkpoints of the last (up to) 100 blocks, the first hashes are the newer ones
        [JsonProperty(PropertyName = "lastKnownBlockHashes")]
        public List<string> LastKnownBlockHashes { get; set; }

        // The last block we scanned
        [JsonProperty(PropertyName = "lastKnownBlockHeight")]
        public int LastKnownBlockHeight { get; set; }

        // Constructor
        public TransactionSyncState()
        {
            BlockHashCheckpoints = new List<string>();
            LastKnownBlockHashes = new List<string>();
            LastKnownBlockHeight = 1;
        }
    }
}
