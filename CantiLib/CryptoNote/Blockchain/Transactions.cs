//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Linq;
using Canti.CryptoNote.Blockchain;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    internal sealed partial class BlockchainCache
    {
        #region Methods

        #region Database

        // Creates the transactions table if it doesn't exist
        private void CreateTransactionsTable()
        {
            // Create table
            Database.CreateTable(
                // Table name
                "transactions",

                // Table columns
                new ValueList
                {
                    { "version", SqlType.TINYINT },
                    { "hash", SqlType.CHAR, 64 },
                    { "blockhash", SqlType.CHAR, 64},
                    { "public_key", SqlType.CHAR, 64 },
                    { "payment_id", SqlType.CHAR, 64 },
                    { "size", SqlType.BIGINT },
                    { "total_amount", SqlType.BIGINT },
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

        // Helper for easier deserialization, so this doesn't need re-typed
        private Transaction DeserializeTransactionRow(ValueList Row)
        {
            return new Transaction()
            {
                Version = (byte)Row["version"],
                Hash = (string)Row["hash"],
                BlockHash = (string)Row["block_hash"],
                PublicKey = (string)Row["public_key"],
                Size = (ulong)Row["size"],
                TotalAmount = (ulong)Row["total_amount"],
                TotalFee = (ulong)Row["fee"],
                Mixin = (byte)Row["mixin"],
                UnlockTime = (ulong)Row["unlock_time"],
                Verified = (int)Row["verified"] == 1,

                // TODO - deserialize input and output arrays from bytes
                Inputs = ByteArrayToObject<Input[]>((byte[])Row["inputs"]),
                Outputs = ByteArrayToObject<Output[]>((byte[])Row["outputs"]),
                Signatures = ByteArrayToObject<byte[]>((byte[])Row["signatures"]),
                Extra = (byte[])Row["extra"]
            };
        }

        // Gets a transaction from the database by hash
        internal bool TryGetTransaction(string Hash, out Transaction Transaction)
        {
            // Check if database is started
            if (!Database.Started)
            {
                Transaction = new Transaction()
                {
                    Hash = Constants.NULL_HASH
                };
                return false;
            }

            // Query the database
            var Result = Database.Select(TRANSACTIONS_TABLE, new ValueList
            {
                ["hash"] = Hash
            });

            // If there are no matching entries, return false along with an empty transaction
            if (!Result.Any())
            {
                Transaction = new Transaction()
                {
                    Hash = Constants.NULL_HASH
                };
                return false;
            }

            // Serialize found transaction from first row of result
            Transaction = DeserializeTransactionRow(Result.First());
            return true;
        }

        // Stores a transaction in the database
        internal bool StoreTransaction(Block Block, Transaction Transaction)
        {
            // Check if database is started
            if (!Database.Started) return false;

            // Store transaction
            Database.Add(TRANSACTIONS_TABLE, new ValueList
            {
                ["version"] = Transaction.Version,
                ["hash"] = Transaction.Hash,
                ["blockhash"] = Block.Hash,
                ["public_key"] = Transaction.PublicKey,
                ["payment_id"] = Transaction.PaymentId,
                ["size"] = Transaction.Size,
                ["total_amount"] = Transaction.TotalAmount,
                ["fee"] = Transaction.TotalFee,
                ["mixin"] = Transaction.Mixin,
                ["unlock_time"] = Transaction.UnlockTime,
                ["verified"] = Transaction.Verified,
                ["inputs"] = ObjectToByteArray(Transaction.Inputs),
                ["outputs"] = ObjectToByteArray(Transaction.Outputs),
                ["signatures"] = ObjectToByteArray(Transaction.Signatures),
                ["extra"] = Transaction.Extra
            });

            // Completed
            return true;
        }

        // Checks if we have a transaction with this hash stored
        internal bool IsTransactionStored(string Hash)
        {
            // Check if database is started
            if (!Database.Started) return false;

            // Count the instances of this transaction hash
            var Count = Database.Count(TRANSACTIONS_TABLE, new ValueList
            {
                ["hash"] = Hash
            });
            return Count > 0;
        }

        #endregion

        #endregion
    }
}
