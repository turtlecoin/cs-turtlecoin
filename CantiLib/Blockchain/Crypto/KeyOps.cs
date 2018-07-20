//
// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Linq;
using Canti.Blockchain.Crypto.ED25519;

/* This lets us do ge_scalarmult_base instead of
   ED25519.ED25519.ge_scalarmult_base */
using static Canti.Blockchain.Crypto.ED25519.ED25519;

namespace Canti.Blockchain.Crypto
{
    internal static class KeyOps
    {
        private static EllipticCurveScalar RandomScalar()
        {
            byte[] tmp = SecureRandom.Bytes(64);
            sc_reduce(tmp);
            return new EllipticCurveScalar(tmp);
        }

        /* Generate a public and private key pair */
        public static KeyPair GenerateKeys()
        {
            /* Make a random scalar */
            EllipticCurveScalar s = RandomScalar();

            /* Take the data and shove it into a private key */
            PrivateKey privateKey = new PrivateKey(s.data);

            /* Computes aG where a is privateKey.data, and G is the ed25519
               base point, and pops the result in point */
            ED25519.ge_p3 point = new ge_p3();
            ge_scalarmult_base(point, privateKey.data);

            byte[] tmp = new byte[32];

            /* Convert the point into a public key format */
            ge_p3_tobytes(tmp, point);
            PublicKey publicKey = new PublicKey(tmp);

            return new KeyPair(publicKey, privateKey);
        }

        /* Generate a public and private key pair, using another private key
           as a seed. E.g. - derive view key from spend key */
        public static KeyPair GenerateDeterministicKeys(PrivateKey seed)
        {
            /* Take seed as integer and outputs the integer modulo the prime q */
            sc_reduce32(seed.data);

            /* Convert into private key */
            PrivateKey privateKey = new PrivateKey(seed.data);

            /* Computes aG where a is privateKey.data, and G is the ed25519
               base point, and pops the result in point */
            ED25519.ge_p3 point = new ge_p3();
            ge_scalarmult_base(point, privateKey.data);

            byte[] tmp = new byte[32];

            /* Convert the point into a public key format */
            ge_p3_tobytes(tmp, point);
            PublicKey publicKey = new PublicKey(tmp);

            return new KeyPair(publicKey, privateKey);
        }
    }
}
