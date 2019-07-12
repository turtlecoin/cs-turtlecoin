//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;
using System.Linq;
using Canti.CryptoNote.Blockchain;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    // Handles all blockchain and storage operations
    internal sealed partial class BlockchainCache
    {
        #region Properties and Fields

        #region Internal

        // The current height of the blockchain
        internal uint Height
        {
            get
            {
                // Check if database is started
                if (!Database.Started)
                {
                    return 0;
                }

                // Count how many blocks are in the table
                // TODO - this can be made more efficient by only wuerying for the last block's height
                return (uint)Database.Count(BLOCKS_TABLE);
            }
        }

        // The last known height of the blockchain
        private uint _knownHeight = 0;
        internal uint KnownHeight
        {
            get
            {
                // Check if last known height has not been assigned
                if (_knownHeight < 1) return Height;

                // Otherwise return assigned value
                else return _knownHeight;
            }
            set
            {
                // Cache assigned value
                _knownHeight = value;
            }
        }

        // The hash of the last stored block
        internal string LastBlockHash
        {
            get
            {
                // Try to get the block at the current height
                if (!TryGetBlock(Height, out Block Block)) return Constants.NULL_HASH;

                // Return last block hash
                return Block.Hash;
            }
        }

        // A byte array representation of the last stored block's hash
        internal byte[] TopBlockHash
        {
            get
            {
                return HexStringToByteArray(LastBlockHash);
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Database

        // Creates the blocks table if it doesn't exist
        private void CreateBlocksTable()
        {
            // Create table
            Database.CreateTable(
                // Table name
                "blocks",

                // Table columns
                new ValueList
                {
                    { "height", SqlType.INTEGER, 0, false, true },
                    { "hash", SqlType.CHAR, 64 },
                    { "timestamp", SqlType.TIMESTAMP },
                    { "nonce", SqlType.INTEGER },
                    { "major_version", SqlType.TINYINT },
                    { "minor_version", SqlType.TINYINT },
                    { "base_reward", SqlType.BIGINT },
                    { "total_fees", SqlType.BIGINT },
                    { "base_transaction", SqlType.CHAR, 64 },
                    { "already_generated_units", SqlType.BIGINT }
                }
            );
        }

        // Helper for easier deserialization, so this doesn't need re-typed
        private Block DeserializeBlockRow(ValueList Row)
        {
            Block Block = new Block(this)
            {
                Height = (uint)Row["height"],
                Hash = (string)Row["hash"],
                Timestamp = (ulong)Row["timestamp"],
                Nonce = (uint)Row["nonce"],
                MajorVersion = (byte)Row["major_version"],
                MinorVersion = (byte)Row["minor_version"],
                BaseReward = (ulong)Row["base_reward"],
                TotalFees = (ulong)Row["total_fees"],
                AlreadyGeneratedUnits = (ulong)Row["already_generated_units"]
            };

            TryGetTransaction((string)Row["base_transaction"], out Transaction BaseTransaction);
            Block.SetBaseTransaction(BaseTransaction);

            return Block;
        }

        // Gets a block hash by height
        internal string GetBlockHash(uint Height)
        {
            // Check if database is started
            if (!Database.Started)
            {
                return Constants.NULL_HASH;
            }

            // Query the database
            var Result = Database.Select(BLOCKS_TABLE, new ValueList
            {
                ["height"] = Height
            });

            // If there are no matching entries, return a null hash
            if (!Result.Any())
            {
                return Constants.NULL_HASH;
            }

            // Return the found block hash
            return Result.First()["hash"];
        }

        // Gets a block from the database by height
        internal bool TryGetBlock(uint Height, out Block Block)
        {
            // Check if database is started
            if (!Database.Started)
            {
                Block = new Block(this)
                {
                    Hash = Constants.NULL_HASH
                };
                return false;
            }

            // Query the database
            var Result = Database.Select(BLOCKS_TABLE, new ValueList
            {
                ["height"] = Height
            });

            // If there are no matching entries, return false along with an empty block
            if (!Result.Any())
            {
                Block = new Block(this)
                {
                    Hash = Constants.NULL_HASH
                };
                return false;
            }

            // Serialize found block from first row of result
            Block = DeserializeBlockRow(Result.First());
            return true;
        }

        // Gets a block from the database by hash
        internal bool TryGetBlock(string Hash, out Block Block)
        {
            // Check if database is started
            if (!Database.Started)
            {
                Block = new Block(this)
                {
                    Hash = Constants.NULL_HASH
                };
                return false;
            }

            // Query the database
            var Result = Database.Select(BLOCKS_TABLE, new ValueList
            {
                ["hash"] = Hash
            });

            // If there are no matching entries, return false along with an empty block
            if (!Result.Any())
            {
                Block = new Block(this)
                {
                    Hash = Constants.NULL_HASH
                };
                return false;
            }

            // Serialize found block from first row of result
            Block = DeserializeBlockRow(Result.First());
            return true;
        }

        // Stores a block in the database at the specified height
        internal bool StoreBlock(Block Block)
        {
            // Check if database is started
            if (!Database.Started) return false;

            // Calculate already generated units
            // TODO - use reward not base reward
            ulong AlreadyGeneratedUnits = GetAlreadyGeneratedUnits() + Block.BaseReward;

            // Store block
            Database.Add(BLOCKS_TABLE, new ValueList
            {
                ["hash"] = Block.Hash,
                ["timestamp"] = Block.Timestamp,
                ["nonce"] = Block.Nonce,
                ["major_version"] = Block.MajorVersion,
                ["minor_version"] = Block.MinorVersion,
                ["base_reward"] = Block.BaseReward,
                ["total_fees"] = Block.TotalFees,
                ["base_transaction"] = Block.BaseTransaction.Hash,
                ["already_generated_units"] = AlreadyGeneratedUnits
            });

            // Store transactions in this block
            StoreTransaction(Block, Block.BaseTransaction);
            foreach (Transaction Transaction in Block.Transactions)
            {
                StoreTransaction(Block, Transaction);
            }

            // Completed
            return true;
        }

        // Checks if we have a block with this hash stored
        internal bool IsBlockStored(string Hash)
        {
            // Check if database is started
            if (!Database.Started) return false;

            // Count the instances of this block hash
            var Count = Database.Count(BLOCKS_TABLE, new ValueList
            {
                ["hash"] = Hash
            });
            return Count > 0;
        }

        #endregion

        #region Utilities

        // Gets how many generated units the last block had
        internal ulong GetAlreadyGeneratedUnits()
        {
            // Query the database
            var Result = Database.Select(BLOCKS_TABLE, new ValueList());

            // If there are no matching entries, return 0
            if (!Result.Any()) return 0;

            // Otherwise return the last known amount
            else return Result.Last()["already_generated_units"];
        }

        // Gets or generates a network genesis block
        internal void CacheGenesisBlock()
        {
            // First, attempt to get the genesis block from the database
            if (!TryGetBlock(1, out Block Genesis))
            {
                // Genesis block wasn't found, generate it
                Logger?.Warning($"Genesis block was not found, generating a new one...");

                // If the genesis block is not found, create a new one
                Genesis = new Block(this)
                {
                    MajorVersion = Constants.BLOCK_MAJOR_VERSION_1,
                    MinorVersion = Constants.BLOCK_MINOR_VERSION,
                    Timestamp = 0,
                    Nonce = 70
                };

                // Set base transaction
                Genesis.SetBaseTransaction(new Transaction(Globals.CURRENCY_GENESIS_TRANSACTION));

                // Store this block in the database
                StoreBlock(Genesis);
            }
            Logger?.Debug($"Genesis block found: {Genesis.Hash}");

            // Assign cached genesis block to this block
            this.Genesis = Genesis;
        }

        // Builds a list of block hashes based on sparse chain data
        internal string[] BuildSparseChain(uint BlockIndex = 0)
        {
            // TODO - In core source, there is a comment listing the following:
            // IDs of the first 10 blocks are sequential, next goes with pow(2,n) offset,
            // like 2, 4, 8, 16, 32, 64 and so on, and the last one is always genesis block
            // ...
            // BUT, in testing, it's more the following:
            // Last hash is always the genesis hash, but the previous hashes follow the
            // scheme listed above... As in, there are no sequential first 10 blocks. :t_shrug:

            // Create a list of strings to add to for our output
            List<string> Hashes = new List<string>();

            // Use synced height if not specified, this is the starting index
            if (BlockIndex < 1) BlockIndex = Height;

            // Starting at index of 1 (one after genesis), double until we reach the starting index
            for (uint Height = 1; Height < BlockIndex; Height *= 2)
            {
                // Get info for the block at this height (height is index + 1)
                if (!TryGetBlock(Height + 1, out Block Block)) break;

                // Add the block hash for this height
                Hashes.Add(Block.Hash);
            }

            // Add genesis block hash
            // TODO - Store genesis info we need for sync on first call, since it doesn't change
            TryGetBlock(1, out Block Genesis);
            Hashes.Add(Genesis.Hash);

            // Return the list as a string array
            return Hashes.ToArray();
        }

        #endregion

        #endregion
    }
}
