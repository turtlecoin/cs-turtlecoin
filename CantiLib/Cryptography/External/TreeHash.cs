//
// Copyright 2012-2013 The CryptoNote Developers
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using static Canti.Utils;

// TODO - Use external tree hash, this is currently copied from C# implementation
namespace Canti.Cryptography
{
    // Tree hash functions
    /*public sealed partial class TurtleCoinCrypto : ICryptography
    {
        /// <summary>
        /// Hashes a set of hashes into an asymmetric tree hash
        /// </summary>
        /// <param name="Hashes">An array of hashes to tree hash</param>
        /// <returns>A 32 byte hash of the input hash array</returns>
        public string TreeHash(string[] Hashes)
        {
            // Empty hashes array
            if (Hashes.Length == 0) return null;

            // Loop through and verify that each input entry is a valid hash
            for (var x = 0; x < Hashes.Length; x++)
            {
                if (!IsKey(Hashes[x])) return null;
            }

            // If input length is 1, we don't need to perform any hashing
            if (Hashes.Length == 1) return Hashes[0];

            // If input length is 2, hash only those two values
            if (Hashes.Length == 2) return CN_FastHash(Hashes[0] + Hashes[1]);

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
            string[] Buffer = new string[Count];
            for (InputIndex = 0; InputIndex < val; InputIndex++)
            {
                Buffer[InputIndex] = Hashes[InputIndex];
            }

            // Perform first round of hashing
            for (InputIndex = val, OutputIndex = val; OutputIndex < Count; InputIndex += 2, OutputIndex++)
            {
                Buffer[OutputIndex] = CN_FastHash(Hashes[InputIndex] + Hashes[InputIndex + 1]);
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
                    Buffer[OutputIndex] = CN_FastHash(Buffer[InputIndex] + Buffer[InputIndex + 1]);
                }
            }

            // Perform final hash
            return CN_FastHash(Buffer[0] + Buffer[1]);
        }

        // TODO - Write a public summary for this method
        public string TreeHashFromBranch(string[] Branch, int Depth, ref string Leaf, ref string Path)
        {
            // If depth is 0, the leaf hash is our root
            if (Depth == 0)
            {
                return Leaf;
            }

            // Declare some variables
            string[] Buffer = new string[2];
            bool FromLeaf = true;

            // Loop until we are at root depth
            while (Depth > 0)
            {
                // Decrement depth
                Depth--;

                // Assign path values
                if (!string.IsNullOrEmpty(Path) && (Path[Depth >> 3] & (1 << (Depth & 7))) != 0)
                {
                    if (FromLeaf)
                    {
                        Leaf = Buffer[1];
                        FromLeaf = false;
                    }
                    else
                    {
                        Buffer[1] = CN_FastHash(Buffer[0] + Buffer[1]);
                    }
                    Branch[Depth] = Buffer[0];
                }
                else
                {
                    if (FromLeaf)
                    {
                        Leaf = Buffer[0];
                        FromLeaf = false;
                    }
                    else
                    {
                        Buffer[0] = CN_FastHash(Buffer[0] + Buffer[1]);
                    }
                    Branch[Depth] = Buffer[1];
                }
            }

            // Perform final hashing
            return CN_FastHash(Buffer[0] + Buffer[1]);
        }
    }*/
}
