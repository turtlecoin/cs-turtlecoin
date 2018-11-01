//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Blockchain.Crypto;
using Canti.Utilities;

namespace Canti.Blockchain.Commands
{
    class NotifyRequestChain
    {
        // Command ID
        public const int Id = GlobalsConfig.CRYPTONOTE_COMMANDS_BASE + 6;

        // Outgoing request structure
        public struct Request : ICommandRequestBase
        {
            // Variables
            public string[] BlockIds;

            // Serializes request data into a byte array
            public byte[] Serialize()
            {
                // Create a portable storage
                PortableStorage Storage = new PortableStorage();

                // Add entries
                Storage.AddEntryAsBinary("txs", BlockIds);

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
                    BlockIds = Hashing.DeserializeHashArray((string)Storage.GetEntry("block_ids"))
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
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "[IN] Received \"Notify Request Chain\" Request:", LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Response Requested: " + !Command.IsNotification, LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- TXs:", LogLevel.DEBUG);
                for (int i = 0; i < Request.BlockIds.Length; i++)
                {
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - [" + i + "]: " + Request.BlockIds[i], LogLevel.DEBUG);
                }                    

                // TODO: Do something with request data
            }
        }
    }
}
