//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain
{
    // Global levin protocol constants
    public partial class LevinProtocol
    {
        // Packet flags
        public const int LEVIN_PACKET_REQUEST = 0x00000001;
        public const int LEVIN_PACKET_RESPONSE = 0x00000002;

        // Version numbers
        public const int LEVIN_PROTOCOL_VER_0 = 0;
        public const int LEVIN_PROTOCOL_VER_1 = 1;

        // Return codes
        public const int LEVIN_RETCODE_SUCCESS = 0;
        public const int LEVIN_RETCODE_FAILURE = 1;
    }
}
