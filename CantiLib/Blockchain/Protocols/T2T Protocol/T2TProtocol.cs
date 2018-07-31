//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Blockchain.P2P;
using Canti.Utilities;
using System;
using System.Collections.Generic;

namespace Canti.Blockchain
{
    class T2TProtocol : IProtocol
    {
        // Server connection
        public Server Server;

        // Logger
        public Logger Logger;

        // Peer read status (0 = head, 1 = body)
        private Dictionary<PeerConnection, LevinPeer> Peers = new Dictionary<PeerConnection, LevinPeer>();

        // Entry point
        public T2TProtocol(Server Connection)
        {
            // Set connection
            Server = Connection;

            // Set logger
            Logger = Connection.Logger;

            // Bind event handlers
            Server.OnDataReceived += OnDataReceived;
            Server.OnPeerConnected += OnPeerConnected;
            Server.OnPeerDisconnected += OnPeerDisconnected;
        }

        public void OnDataReceived(object sender, EventArgs e)
        {
            // TODO
        }

        public void OnPeerConnected(object sender, EventArgs e)
        {
            // TODO
        }

        public void OnPeerDisconnected(object sender, EventArgs e)
        {
            // TODO
        }
    }
}
