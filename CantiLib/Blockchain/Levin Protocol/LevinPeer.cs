//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Blockchain.P2P;

namespace Canti.Blockchain
{
    // Wraps a peer connection with levin specific data
    public class LevinPeer
    {
        // public variables
        public byte[] Data = new byte[0];
        public BucketHead2 Header = default(BucketHead2);
        public PeerConnection Connection;

        // Peer stats
        public PacketReadStatus ReadStatus = PacketReadStatus.Head;
        public PeerState State = PeerState.Unverified;
        // Height
        // ID?
        // Hopes and dreams

        // Entry point
        public LevinPeer(PeerConnection Connection)
        {
            this.Connection = Connection;
        }
    }

    // Gives names to read status
    public enum PacketReadStatus : int
    {
        Head = 0,
        Body = 1
    }

    // A peer's connection state
    public enum PeerState : int
    {
        Unverified = 0,
        Verified = 1
    }
}
