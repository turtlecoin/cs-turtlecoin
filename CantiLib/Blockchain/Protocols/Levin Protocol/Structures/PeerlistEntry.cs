//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.Blockchain
{
    [Serializable]
    public struct PeerlistEntry
    {
        public NetworkAddress Address { get; set; }
        public ulong Id { get; set; }
        public ulong LastSeen { get; set; }
        public byte[] Serialize()
        {
            PortableStorage Storage = new PortableStorage();
            Storage.AddEntry("address", Address.Serialize());
            Storage.AddEntry("id", Id);
            Storage.AddEntry("last_seen", LastSeen);
            return Storage.Serialize(false);
        }
    };
}
