//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain
{
    // Interface for handling database connections and storage
    public interface IDatabase
    {
        // Storing
        void StoreBlock(Block Block);
        void StoreBlocks(Block[] Blocks);
        void StoreTransaction(Transaction Transaction);
        void StoreTransactions(Transaction[] Transactions);
        void StoreInput(Input Input);
        void StoreInputs(Input[] Inputs);
        void StoreOutput(Output Output);
        void StoreOutputs(Output[] Outputs);

        // Reading
        Block GetBlock(int Height);
        Block GetBlock(string BlockHash);
        Transaction GetTransaction(string TransactionHash);
        Transaction[] GetTransactionWithPaymentId(string PaymentId);
        Transaction[] GetTransactionInBlock(string BlockHash);
        Input[] GetInputs();
        Output GetOutput();
    }
}
