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
using static Canti.Blockchain.Crypto.Keccak.Keccak;

namespace Canti.Blockchain.Crypto
{
    public static class KeyOps
    {
        private static EllipticCurveScalar RandomScalar()
        {
            byte[] tmp = SecureRandom.Bytes(64);
            sc_reduce(tmp);
            return new EllipticCurveScalar(tmp);
        }

        /* Generate a private + public spend, and private + public view key,
           where the spend key is derived from the view key */
        public static WalletKeys GenerateWalletKeys()
        {
            /* Generate a random public and private key for our spend keys */
            KeyPair spendKeys = GenerateKeys();
            /* Use the spend private key to derive our view keys */
            KeyPair viewKeys = GenerateDeterministicKeys(spendKeys.privateKey);
            return new WalletKeys(spendKeys, viewKeys);
        }

        /* Generate a public and private key pair */
        public static KeyPair GenerateKeys()
        {
            /* Make a random scalar */
            EllipticCurveScalar s = RandomScalar();

            /* Take the data and shove it into a private key */
            PrivateKey privateKey = new PrivateKey(s.data);

            /* Derive the public key */
            PublicKey publicKey = PrivateKeyToPublicKey(privateKey);

            return new KeyPair(publicKey, privateKey);
        }

        /* Generate a public and private key pair, using another private key
           as a seed. E.g. - derive view key from spend key */
        public static KeyPair GenerateDeterministicKeys(PrivateKey seed)
        {
            byte[] seedTmp = new byte[seed.data.Length];
            
            /* Make a copy so we don't modify the input param */
            seed.data.CopyTo(seedTmp, 0);

            /* Hash the private key with keccak */
            byte[] hashed = keccak(seedTmp);

            /* Take hash as integer and outputs the integer modulo the prime q */
            sc_reduce32(hashed);

            /* Convert into private key */
            PrivateKey privateKey = new PrivateKey(hashed);

            /* Derive the public key */
            PublicKey publicKey = PrivateKeyToPublicKey(privateKey);

            return new KeyPair(publicKey, privateKey);
        }

        public static bool AreKeysDeterministic(PrivateKey privateSpendKey,
                                                PrivateKey privateViewKey)
        {
            /* Derive the private view key */
            PrivateKey derivedPrivateViewKey
                = GenerateDeterministicKeys(privateSpendKey).privateKey;

            /* Remember `==` is used for comparing if both objects point to
               the same memory location, not equality */
            return privateViewKey.Equals(derivedPrivateViewKey);
        }

        public static PublicKey PrivateKeyToPublicKey(PrivateKey privateKey)
        {
            /* Computes aG where a is privateKey.data, and G is the ed25519
               base point, and pops the result in point */
            ED25519.ge_p3 point = new ge_p3();
            ge_scalarmult_base(point, privateKey.data);

            /* Stores the result of the derivation */
            byte[] tmp = new byte[32];

            /* Convert the point into a public key format */
            ge_p3_tobytes(tmp, point);

            return new PublicKey(tmp);
        }
    }
}
