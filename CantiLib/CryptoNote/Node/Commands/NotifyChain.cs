//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        // A notify chain packet was received
        private void HandleNotifyChain(Peer Peer, Packet Packet)
        {
            // Check if this peer is validated
            if (!Peer.Validated)
            {
                throw new InvalidOperationException($"Received a {Packet.Type} packet from non-validated peer {Peer.Address}:{Peer.Port}, killing connection...");
            }

            // TODO - handle sync stuff
        }

        // Sends a notify chain request packet
        private void RequestNotifyChain(Peer Peer)
        {
            // Check if this peer is validated
            if (!Peer.Validated) return;

            // Construct a request packet
            var Request = new Packet(PacketType.NOTIFY_CHAIN_REQUEST, PacketFlag.REQUEST, false)
            {
                ["block_ids"] = GetSparseChain()
            };

            // Send our request
            Peer.SendMessage(Request);
        }
    }
}
