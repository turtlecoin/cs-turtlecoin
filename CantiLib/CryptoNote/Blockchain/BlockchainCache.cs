//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using Canti.CryptoNote.Blockchain;

namespace Canti.CryptoNote
{
    // Handles all blockchain and storage operations
    // TODO - finish blockchain cache
    internal sealed partial class BlockchainCache
    {
        #region Constants

        // Table names
        private const string BLOCKS_TABLE = "blocks";
        private const string TRANSACTIONS_TABLE = "transactions";

        #endregion

        #region Properties and Fields

        #region Internal

        // Logger used to log messages
        internal Logger Logger { get; set; }

        // Holds configuration for everything on the network
        internal NetworkConfig Globals { get; private set; }

        #endregion

        #region Private

        // Contains a cached copy of the first block of the chain
        private Block Genesis { get; set; }

        #endregion

        #endregion

        #region Methods

        // Starts the blockchain handler and checks database connection
        internal void Start(IDatabase Database)
        {
            // Start database
            StartDatabase(Database);

            // Cache genesis block
            CacheGenesisBlock();
        }

        // Stops the blockchain handler and closes the database connection
        internal void Stop()
        {
            // Stop database
            StopDatabase();
        }

        // TODO - Move to a better location - CryptoNoteUtils?
        //        Along with others such as generating a genesis block?
        internal ulong GetBlockBaseRewardByHeight(uint Height)
        {
            // Try to get the previous block
            if (!TryGetBlock(Height - 1, out Block Block))
            {
                throw new InvalidOperationException("Can't find previous block");
            }

            // Calculate reward
            ulong AlreadyGeneratedUnits = Block.AlreadyGeneratedUnits;

            return (Globals.CURRENCY_TOTAL_SUPPLY - AlreadyGeneratedUnits) >> Globals.CURRENCY_EMISSION_FACTOR;
        }

        #endregion

        #region Constructors

        // Initializes a new blockchain storage
        internal BlockchainCache(NetworkConfig Config)
        {
            // Assign network config
            Globals = Config;
        }

        #endregion
    }
}
