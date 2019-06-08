//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.CryptoNote
{
    internal sealed partial class BlockchainCache
    {
        // The database we will use to store information
        private IDatabase Database { get; set; }

        // Starts the database and sets up associated tables
        private void StartDatabase(IDatabase Database)
        {
            // Assign and start database
            this.Database = Database;
            this.Database.Start();

            // TODO - add unique flags and proper value types when creating tables
            // Format:
            // { "Name", SQL Type, Size (if applicable), Unique, Auto Inc, Default (if applicable) }

            // Create blocks table if it doesn't exist
            Logger.Debug("Setting up blocks table...");
            CreateBlocksTable();

            // Create transactions table if it doesn't exist
            Logger.Debug("Setting up transactions table...");
            CreateTransactionsTable();
        }

        // Stops the database
        private void StopDatabase()
        {
            Database.Stop();
        }
    }
}
