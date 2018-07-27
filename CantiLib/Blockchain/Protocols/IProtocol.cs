using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.Blockchain
{
    interface IProtocol
    {
        void OnDataReceived(object sender, EventArgs e);
        void OnPeerConnected(object sender, EventArgs e);
        void OnPeerDisconnected(object sender, EventArgs e);
    }
}
