//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using static Canti.Utils;

namespace Canti.CryptoNote.Blockchain
{
    [Serializable]
    internal class Input
    {
        // Input type
        internal byte Type { get; set; }

        // Amount in this input
        internal ulong Amount { get; set; }

        // The block index this input belongs to, if its type is a mining reward
        internal uint BlockIndex { get; set; }

        // List of indexes corresponding to outputs within a transaction
        internal uint[] OutputIndexes { get; set; }

        // A unique key representing this input
        internal string PublicKey { get; set; }

        // Returns a byte array representation of this input
        internal byte[] Serialize()
        {
            // Create an output buffer
            byte[] Buffer = new byte[0];

            // Add type
            Buffer = Buffer.AppendVarInt(Type);

            // Type is mining reward
            if (Type == 0x7F)
            {
                // Add block index
                Buffer = Buffer.AppendVarInt(BlockIndex);
            }

            // Type is transfer
            else
            {
                // Add amount
                Buffer = Buffer.AppendVarInt(Amount);

                // Add output indexes
                Buffer = Buffer.AppendVarInt(OutputIndexes.Length);
                foreach (var Index in OutputIndexes)
                {
                    Buffer = Buffer.AppendVarInt(Index);
                }

                // Add key
                Buffer = Buffer.AppendHexString(PublicKey);
            }

            // Completed, return output buffer
            return Buffer;
        }

        // Deserializes an input from a byte array
        internal Input(byte[] Data, int Offset, out int NewOffset)
        {
            // Get input type
            Type = UnpackVarInt<byte>(Data, Offset, out Offset);

            // Type is mining reward
            if (Type == 0x7f)
            {
                // Get block index
                BlockIndex = UnpackVarInt<uint>(Data, Offset, out Offset);
            }

            // Type is transfer
            else
            {
                // Get amount
                Amount = UnpackVarInt<ulong>(Data, Offset, out Offset);

                // Get output indexes
                List<uint> Indexes = new List<uint>();
                for (int x = 0; x < UnpackVarInt<uint>(Data, Offset, out Offset); x++)
                {
                    Indexes.Add(UnpackVarInt<uint>(Data, Offset, out Offset));
                }
                OutputIndexes = Indexes.ToArray();

                // Get key
                PublicKey = Data.SubHexString(Offset, 32);

                // Adjust offset to account for key length
                Offset += 32;
            }

            // Set new offset
            NewOffset = Offset;
        }
    }
}
