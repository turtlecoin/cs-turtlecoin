//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain.Commands
{
    public class Ping
    {
        // Command ID
        public const int Id = GlobalsConfig.LEVIN_COMMANDS_BASE + 3;

        // Outgoing request structure
        public struct Request : ICommandRequestBase
        {
            // Serializes request data into a byte array
            public byte[] Serialize()
            {
                // No data is needed for this request
                return new byte[0];
            }
        }

        // Incoming response structure
        public struct Response : ICommandResponseBase<Response>
        {
            // Variables
            public string Status { get; set; }
            public ulong PeerId { get; set; }

            // Deseriaizes response data
            public static Response Deserialize(byte[] Data)
            {
                // Deserialize data
                PortableStorage Storage = new PortableStorage();
                Storage.Deserialize(Data);

                // Populate and return new response
                return new Response
                {
                    Status = (string)Storage.GetEntry("status"),
                    PeerId = (ulong)Storage.GetEntry("peer_id")
                };
            }
        }

        // Process incoming command instance
        public static void Invoke(LevinProtocol Context, LevinPeer Peer, Command Command)
        {
            // Command is a request
            if (!Command.IsResponse)
            {
                // TODO: Do something
            }

            // Command is a response
            else
            {
                // TODO: Do something
            }
        }
    }
}
