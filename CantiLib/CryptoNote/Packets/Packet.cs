//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.CryptoNote
{
    internal sealed class Packet
    {
        #region Properties

        #region Internal

        // The type of packet
        internal PacketType Type { get; private set; }

        // Whether this packet is a request or response
        internal PacketFlag Flag { get; private set; }

        // The packet's header
        internal PacketHeader Header { get; private set; }

        // The packet's body
        internal PortableStorage Body { get; private set; }

        // Quick access to the packet body
        internal dynamic this[string Key]
        {
            get
            {
                return Body[Key];
            }
            set
            {
                Body[Key] = value;
            }
        }

        #endregion

        #endregion

        #region Methods

        // Serializes this packet to a byte array for sending to peers
        public byte[] Serialize()
        {
            byte[] Output = new byte[0];
            byte[] BodyBytes = Body.Serialize();
            Header.PayloadSize = (ulong)BodyBytes.LongLength;
            byte[] HeaderBytes = Header.Serialize();
            Output = Output.AppendBytes(HeaderBytes);
            Output = Output.AppendBytes(BodyBytes);
            return Output;
        }

        #endregion

        #region Constructors

        // Initializes a new outgoing packet
        internal Packet(PacketType Type, PacketFlag Flag, bool ResponseRequired, bool Success = true)
        {
            this.Type = Type;
            this.Flag = Flag;
            Body = new PortableStorage();
            Header = new PacketHeader()
            {
                Signature = 0x0101010101012101UL,
                ProtocolVersion = 1,
                CommandCode = (uint)Type,
                ResponseRequired = ResponseRequired,
                ReturnCode = (uint)(Success ? 0 : 1), // 0 = success
                Flag = (uint)Flag
            };
        }

        // Initializes a new incoming packet
        internal Packet(PacketHeader Header, PortableStorage Body)
        {
            Type = (PacketType)Header.CommandCode;
            Flag = (PacketFlag)Header.Flag;
            this.Header = Header;
            this.Body = Body;
        }

        #endregion
    }
}
