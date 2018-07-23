//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain.P2P
{
    public struct Packet
    {
        public PeerConnection Peer;
        public byte[] Data;

        public Packet(PeerConnection Peer, byte[] Data)
        {
            this.Peer = Peer;
            this.Data = Data;
        }
    }
}
