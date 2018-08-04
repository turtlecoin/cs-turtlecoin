// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using Canti.Blockchain.Crypto.Skein.SkeinFish;

namespace Canti.Blockchain.Crypto.Skein
{
    public static class Skein
    {
        public static byte[] skein(byte[] input)
        {
            Skein256 s = new Skein256();
            return s.ComputeHash(input);
        }
    }
}
