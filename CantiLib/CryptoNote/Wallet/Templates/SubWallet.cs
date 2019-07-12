//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Cryptography;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Canti.CryptoNote.Wallet
{
    public class SubWallet
    {
        // 64 char hex string that represents this subwallets private spend key (Will be all zeros if view only wallet)
        [JsonProperty(PropertyName = "privateSpendKey")]
        public string PrivateSpendKey { get; set; }

        // 64 char hex string that represents this subwallets public spend key (Duplicated in publicSpendKeys above)
        [JsonProperty(PropertyName = "publicSpendKey")]
        public string PublicSpendKey { get; set; }

        // Private view key derived from private spend key
        private string _privateViewKey;
        [JsonIgnore]
        public string PrivateViewKey
        {
            internal set
            {
                _privateViewKey = value;
            }
            get
            {
                if (string.IsNullOrEmpty(_privateViewKey))
                {
                    if (string.IsNullOrEmpty(PrivateSpendKey) || PrivateSpendKey == Constants.NULL_HASH)
                    {
                        return Constants.NULL_HASH;
                    }

                    else
                    {
                        KeyPair ViewKeys = Crypto.GenerateViewKeysFromPrivateSpendKey(PrivateSpendKey);
                        _privateViewKey = ViewKeys.PrivateKey;
                        _publicViewKey = ViewKeys.PublicKey;
                    }
                }

                return _privateViewKey;
            }
        }

        // Public view key derived from private spend key
        private string _publicViewKey;
        [JsonIgnore]
        public string PublicViewKey
        {
            internal set
            {
                _publicViewKey = value;
            }
            get
            {
                if (string.IsNullOrEmpty(_publicViewKey))
                {
                    if (string.IsNullOrEmpty(PrivateSpendKey) || PrivateSpendKey == Constants.NULL_HASH)
                    {
                        return Constants.NULL_HASH;
                    }

                    else
                    {
                        KeyPair ViewKeys = Crypto.GenerateViewKeysFromPrivateSpendKey(PrivateSpendKey);
                        _privateViewKey = ViewKeys.PrivateKey;
                        _publicViewKey = ViewKeys.PublicKey;
                    }
                }

                return _publicViewKey;
            }
        }

        // This subwallet's address
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        // Is this subwallet's address the wallet's primary address
        [JsonProperty(PropertyName = "isPrimaryAddress")]
        public bool IsPrimaryAddress { get; set; }

        // The height to begin requesting blocks from. Ignored if syncStartTimestamp != 0
        [JsonProperty(PropertyName = "syncStartHeight")]
        public int SyncHeight { get; set; }

        // The timestamp to begin request blocks from. Ignored if syncStartHeight != 0
        [JsonProperty(PropertyName = "syncStartTimestamp")]
        public ulong SyncTimestamp { get; set; }

        // Inputs which have been spent in an outgoing transaction, but not added to a block yet
        [JsonProperty(PropertyName = "lockedInputs")]
        public List<Input> LockedInputs { get; set; }

        // Inputs which have been spent in an outgoing transaction
        [JsonProperty(PropertyName = "spentInputs")]
        public List<Input> SpentInputs { get; set; }

        // Inputs which have not been spent
        [JsonProperty(PropertyName = "unspentInputs")]
        public List<Input> UnspentInputs { get; set; }

        // The amounts and keys of incoming amounts, these are transactions we have sent that come back as change
        [JsonProperty(PropertyName = "unconfirmedIncomingAmounts")]
        public List<UnconfirmedAmount> UnconfirmedAmounts { get; set; }

        // Constructor
        public SubWallet()
        {
            LockedInputs = new List<Input>();
            SpentInputs = new List<Input>();
            UnspentInputs = new List<Input>();
            UnconfirmedAmounts = new List<UnconfirmedAmount>();
        }
    }
}
