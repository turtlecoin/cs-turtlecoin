//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.CryptoNote
{
    // Packet command codes for levin and cryptonote protocol packets
    internal enum PacketType
    {
        // P2P Packets
        HANDSHAKE = 1001,
        TIMED_SYNC = 1002,
        PING = 1003,

        // CryptoNote Packets
        NOTIFY_NEW_BLOCK = 2001,
        NOTIFY_NEW_TRANSACTIONS = 2002,
        NOTIFY_GET_OBJECTS_REQUEST = 2003,
        NOTIFY_GET_OBJECTS_RESPONSE = 2004,
        // -
        NOTIFY_CHAIN_REQUEST = 2006,
        NOTIFY_CHAIN_RESPONSE = 2007,
        NOTIFY_TX_POOL_REQUEST = 2008,
        NOTIFY_NEW_LITE_BLOCK = 2009,
        NOTIFY_MISSING_TXS = 2010
    }

    // Packet flags that give an indicator on which direction a packet is moving
    internal enum PacketFlag
    {
        REQUEST = 0x00000001,
        RESPONSE = 0x00000002
    }
}
