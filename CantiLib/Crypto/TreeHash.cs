//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

namespace Canti
{
    // TODO - tree_branch
    // TODO - tree_hash_from_branch
    public sealed partial class Crypto
    {
        // Performs a tree hash on a list of hashes to get one root hash
        public static string TreeHash(string[] Hashes)
        {
            // Empty hashes array
            if (Hashes.Length == 0) return null;

            // Only one hash, return that hash
            else if (Hashes.Length == 1)
            {
                return Hashes[0];
            }

            // More than one hash, perform tree hash
            else
            {
                // Loop until hash array length is just 1, that will be our final value
                Start:
                // Calculate a new size
                var Size = (Hashes.Length % 2 == 0) ? Hashes.Length / 2 : Hashes.Length / 2  + 1;

                // Perform hashing
                for (var i = 0; i < Size; i++)
                {
                    var val = i * 2;
                    if (i == Size - 1) Hashes[i] = CN_FastHash(Hashes[val]);
                    else Hashes[i] = CN_FastHash(Hashes[val] + Hashes[val + 1]);
                }

                // Resize hashes array
                if (Size == 1) return Hashes[0];
                Array.Resize(ref Hashes, Size);
                goto Start;
            }
        }
    }
}
