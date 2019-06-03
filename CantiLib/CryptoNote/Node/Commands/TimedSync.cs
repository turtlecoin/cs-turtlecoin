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
        // A timed sync packet was received
        private void HandleTimedSync(Peer Peer, Packet Packet)
        {
            // Check if this peer is validated
            if (!Peer.Validated)
            {
                throw new InvalidOperationException($"Received a {Packet.Type} packet from non-validated peer {Peer.Address}:{Peer.Port}, killing connection...");
            }

            // TODO - use node time?

            // Received a request
            if (Packet.Flag == PacketFlag.REQUEST)
            {
                // Construct a response packet
                var Response = new Packet(PacketType.TIMED_SYNC, PacketFlag.RESPONSE, false)
                {
                    ["local_time"] = GetTimestamp(),
                    ["payload_data"] = GetCoreSyncData(),
                    ["local_peerlist"] = SerializePeerList()
                };

                // Send our response
                Peer.SendMessage(Response);
            }

            // Received a response
            else
            {
                // Update peer list
                AddPeerCandidates(HexStringToByteArray(Packet["local_peerlist"]));
            }

            // Add core sync data
            HandleSyncData(Peer, Packet["payload_data"]);
        }

        // Sends a timed sync request packet
        private void RequestTimedSync(Peer Peer)
        {
            // Check if this peer is validated
            if (!Peer.Validated) return;

            // Construct a request packet
            var Request = new Packet(PacketType.TIMED_SYNC, PacketFlag.REQUEST, true)
            {
                ["payload_data"] = GetCoreSyncData()
            };

            // Send our request
            Peer.SendMessage(Request);
        }
    }
}
