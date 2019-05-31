//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using static Canti.Utils;

namespace Canti.CryptoNote
{
    // Handles all blockchain and storage operations
    internal sealed partial class BlockchainCache
    {
        #region Properties and Fields

        #region Internal

        // Logger used to log messages
        internal Logger Logger { get; set; }

        // The current height of the blockchain
        internal int Height { get; set; }

        // The hash of the last stored block
        internal string LastHash { get; set; }

        // A byte array representation of the last stored block's hash
        internal byte[] TopId
        {
            get
            {
                return HexStringToByteArray(LastHash);
            }
        }

        #endregion

        #endregion

        #region Methods

        // Starts the blockchain handler and checks database connection
        internal void Start(IDatabase Database)
        {
            // Start database
            StartDatabase(Database);

            // TODO - all this
        }

        // Stops the blockchain handler and closes the database connection
        internal void Stop()
        {
            // Stop database
            StopDatabase();

            // TODO
        }

        #endregion

        #region Constructors

        // Initializes a new blockchain storage
        internal BlockchainCache()
        {
            // Setup default variable values
            // TODO - all this
            Height = 1;
            LastHash = "7fb97df81221dd1366051b2d0bc7f49c66c22ac4431d879c895b06d66ef66f4c";
        }

        #endregion
    }
}
