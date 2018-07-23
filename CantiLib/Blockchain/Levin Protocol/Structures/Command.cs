//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain
{
    // Levin command structure
    public struct Command
    {
        // Command variables (names should be self explanatory)
        public uint CommandCode { get; set; }
        public bool IsNotification { get; set; }
        public bool IsResponse { get; set; }
        public byte[] Data { get; set; }
    }
}
