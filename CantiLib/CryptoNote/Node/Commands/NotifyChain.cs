//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        // Handshake request was received
        private void HandleNotifyChainRequest(Peer Peer, Packet Packet)
        {
            // TODO - Stuff

            // Construct a response packet
            var Response = new Packet(PacketType.NOTIFY_CHAIN_REQUEST, PacketFlag.RESPONSE, false);
            Response["node_data"] = new Dictionary<string, dynamic>
            {
                ["network_id"] = Globals.NETWORK_ID,
                ["version"] = Globals.P2P_CURRENT_VERSION,
                ["peer_id"] = Id,
                ["local_time"] = GetTimestamp(),
                ["my_port"] = P2pPort
            };
            Response["payload_data"] = new Dictionary<string, dynamic>
            {
                ["current_height"] = Blockchain.Height,
                ["top_id"] = Blockchain.TopId
            };
            Response["peer_list"] = SerializePeerList();

            // Send our response
            Peer.SendMessage(Response);
        }

        // Handshake response was received
        private void HandleNotifyChainResponse(Peer Peer, Packet Packet)
        {
            // TODO - Stuff
        }

        // A handshake packet was received
        private void HandleNotifyChain(Peer Peer, Packet Packet)
        {
            // Check if this peer is validated
            if (!Peer.Validated)
            {
                throw new InvalidOperationException($"Received a {Packet.Type} packet from non-validated peer {Peer.Address}:{Peer.Port}, killing connection...");
            }

            // Check packet flag
            switch (Packet.Type)
            {
                case PacketType.NOTIFY_CHAIN_REQUEST:
                    HandleNotifyChainRequest(Peer, Packet);
                    break;

                case PacketType.NOTIFY_CHAIN_RESPONSE:
                    HandleNotifyChainResponse(Peer, Packet);
                    break;
            }
        }

        // Sends a handshake request packet
        private void NotifyChain(Peer Peer)
        {
            // Check if this peer is validated
            if (!Peer.Validated) return;

            // Construct a request packet
            var Request = new Packet(PacketType.NOTIFY_CHAIN_REQUEST, PacketFlag.REQUEST, false);
            Request["block_ids"] = Blockchain.TopId;

            // Send our request
            Peer.SendMessage(Request);
        }
    }
}
