//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Data;
using Canti.Utilities;
using System;

namespace Canti.Blockchain.Commands
{
    class TimedSync
    {
        // Command ID
        public const int Id = GlobalsConfig.LEVIN_COMMANDS_BASE + 2;

        // Outgoing request structure
        public struct Request : ICommandRequestBase
        {
            // Variables
            public CoreSyncData PayloadData { get; set; }

            // Serializes request data into a byte array
            public byte[] Serialize()
            {
                // Create a portable storage
                PortableStorage Storage = new PortableStorage();

                // Add entries
                Storage.AddEntry("payload_data", PayloadData);

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
                    PayloadData = CoreSyncData.Deserialize(Storage.GetEntry("payload_data"))
                };
            }
        }

        // Incoming response structure
        public struct Response : ICommandResponseBase<Response>
        {
            // Variables
            public ulong LocalTime { get; set; }
            public CoreSyncData PayloadData { get; set; }
            public PeerlistEntry[] LocalPeerlist { get; set; }

            // Serializes response data
            public byte[] Serialize()
            {
                // Create a portable storage
                PortableStorage Storage = new PortableStorage();

                // Add entries
                Storage.AddEntry("local_time", LocalTime);
                Storage.AddEntry("payload_data", PayloadData);
                Storage.AddEntryAsBinary("local_peerlist", LocalPeerlist);

                // Return serialized byte array
                return Storage.Serialize();
            }

            // Deseriaizes response data
            public static Response Deserialize(byte[] Data)
            {
                // Deserialize data
                PortableStorage Storage = new PortableStorage();
                Storage.Deserialize(Data);

                // Populate and return new response
                return new Response
                {
                    LocalTime = (ulong)Storage.GetEntry("local_time"),
                    PayloadData = CoreSyncData.Deserialize(Storage.GetEntry("payload_data")),
                    LocalPeerlist = Storage.DeserializeArrayFromBinary<PeerlistEntry>("local_peerlist")
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
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "[IN] Received \"Timed Sync\" Request:", LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Response Requested: " + !Command.IsNotification, LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Core Sync Data:", LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Current Height: " + Request.PayloadData.CurrentHeight, LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Top ID: " + Encoding.StringToHexString(Request.PayloadData.TopId), LogLevel.DEBUG);

                // TODO: Do something with request data

                // TODO: Do some processing in here, make sure the packet isn't a notification for some reason,
                //       make sure peer isn't duplicate, etc.

                // Create a response
                Response Response = new Response
                {
                    LocalTime = GeneralUtilities.GetTimestamp(),
                    PayloadData = new CoreSyncData
                    {
                        CurrentHeight = Globals.DAEMON_BLOCK_HEIGHT,
                        TopId = Globals.DAEMON_TOP_ID
                    },
                    LocalPeerlist = Globals.DAEMON_PEERLIST
                };

                // debug
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "[OUT] Sending \"Timed Sync\" Response:", LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Local Time: " + Response.LocalTime, LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Core Sync Data:", LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Current Height: " + Response.PayloadData.CurrentHeight, LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Top ID: " + Encoding.StringToHexString(Response.PayloadData.TopId), LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Local Peerlist:", LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Entries: " + Response.LocalPeerlist.Length, LogLevel.DEBUG);

                // Reply with response
                Context.Reply(Peer, Id, Response.Serialize(), true);
            }

            // Command is a response
            else
            {
                // Deserialize response
                Response Response = Response.Deserialize(Command.Data);

                // debug
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "[IN] Received \"Timed Sync\" Response:", LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Response Requested: " + !Command.IsNotification, LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Local Time: " + Response.LocalTime, LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Core Sync Data:", LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Current Height: " + Response.PayloadData.CurrentHeight, LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Top ID: " + Encoding.StringToHexString(Response.PayloadData.TopId), LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Local Peerlist:", LogLevel.DEBUG);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Entries: " + Response.LocalPeerlist.Length, LogLevel.DEBUG);

                // TODO: Do something with response data
            }
        }
    }
}
