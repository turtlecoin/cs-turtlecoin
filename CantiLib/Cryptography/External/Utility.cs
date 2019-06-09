//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Runtime.InteropServices;
using static Canti.Utils;

namespace Canti.Cryptography
{
    public sealed partial class TurtleCoinCrypto : ICryptography
    {
        public string HashToEllipticCurve(string hash)
        {
            if (!IsKey(hash)) return null;

            IntPtr ec = new IntPtr();

            _hashToEllipticCurve(hash, ref ec);

            return Marshal.PtrToStringAnsi(ec);
        }

        public string ScReduce32(string input)
        {
            if (!IsKey(input)) return null;

            IntPtr output = new IntPtr();

            _scReduce32(input, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string HashToScalar(string hash)
        {
            if (hash.Length % 2 != 0) return null;

            IntPtr scalar = new IntPtr();

            _hashToScalar(hash, ref scalar);

            return Marshal.PtrToStringAnsi(scalar);
        }
    }
}
