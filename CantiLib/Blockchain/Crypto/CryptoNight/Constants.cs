// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

namespace Canti.Blockchain.Crypto.CryptoNight
{
    public static class Constants
    {
        public const int InitSizeBlock = 8;
        public const int AesBlockSize = 16;
        public const int AesKeySize = 32;

        public const int InitSizeByte = InitSizeBlock * AesBlockSize;

        public const int Memory = 2097152; /* 2 ^ 21 */
        public const int Iterations = 1048576; /* 2 ^ 20 */

        public const int CNIterations = Memory / InitSizeByte;
    }
}
