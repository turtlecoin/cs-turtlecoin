//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.Blockchain
{
    public interface IProtocol
    {
        void OnDataReceived(object sender, EventArgs e);
        void OnPeerConnected(object sender, EventArgs e);
        void OnPeerDisconnected(object sender, EventArgs e);
    }
}
