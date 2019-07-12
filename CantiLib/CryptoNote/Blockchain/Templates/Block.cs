//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;
using Canti.Cryptography;

namespace Canti.CryptoNote.Blockchain
{
    // Block template for local storage
    internal class Block
    {
        #region Properties and Fields

        #region Internal

        // Block hash - returns a predefined hash or hashes current data
        private string _hash;
        internal string Hash
        {
            get
            {
                // Check if hash is predefined
                if (!string.IsNullOrEmpty(_hash)) return _hash;

                // Set hash
                SetHash();

                // Return hash
                return _hash;
            }
            set
            {
                // Set pre-defined hash
                _hash = value;
            }
        }

        #region Header

        internal uint MajorVersion { get; set; }
        internal uint MinorVersion { get; set; }
        internal ulong Timestamp { get; set; }
        internal uint Nonce { get; set; }
        internal Transaction BaseTransaction { get; set; }
        internal Transaction[] Transactions { get; set; }

        // Merged mining parent block
        internal Block ParentBlock { get; set; }

        #endregion

        #region Convenience

        internal uint Height { get; set; }
        internal ulong BaseReward { get; set; }
        internal ulong TotalFees { get; set; }
        internal ulong AlreadyGeneratedUnits { get; set; }

        // Previous block info
        private string _previousBlockHash;
        internal string PreviousBlockHash
        {
            get
            {
                if (string.IsNullOrEmpty(_previousBlockHash))
                {
                    _previousBlockHash = Blockchain.GetBlockHash(Height - 1);
                }
                return _previousBlockHash;
            }
        }

        #endregion

        #endregion

        #region Private

        // Reference to blockchain to work with
        private BlockchainCache Blockchain { get; set; }

        #endregion

        #endregion

        #region Methods

        internal void SetHash()
        {
            // Get hashing array
            byte[] HashingArray = GetHashingArray();

            // Check block version
            if (MajorVersion >= Constants.BLOCK_MAJOR_VERSION_2)
            {
                // TODO - Handle block versions 2 and over
            }

            // Create a byte array that will act as our hash seed
            byte[] Buffer = new byte[0];
            Buffer = Buffer.AppendInteger(HashingArray.Length, true);
            Buffer = Buffer.AppendBytes(HashingArray);

            // Store computed hash
            _hash = Crypto.CN_FastHash(Buffer);
        }

        internal string GetTransactionTreeHash()
        {
            // Create a list of hashes, starting with the base transaction hash
            List<string> Hashes = new List<string>
            {
                BaseTransaction.Hash
            };

            // Add all other transaction hashes
            foreach (var Transaction in Transactions)
            {
                Hashes.Add(Transaction.Hash);
            }

            // Return a hash of this list
            return Crypto.TreeHash(Hashes.ToArray());
        }

        internal byte[] GetHashingArray()
        {
            // Get raw block header
            byte[] Buffer = GetHeaderArray();

            // Add transaction tree hash
            Buffer = Buffer.AppendHexString(GetTransactionTreeHash());

            // Add transaction count... shit
            Buffer = Buffer.AppendVarInt(Transactions.Length + 1);

            // Return output buffer
            return Buffer;
        }

        internal byte[] GetHeaderArray()
        {
            // Create a byte array buffer to work on
            byte[] Buffer = new byte[0];

            // Add block versions
            Buffer = Buffer.AppendVarInt(MajorVersion);
            Buffer = Buffer.AppendVarInt(MinorVersion);

            // Add timestamp
            Buffer = Buffer.AppendVarInt(Timestamp);

            // Add previous block hash
            Buffer = Buffer.AppendHexString(PreviousBlockHash);

            // Add nonce
            Buffer = Buffer.AppendInteger(Nonce);

            // Return resulting array
            return Buffer;
        }

