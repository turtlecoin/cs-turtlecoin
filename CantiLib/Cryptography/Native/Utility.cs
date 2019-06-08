//
// Copyright (c) Canti, 2019 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

namespace Canti.Cryptography
{
    public sealed partial class NativeCrypto : ICryptography
    {
        public string HashToEllipticCurve(string hash)
        {
            throw new NotImplementedException();
        }

        public string ScReduce32(string input)
        {
            throw new NotImplementedException();
        }

        public string HashToScalar(string hash)
        {
            throw new NotImplementedException();
        }
    }
}
