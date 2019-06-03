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

            // TODO - add unique flags and proper value types (need to finish researching)
            // { "Name", SQL Type, Size (if applicable), Unique, Default (if applicable) }
            // Create blocks table if it doesn't exist
            Logger.Debug("Setting up blocks table...");
            Database.CreateTable(
                // Table name
                "blocks",

                // Table columns
                new ValueList
                {
                    { "height", SqlType.INT, 0 },
                    { "hash", SqlType.CHAR, 64 },
                    { "size", SqlType.BIGINT },
                    { "timestamp", SqlType.TIMESTAMP },
                    { "nonce", SqlType.INT },
                    { "major_version", SqlType.TINYINT },
                    { "minor_version", SqlType.TINYINT },
                    { "base_reward", SqlType.BIGINT },
                    { "total_fees", SqlType.BIGINT },
                    { "base_transaction", SqlType.CHAR, 64 }
                }
            );

            // Create transactions table if it doesn't exist
            Logger.Debug("Setting up transactions table...");
            Database.CreateTable(
                // Table name
                "transactions",

                // Table columns
                new ValueList
                {
                    { "hash", SqlType.CHAR, 64 },
                    { "blockhash", SqlType.CHAR, 64},
                    { "public_key", SqlType.CHAR, 64 },
                    { "payment_id", SqlType.CHAR, 64 },
                    { "size", SqlType.BIGINT },
                    { "fee", SqlType.BIGINT },
                    { "mixin", SqlType.TINYINT },
                    { "unlock_time", SqlType.BIGINT },
                    { "verified", SqlType.BOOLEAN },
                    { "inputs", SqlType.VARBINARY },
                    { "outputs", SqlType.VARBINARY },
                    { "signatures", SqlType.VARBINARY },
                    { "extra", SqlType.VARBINARY }
                }
            );
        }

        // Stops the database
        private void StopDatabase()
        {
            Database.Stop();
        }
    }
}
