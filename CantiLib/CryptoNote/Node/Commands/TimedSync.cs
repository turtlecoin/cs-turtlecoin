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
        // A timed sync packet was received
        private void HandleTimedSync(Peer Peer, Packet Packet)
        {
            // Check if this peer is validated
            if (!Peer.Validated)
            {
                throw new InvalidOperationException($"Received a {Packet.Type} packet from non-validated peer {Peer.Address}:{Peer.Port}, killing connection...");
            }

            // TODO - use node time

            // Add core sync data
            AddSyncData(Peer, Packet["payload_data"]);

            // Received a request
            if (Packet.Flag == PacketFlag.REQUEST)
            {
                // Construct a response packet
                var Response = new Packet(PacketType.TIMED_SYNC, PacketFlag.RESPONSE, false);
                Response["payload_data"] = new Dictionary<string, dynamic>
                {
                    ["current_height"] = Blockchain.Height,
                    ["top_id"] = Blockchain.TopId
                };
                Response["local_peerlist"] = SerializePeerList();

                // Send our response
                Peer.SendMessage(Response);
            }

            // Received a response
            else
            {
                // Update peer list
                AddPeerCandidates(HexStringToByteArray(Packet["local_peerlist"]));
            }
        }

        // Sends a timed sync request packet
        private void RequestTimedSync(Peer Peer)
        {
            // Check if this peer is validated
            if (!Peer.Validated) return;

            // Construct a request packet
            var Request = new Packet(PacketType.TIMED_SYNC, PacketFlag.REQUEST, true);
            Request["payload_data"] = new Dictionary<string, dynamic>
            {
                ["current_height"] = Blockchain.Height,
                ["top_id"] = Blockchain.TopId
            };

            // Send our request
            Peer.SendMessage(Request);
        }
    }
}
