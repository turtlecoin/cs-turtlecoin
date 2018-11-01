//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Blockchain.Crypto;
using Canti.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.Blockchain.Commands
{
    class RequestTxPool
    {
        // Command ID
        public const int Id = GlobalsConfig.CRYPTONOTE_COMMANDS_BASE + 8;

        // Outgoing request structure
        public struct Request : ICommandRequestBase
        {
            // Variables
            public string[] Txs;

            // Serializes request data into a byte array
            public byte[] Serialize()
            {
                // Create a portable storage
                PortableStorage Storage = new PortableStorage();

                // Add entries
                Storage.AddEntryAsBinary("txs", Txs);

                // Return serialized byte array
                return Storage.Serialize();
            }

            // Deseriaizes response data
            public static Request Deserialize(byte[] Data)
            {
                // Deserialize data
                PortableStorage Storage = new PortableStorage();
                Storage.Deserialize(Data);

                // Populate and return new response
                return new Request
                {
                    Txs = Hashing.DeserializeHashArray((string)Storage.GetEntry("txs"))
                };
            }
        }

        // Process incoming command instance
        public static void Invoke(LevinProtocol Context, LevinPeer Peer, Command Command)
        {
            // Command is a request
            if (!Command.IsResponse)
            {
                // Deserialize request
                Request Request = Request.Deserialize(Command.Data);

                // debug
                Logger.Log(Level.DEBUG, "[IN] Received \"Request TX Pool\" Request:");
                Logger.Log(Level.DEBUG, "- Response Requested: {0}", !Command.IsNotification);
                Logger.Log(Level.DEBUG, "- TXs:");
                for (int i = 0; i < Request.Txs.Length; i++)
                    Logger.Log(Level.DEBUG, "  - [{0}]: {1}", i, Request.Txs[i]);

                // TODO: Do something with request data
            }
        }
    }
}
