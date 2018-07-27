//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Data;
using System;

namespace Canti.Blockchain
{
    // Header for levin protocol requests
    [Serializable]
    public struct BucketHead2
    {
        // Header variables (names should be self explanatory)
        public ulong Signature { get; set; }
        public ulong PayloadSize { get; set; }
        public bool ResponseRequired { get; set; }
        public uint CommandCode { get; set; }
        public int ReturnCode { get; set; }
        public uint Flags { get; set; }
        public uint ProtocolVersion { get; set; }

        // Serializes the header information into a byte array
        public byte[] Serialize()
        {
            // Create output array
            byte[] Output = new byte[33];
            int Offset = 0;

            // Add signature bytes
            byte[] CurrentBytes = Encoding.IntegerToByteArray(Signature);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add payload size bytes
            CurrentBytes = Encoding.IntegerToByteArray(PayloadSize);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add response required bytes
            CurrentBytes = new byte[] { Convert.ToByte(ResponseRequired) };
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add command code bytes
            CurrentBytes = Encoding.IntegerToByteArray(CommandCode);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add return code bytes
            CurrentBytes = Encoding.IntegerToByteArray(ReturnCode);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add flags bytes
            CurrentBytes = Encoding.IntegerToByteArray(Flags);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add protocol version bytes
            CurrentBytes = Encoding.IntegerToByteArray(ProtocolVersion);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Return output array
            return Output;
        }

        // Deserializes the header information from a byte array
        public static BucketHead2 Deserialize(byte[] Data)
        {
            try
            {
                // Create a header object
                BucketHead2 Head = new BucketHead2();

                // Create a working byte array
                byte[] CurrentBytes = new byte[0];

                // Decode signature
                CurrentBytes = new byte[8];
                Buffer.BlockCopy(Data, 0, CurrentBytes, 0, CurrentBytes.Length);
                Head.Signature = Encoding.ByteArrayToInteger<ulong>(CurrentBytes);

                // Decode payload size
                CurrentBytes = new byte[8];
                Buffer.BlockCopy(Data, 8, CurrentBytes, 0, CurrentBytes.Length);
                Head.PayloadSize = Encoding.ByteArrayToInteger<ulong>(CurrentBytes);

                // Decode response required
                CurrentBytes = new byte[1];
                Buffer.BlockCopy(Data, 16, CurrentBytes, 0, CurrentBytes.Length);
                Head.ResponseRequired = Convert.ToBoolean(CurrentBytes[0]);

                // Decode command code
                CurrentBytes = new byte[4];
                Buffer.BlockCopy(Data, 17, CurrentBytes, 0, CurrentBytes.Length);
                Head.CommandCode = Encoding.ByteArrayToInteger<uint>(CurrentBytes);

                // Decode return code
                CurrentBytes = new byte[4];
                Buffer.BlockCopy(Data, 21, CurrentBytes, 0, CurrentBytes.Length);
                Head.ReturnCode = Encoding.ByteArrayToInteger<int>(CurrentBytes);

                // Decode flags
                CurrentBytes = new byte[4];
                Buffer.BlockCopy(Data, 25, CurrentBytes, 0, CurrentBytes.Length);
                Head.Flags = Encoding.ByteArrayToInteger<uint>(CurrentBytes);

                // Decode protocol version
                CurrentBytes = new byte[4];
                Buffer.BlockCopy(Data, 29, CurrentBytes, 0, CurrentBytes.Length);
                Head.ProtocolVersion = Encoding.ByteArrayToInteger<uint>(CurrentBytes);

                // Return header object
                return Head;
            }
            catch
            {
                return default(BucketHead2);
            }
        }
    };
}
