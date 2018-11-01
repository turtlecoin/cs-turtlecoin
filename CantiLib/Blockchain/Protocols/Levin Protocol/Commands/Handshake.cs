//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 


// Please see the included LICENSE file for more information.using Canti.Data;
using Canti.Utilities;
using Canti.Data;
using System;

namespace Canti.Blockchain.Commands
{
    public class Handshake
    {
        // Command ID
        public const int Id = GlobalsConfig.LEVIN_COMMANDS_BASE + 1;

        // Outgoing request structure
        public struct Request : ICommandRequestBase
        {
            // Variables
            public NodeData NodeData { get; set; }
            public CoreSyncData PayloadData { get; set; }

            // Serializes request data into a byte array
            public byte[] Serialize()
            {
                // Create a portable storage
                PortableStorage Storage = new PortableStorage();

                // Add entries
                Storage.AddEntry("node_data", NodeData);
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
                    NodeData = NodeData.Deserialize(Storage.GetEntry("node_data")),
                    PayloadData = CoreSyncData.Deserialize(Storage.GetEntry("payload_data"))
                };
            }
        }

        // Incoming response structure
        public struct Response : ICommandResponseBase<Response>
        {
            // Variables
            public NodeData NodeData { get; set; }
            public CoreSyncData PayloadData { get; set; }
            public PeerlistEntry[] LocalPeerlist { get; set; }

            // Serializes response data
            public byte[] Serialize()
            {
                // Create a portable storage
                PortableStorage Storage = new PortableStorage();

                // Add entries
                Storage.AddEntry("node_data", NodeData);
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
                    NodeData = NodeData.Deserialize(Storage.GetEntry("node_data")),
                    PayloadData = CoreSyncData.Deserialize(Storage.GetEntry("payload_data")),
                    LocalPeerlist = Storage.DeserializeArrayFromBinary<PeerlistEntry>("local_peerlist")
                };
            }
        }

        // Process incoming command instance
        public static void Invoke(LevinProtocol Context, LevinPeer Peer, Command Command)
        {
            // Debug
            Peer.State = PeerState.Verified;

            // Command is a request
            if (!Command.IsResponse)
            {
                // Deserialize request
                Request Request = Request.Deserialize(Command.Data);

                // debug
                Logger.Log(Level.DEBUG, "[IN] Received \"Handshake\" Request:");
                Logger.Log(Level.DEBUG, "- Response Requested: {0}", !Command.IsNotification);
                Logger.Log(Level.DEBUG, "- Node Data:");
                Logger.Log(Level.DEBUG, "  - Network ID: {0}", Encoding.StringToHexString(Request.NodeData.NetworkId));
                Logger.Log(Level.DEBUG, "  - Peer ID: {0}", Request.NodeData.PeerId);
                Logger.Log(Level.DEBUG, "  - Version: {0}", Request.NodeData.Version);
                Logger.Log(Level.DEBUG, "  - Local Time: {0}", Request.NodeData.LocalTime);
                Logger.Log(Level.DEBUG, "  - Port: {0}", Request.NodeData.Port);
                Logger.Log(Level.DEBUG, "- Core Sync Data:");
                Logger.Log(Level.DEBUG, "  - Current Height: {0}", Request.PayloadData.CurrentHeight);
                Logger.Log(Level.DEBUG, "  - Top ID: {0}", Encoding.StringToHexString(Request.PayloadData.TopId));

                // TODO: Do something with request data

                // TODO: Do some processing in here, make sure the packet isn't a notification for some reason,
                //       make sure peer isn't duplicate, etc.

                // Create a response
                Response Response = new Response
                {
                    NodeData = new NodeData
                    {
                        NetworkId = GlobalsConfig.NETWORK_ID,
                        LocalTime = GeneralUtilities.GetTimestamp(),
                        Port = (uint)Context.Server.Port,
                        PeerId = Context.Server.PeerId,
                        Version = GlobalsConfig.LEVIN_VERSION
                    },
                    PayloadData = new CoreSyncData
                    {
                        CurrentHeight =  Globals.DAEMON_BLOCK_HEIGHT,
                        TopId = Globals.DAEMON_TOP_ID
                    },
                    LocalPeerlist = Globals.DAEMON_PEERLIST
                };

                // debug
                Logger.Log(Level.DEBUG, "[OUT] Sending \"Handshake\" Response:");
                Logger.Log(Level.DEBUG, "- Node Data:");
                Logger.Log(Level.DEBUG, "  - Network ID: {0}", Encoding.StringToHexString(Response.NodeData.NetworkId));
                Logger.Log(Level.DEBUG, "  - Peer ID: {0}", Response.NodeData.PeerId);
                Logger.Log(Level.DEBUG, "  - Version: {0}", Response.NodeData.Version);
                Logger.Log(Level.DEBUG, "  - Local Time: {0}", Response.NodeData.LocalTime);
                Logger.Log(Level.DEBUG, "  - Port: {0}", Response.NodeData.Port);
                Logger.Log(Level.DEBUG, "- Core Sync Data:");
                Logger.Log(Level.DEBUG, "  - Current Height: {0}", Response.PayloadData.CurrentHeight);
                Logger.Log(Level.DEBUG, "  - Top ID: {0}", Encoding.StringToHexString(Response.PayloadData.TopId));
                Logger.Log(Level.DEBUG, "- Local Peerlist:");
                Logger.Log(Level.DEBUG, "  - Entries: {0}", Response.LocalPeerlist.Length);

                // Reply with response
                Context.Reply(Peer, Id, Response.Serialize(), true);
            }

            // Command is a response
            else if (Command.IsResponse && Peer.State != PeerState.Verified)
            {
                // Deserialize response
                Response Response = Response.Deserialize(Command.Data);

                // debug
                Logger.Log(Level.DEBUG, "[IN] Received \"Handshake\" Response:");
                Logger.Log(Level.DEBUG, "- Response Requested: {0}", !Command.IsNotification);
                Logger.Log(Level.DEBUG, "- Node Data:");
                Logger.Log(Level.DEBUG, "  - Network ID: {0}", Encoding.StringToHexString(Response.NodeData.NetworkId));
                Logger.Log(Level.DEBUG, "  - Peer ID: {0}", Response.NodeData.PeerId);
                Logger.Log(Level.DEBUG, "  - Version: {0}", Response.NodeData.Version);
                Logger.Log(Level.DEBUG, "  - Local Time: {0}", Response.NodeData.LocalTime);
                Logger.Log(Level.DEBUG, "  - Port: {0}", Response.NodeData.Port);
                Logger.Log(Level.DEBUG, "- Core Sync Data:");
                Logger.Log(Level.DEBUG, "  - Current Height: {0}", Response.PayloadData.CurrentHeight);
                Logger.Log(Level.DEBUG, "  - Top ID: {0}", Encoding.StringToHexString(Response.PayloadData.TopId));
                Logger.Log(Level.DEBUG, "- Local Peerlist:");
                Logger.Log(Level.DEBUG, "  - Entries: {0}", Response.LocalPeerlist.Length);

                // TODO: Do something with response data
            }
        }
    }
}