        internal byte[] GetRawBlock()
        {
            // Get parent block info (for later)
            ParentBlock = ParentBlock ?? new Block(Blockchain);

            // Create a byte array buffer to work on
            byte[] Buffer = new byte[0];

            // Add block versions
            Buffer = Buffer.AppendVarInt(MajorVersion);
            Buffer = Buffer.AppendVarInt(MinorVersion);

            // Add parent block info
            if (MajorVersion > Constants.MERGED_MINING_BLOCK_VERSION)
            {
                Buffer = Buffer.AppendHexString(PreviousBlockHash);
                Buffer = Buffer.AppendVarInt(ParentBlock.MajorVersion);
                Buffer = Buffer.AppendVarInt(ParentBlock.MinorVersion);
            }

            // Add timestamp
            Buffer = Buffer.AppendVarInt(Timestamp);

            // Add previous block info
            if (MajorVersion >= Constants.MERGED_MINING_BLOCK_VERSION)
            {
                Buffer = Buffer.AppendHexString(ParentBlock.Hash);
            }
            else
            {
                Buffer = Buffer.AppendHexString(PreviousBlockHash);
            }

            // Add nonce
            Buffer = Buffer.AppendInteger(Nonce);

            // Add more parent block information
            if (MajorVersion >= Constants.MERGED_MINING_BLOCK_VERSION)
            {
                Buffer = Buffer.AppendVarInt(ParentBlock.Transactions.Length);
                Buffer = Buffer.AppendVarInt(ParentBlock.MajorVersion);
                Buffer = Buffer.AppendVarInt(ParentBlock.BaseTransaction.UnlockTime);
                Buffer = Buffer.AppendVarInt(ParentBlock.BaseTransaction.Inputs.Length);
                Buffer = Buffer.AppendVarInt(ParentBlock.BaseTransaction.Outputs.Length);
                Buffer = Buffer.AppendVarInt(ParentBlock.BaseTransaction.Extra.Length);
                Buffer = Buffer.AppendBytes(ParentBlock.BaseTransaction.Extra);
            }

            // Add base transaction
            Buffer = Buffer.AppendBytes(BaseTransaction.GetRawTransaction());

            // Add transaction count
            Buffer = Buffer.AppendVarInt(Transactions.Length);

            // Add transaction hashes
            foreach (var Transaction in Transactions)
            {
                Buffer = Buffer.AppendHexString(Transaction.Hash);
            }

            // Serialization complete
            return Buffer;
        }

        internal void SetBaseTransaction(Transaction Transaction)
        {
            BaseTransaction = Transaction;
            BaseReward = Transaction.TotalAmount;
            BaseTransaction.BlockHash = Hash;
        }

        #endregion

        #region Constructors

        // Initializes an empty block
        internal Block(BlockchainCache Blockchain)
        {
            // Set blockchain reference
            this.Blockchain = Blockchain;

            // Setup defaults
            Height = 0;
            Timestamp = 0;
            Nonce = 0;
            MajorVersion = Constants.BLOCK_MAJOR_VERSION_1;
            MinorVersion = Constants.BLOCK_MINOR_VERSION;
            BaseReward = 0;
            TotalFees = 0;
            BaseTransaction = new Transaction();
            Transactions = new Transaction[0];
        }

        // Initializes a block using a base transaction
        internal Block(BlockchainCache Blockchain, string Hex)
        {
            // Set blockchain reference
            this.Blockchain = Blockchain;

            // Setup defaults
            Height = 0;
            Timestamp = 0;
            Nonce = 0;
            MajorVersion = Constants.BLOCK_MAJOR_VERSION_1;
            MinorVersion = Constants.BLOCK_MINOR_VERSION;
            BaseReward = 0;
            TotalFees = 0;
            BaseTransaction = new Transaction();
            Transactions = new Transaction[0];

            // TODO - deserialize from hex

            // TODO - parent block shit
            ParentBlock = new Block(Blockchain);
        }

        #endregion
    }
}
