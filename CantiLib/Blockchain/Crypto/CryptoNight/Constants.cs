//
// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using Canti.Blockchain.Crypto.AES;

namespace Canti.Blockchain.Crypto.CryptoNight
{
    public static class Constants
    {
        public const int InitSizeBlock = 8;

        public const int InitSizeByte = InitSizeBlock * AES.Constants.BlockSize;
    }
}
