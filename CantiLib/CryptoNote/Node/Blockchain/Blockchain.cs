//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        #region Properties and Fields

        #region Internal

        // This node's blockchain storage handler
        internal BlockchainCache Blockchain { get; private set; }

        #endregion

        #region Private

        // The location of the database being used
        private string DatabaseLocation { get; set; }

        // The database this node will use for storage
        private IDatabase Database { get; set; }

        #endregion

        #endregion

        #region Methods

        // Starts the blockchain cache and local storage
        private bool StartBlockchainCache()
        {
            // Initialize database
            switch (Globals.DATABASE_TYPE)
            {
                case DatabaseType.SQLITE:
                    DatabaseLocation = CombinePath(Globals.LOCAL_STORAGE_DIRECTORY, Globals.DATABASE_LOCATION);
                    Database = new Sqlite(DatabaseLocation);
                    break;
                default:
                    throw new ArgumentException("Invalid or non-specified database type");
            }
            Logger.Debug("Database initialized");

            // Start blockchain cache
            /*try
            {*/
                Blockchain.Start(Database);
            /*}
            catch (Exception e)
            {
                Logger.Error($"Could not start blockchain cache: {e.Message}");
                return false;
            }
            Logger.Debug("Cache started");*/

            // Blockchain cache started
            return true;
        }

        // Stops the blockchain cache
        private void StopBlockchainCache()
        {
            // Stops the blockchain cache
            Blockchain.Stop();
            Logger.Debug("Blockchain cache stopped");
        }

        #endregion
    }
}
