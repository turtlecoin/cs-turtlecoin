//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    // Contains information about a packet's contents
    internal sealed class PacketHeader
    {
        #region Properties and Fields

        #region Internal

        // This signature number must match on both the receiving and sending sides
        internal ulong Signature { get; set; }

        // This is the version of the levin protocol we will be using
        internal uint ProtocolVersion { get; set; }

        // Command code is the packet type
        internal uint CommandCode { get; set; }

        // Whether or not we are requesting a response
        internal bool ResponseRequired { get; set; }

        // Payload size is how many bytes the body of this packet contains
        internal ulong PayloadSize { get; set; }

        // Return code that says whether or not a request was successful or not
        internal uint ReturnCode { get; set; }

        // Flag is the packet type (request or response)
        internal uint Flag { get; set; }

        #endregion

        #endregion

        #region Methods

        // Serializes the header information into a 33-byte array
        internal byte[] Serialize()
        {
            // Create output buffer
            byte[] Output = new byte[0];

            // Add our packet header information to our output buffer
            Output = Output.AppendBytes(IntegerToByteArray(Signature));
            Output = Output.AppendBytes(IntegerToByteArray(PayloadSize));
            Output = Output.AppendBytes(new byte[] { Convert.ToByte(ResponseRequired) });
            Output = Output.AppendBytes(IntegerToByteArray(CommandCode));
            Output = Output.AppendBytes(IntegerToByteArray(ReturnCode));
            Output = Output.AppendBytes(IntegerToByteArray(Flag));
            Output = Output.AppendBytes(IntegerToByteArray(ProtocolVersion));

            // Return output buffer
            return Output;
        }

        #endregion

        #region Constructors

        // Deserializes the header information from a byte array
        internal PacketHeader(byte[] Data)
        {
            // Decode signature
            Signature = ByteArrayToInteger<ulong>(Data);
            Data = Data.SubBytes(8, Data.Length - 8);

            // Decode payload size
            PayloadSize = ByteArrayToInteger<ulong>(Data);
            Data = Data.SubBytes(8, Data.Length - 8);

            // Decode response required
            ResponseRequired = Convert.ToBoolean(Data[0]);
            Data = Data.SubBytes(1, Data.Length - 1);

            // Decode command code
            CommandCode = ByteArrayToInteger<uint>(Data);
            Data = Data.SubBytes(4, Data.Length - 4);

            // Decode return code
            ReturnCode = ByteArrayToInteger<uint>(Data);
            Data = Data.SubBytes(4, Data.Length - 4);

            // Decode flags
            Flag = ByteArrayToInteger<uint>(Data);
            Data = Data.SubBytes(4, Data.Length - 4);

            // Decode protocol version
            ProtocolVersion = ByteArrayToInteger<uint>(Data);
        }

        // Initializes an empty packet header
        internal PacketHeader() { }

        #endregion
    }
}
