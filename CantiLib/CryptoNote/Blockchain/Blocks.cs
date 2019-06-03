//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Linq;

namespace Canti.CryptoNote
{
    // Handles all blockchain and storage operations
    internal sealed partial class BlockchainCache
    {
        #region Methods

        // Helper for easier deserialization, so this doesn't need re-typed
        private Block DeserializeBlockRow(ValueList Row)
        {
            return new Block
            {
                Height = (uint)Row["height"],
                Hash = Row["hash"],
                Size = (ulong)Row["size"],
                Timestamp = (ulong)Row["timestamp"],
                Nonce = (uint)Row["nonce"],
                MajorVersion = (byte)Row["major_version"],
                MinorVersion = (byte)Row["minor_version"],
                BaseReward = (ulong)Row["base_reward"],
                TotalFees = (ulong)Row["total_fees"],
                BaseTransaction = Row["base_transaction"]
            };
        }

        // Gets a block from the database by height
        internal bool TryGetBlock(uint Height, out Block Block)
        {
            // Query the databased
            var Result = Database.Select(BLOCKS_TABLE, new ValueList
            {
                ["height"] = Height
            });

            // If there are no matching entries, return false along with an empty block
            if (!Result.Any())
            {
                Block = new Block();
                return false;
            }

            // Serialize found block from first row of result
            Block = DeserializeBlockRow(Result.First());
            return true;
        }

        // Gets a block from the database by hash
        internal bool TryGetBlock(string Hash, out Block Block)
        {
            // Query the databased
            var Result = Database.Select(BLOCKS_TABLE, new ValueList
            {
                ["hash"] = Hash
            });

            // If there are no matching entries, return false along with an empty block
            if (!Result.Any())
            {
                Block = new Block();
                return false;
            }

            // Serialize found block from first row of result
            Block = DeserializeBlockRow(Result.First());
            return true;
        }

        // Stores a block in the database
        internal void StoreBlock(Block Block)
        {
            Database.Add(BLOCKS_TABLE, new ValueList
            {
                ["height"] = Block.Height,
                ["hash"] = Block.Hash,
                ["size"] = Block.Size,
                ["timestamp"] = Block.Timestamp,
                ["nonce"] = Block.Nonce,
                ["major_version"] = Block.MajorVersion,
                ["minor_version"] = Block.MinorVersion,
                ["base_reward"] = Block.BaseReward,
                ["total_fees"] = Block.TotalFees,
                ["base_transaction"] = Block.BaseTransaction
            });
        }

        // Checks if we have a block with this hash stored
        internal bool IsBlockStored(string Hash)
        {
            var Count = Database.Count(BLOCKS_TABLE, new ValueList
            {
                ["hash"] = Hash
            });
            return Count > 0;
        }

        #endregion
    }
}
