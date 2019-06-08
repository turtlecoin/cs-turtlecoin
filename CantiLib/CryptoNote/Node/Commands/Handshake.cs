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
        // Verifies handshake information from a node
        private void VerifyNodeData(Peer Peer, Packet Packet)
        {
            // Check if this peer is already validated
            if (Peer.Validated)
            {
                throw new InvalidOperationException($"Received a handshake packet from an already validated peer");
            }

            // Verify network id
            if (Packet["node_data"]["network_id"] != ByteArrayToHexString(Globals.NETWORK_ID))
            {
                throw new InvalidOperationException($"Wrong network id");
            }

            // Check that peer ID differs from our own
            if (Packet["node_data"]["peer_id"] == Id)
            {
                throw new InvalidOperationException($"Detected connection with self");
            }

            // Check version
            if (Packet["node_data"]["version"] < Globals.P2P_MINIMUM_VERSION)
            {
                throw new InvalidOperationException($"Version is outdated");
            }

            // Check timestamp delta
            if (GetTimeDelta(Packet["node_data"]["local_time"]) > Globals.P2P_HANDSHAKE_TIME_DELTA)
            {
                throw new InvalidOperationException("Time delta is too large");
            }
        }

        // A handshake packet was received
        private void HandleHandshake(Peer Peer, Packet Packet)
        {
            // Verify node data
            VerifyNodeData(Peer, Packet);

            // Let the node know a handshake has been accepted
            OnPeerConnected(Peer);

            // Send response
            if (Packet.Flag == PacketFlag.REQUEST)
            {
                // Construct a response packet
                var Response = new Packet(PacketType.HANDSHAKE, PacketFlag.RESPONSE, false)
                {
                    ["node_data"] = GetNodeData(),
                    ["payload_data"] = GetCoreSyncData(),
                    ["local_peerlist"] = SerializePeerList()
                };

                // Send our response
                Peer.SendMessage(Response);
            }

            // Add peer list candidates
            if (Packet["local_peerlist"] != null)
            {
                AddPeerCandidates(HexStringToByteArray(Packet["local_peerlist"]));
            }

            // Add core sync data
            HandleSyncData(Peer, Packet["payload_data"]);

            // Set peer to validated
            Peer.Port = Packet["node_data"]["my_port"];
            Peer.Id = Packet["node_data"]["peer_id"];
            Peer.Validated = true;
        }

        // Sends a handshake request packet
        private void RequestHandshake(Peer Peer)
        {
            // Check if this peer is already validated
            if (Peer.Validated || Peer.State != PeerState.BEFORE_HANDSHAKE)
            {
                throw new InvalidOperationException($"Attempted to handshake with an already validated peer");
            }

            // Construct a request packet
            var Request = new Packet(PacketType.HANDSHAKE, PacketFlag.REQUEST, true)
            {
                ["node_data"] = GetNodeData(),
                ["payload_data"] = GetCoreSyncData()
            };

            // Send our request
            Peer.SendMessage(Request);
        }
    }
}
