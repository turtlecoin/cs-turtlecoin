//
// Copyright 2012-2013 The CryptoNote Developers
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

namespace Canti.Cryptography.Native
{
    // Tree hash functions
    public static class TreeHash
    {
        public static byte[] Hash(byte[][] Hashes)
        {
            // Empty hashes array
            if (Hashes.Length == 0) return null;

            // If input length is 1, we don't need to perform any hashing
            if (Hashes.Length == 1) return Hashes[0];

            // If input length is 2, hash only those two values
            if (Hashes.Length == 2) return Keccak.KeccakHash(Hashes[0].AppendBytes(Hashes[1]));

            // Declare some values
            int InputIndex, OutputIndex;
            int Count = Hashes.Length - 1;

            // Perform some bitshift shenanigans
            for (InputIndex = 1; InputIndex < 8; InputIndex <<= 1)
            {
                Count |= Count >> InputIndex;
            }
            Count &= ~(Count >> 1);

            // Assign this value now so it's not assigned multiple times
            int val = 2 * Count - Hashes.Length;

            // Create an output array and copy a set of values to it
            byte[][] Buffer = new byte[Count][];
            for (InputIndex = 0; InputIndex < val; InputIndex++)
            {
                Buffer[InputIndex] = Hashes[InputIndex];
            }

            // Perform first round of hashing
            for (InputIndex = val, OutputIndex = val; OutputIndex < Count; InputIndex += 2, OutputIndex++)
            {
                Buffer[OutputIndex] = Keccak.KeccakHash(Hashes[InputIndex].AppendBytes(Hashes[InputIndex + 1]));
            }

            // Sanity check
            if (InputIndex != Hashes.Length)
            {
                throw new Exception("Invalid tree hash operation");
            }

            // Loop until there are just two hashes left
            while (Count > 2)
            {
                Count >>= 1;
                for (InputIndex = 0, OutputIndex = 0; OutputIndex < Count; InputIndex += 2, OutputIndex++)
                {
                    Buffer[OutputIndex] = Keccak.KeccakHash(Buffer[InputIndex].AppendBytes(Buffer[InputIndex + 1]));
                }
            }

            // Perform final hash
            return Keccak.KeccakHash(Buffer[0].AppendBytes(Buffer[1]));
        }

        public static byte[] HashFromBranch(byte[][] Branch, int Depth, ref byte[] Leaf, ref byte[] Path)
        {
            // If depth is 0, the leaf hash is our root
            if (Depth == 0)
            {
                return Leaf;
            }

            // Declare some variables
            HashBuffer Buffer = new HashBuffer();
            bool FromLeaf = true;

            // Loop until we are at root depth
            while (Depth > 0)
            {
                // Decrement depth
                Depth--;

                // Assign path values
                if (Path != null && (Path[Depth >> 3] & (1 << (Depth & 7))) != 0)
                {
                    if (FromLeaf)
                    {
                        Leaf = Buffer.B;
                        FromLeaf = false;
                    }
                    else
                    {
                        Buffer.B = Keccak.KeccakHash(Buffer.Output);
                    }
                    Branch[Depth] = Buffer.A;
                }
                else
                {
                    if (FromLeaf)
                    {
                        Leaf = Buffer.A;
                        FromLeaf = false;
                    }
                    else
                    {
                        Buffer.A = Keccak.KeccakHash(Buffer.Output);
                    }
                    Branch[Depth] = Buffer.B;
                }
            }

            // Perform final hashing
            return Keccak.KeccakHash(Buffer.Output);
        }
    }
}
