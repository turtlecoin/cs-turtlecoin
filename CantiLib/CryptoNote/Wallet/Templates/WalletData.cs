//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Canti.CryptoNote.Wallet
{
    public class WalletContainer
    {
        // Is the wallet a view only wallet or not, view only wallets will have a privateSpendKey of all zeros
        [JsonProperty(PropertyName = "isViewWallet")]
        public bool IsViewWallet { get; set; }

        // Stores transactions that have been sent by the user (outgoing), but have not been added to a block yet
        [JsonProperty(PropertyName = "lockedTransactions")]
        public List<Transaction> LockedTransactions { get; set; }

        // 64 char hex string that represents the shared, private view key
        [JsonProperty(PropertyName = "privateViewKey")]
        public string PrivateViewKey { get; set; }

        // Stores the public spend keys of the subwallets, each are 64 char hex strings
        [JsonProperty(PropertyName = "publicSpendKeys")]
        public List<string> PublicSpendKeys { get; set; }

        // An array of subwallets. Contains keys, transaction inputs, etc
        [JsonProperty(PropertyName = "subWallet")]
        public List<SubWallet> SubWallets { get; set; }

        // Any transactions which are in a block
        [JsonProperty(PropertyName = "transactions")]
        public List<Transaction> Transactions { get; set; }

        // Private keys of transactions sent by this container, and the hash they belong to
        [JsonProperty(PropertyName = "txPrivateKeys")]
        public List<SentTransaction> TransactionKeys { get; set; }

        // Constructor
        public WalletContainer()
        {
            IsViewWallet = false;
            LockedTransactions = new List<Transaction>();
            PrivateViewKey = Constants.NULL_HASH;
            PublicSpendKeys = new List<string>();
            SubWallets = new List<SubWallet>();
            Transactions = new List<Transaction>();
            TransactionKeys = new List<SentTransaction>();
        }
    }
}
