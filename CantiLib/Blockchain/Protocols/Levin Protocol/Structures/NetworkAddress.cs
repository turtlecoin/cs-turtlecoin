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
    public struct NetworkAddress
    {
        public uint IpAddress { get; set; }
        public uint Port { get; set; }
        public byte[] Serialize()
        {
            PortableStorage Storage = new PortableStorage();
            Storage.AddEntry("ip", IpAddress);
            Storage.AddEntry("port", Port);
            return Storage.Serialize(false);
        }
    };
}
