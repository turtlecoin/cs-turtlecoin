//
// Copyright (c) Canti, 2019 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using Canti.Cryptography.Native;
using System;
using static Canti.Cryptography.Native.ED25519;
using static Canti.Utils;

namespace Canti.Cryptography
{
    public sealed partial class NativeCrypto : ICryptography
    {
        public string HashToEllipticCurve(string hash)
        {
            if (!IsKey(hash)) return null;

            throw new NotImplementedException();
        }

        public string ScReduce32(string input)
        {
            if (!IsKey(input)) return null;

            byte[] tmp = HexStringToByteArray(input);

            sc_reduce32(ref tmp);

            return ByteArrayToHexString(tmp);
        }

        public string HashToScalar(string hash)
        {
            if (hash.Length % 2 != 0) return null;

            byte[] tmp = Keccak.Hash(HexStringToByteArray(hash));

            sc_reduce32(ref tmp);

            return ByteArrayToHexString(tmp);
        }
    }
}
