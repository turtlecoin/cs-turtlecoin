//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
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
        public string GeneratePrivateViewKeyFromPrivateSpendKey(string spendPrivateKey)
        {
            throw new NotImplementedException();
        }

        public KeyPair GenerateViewKeysFromPrivateSpendKey(string spendPrivateKey)
        {
            throw new NotImplementedException();
        }

        public KeyPair GenerateKeys()
        {
            throw new NotImplementedException();
        }

        public bool CheckKey(string publicKey)
        {
            throw new NotImplementedException();
        }

        public string SecretKeyToPublicKey(string privateKey)
        {
            throw new NotImplementedException();
        }

        public string GenerateKeyDerivation(string publicKey, string privateKey)
        {
            throw new NotImplementedException();
        }

        public string DerivePublicKey(string derivation, uint outputIndex, string publicKey)
        {
            throw new NotImplementedException();
        }

        public string DeriveSecretKey(string derivation, uint outputIndex, string privateKey)
        {
            throw new NotImplementedException();
        }

        public string UnderivePublicKey(string derivation, uint outputIndex, string derivedKey)
        {
            throw new NotImplementedException();
        }

        public string GenerateSignature(string prefixHash, string publicKey, string privateKey)
        {
            ge_p3 Point = new ge_p3();
            byte[] Hash = HexStringToByteArray(prefixHash);
            byte[] PubKey = HexStringToByteArray(publicKey);
            byte[] Comm = new byte[32];
            byte[] SecKey = HexStringToByteArray(privateKey);

            ge_p3 TestPoint = new ge_p3();
            if (sc_check(SecKey) != 0)
            {
                // Invalid secret key
                return null;
            }

            ge_scalarmult_base(TestPoint, SecKey);
            byte[] Derived = new byte[32];
            ge_p3_tobytes(ref Derived, TestPoint);
            if (!PubKey.Matches(Derived))
            {
                // Invalid public key
                return null;
            }

            byte[] Scalar = new byte[32];
            random_scalar(ref Scalar);

            ge_scalarmult_base(Point, Scalar);
            ge_p3_tobytes(ref Comm, Point);

            byte[] tmp = Keccak.Keccak1600(Hash.AppendBytes(PubKey).AppendBytes(Comm));
            sc_reduce32(ref tmp);

            byte[] Sig1 = tmp.SubBytes(0, 32);
            byte[] Sig2 = tmp.SubBytes(32, 32);
            sc_mulsub(ref Sig2, Sig1, SecKey, Scalar);

            return ByteArrayToHexString(Sig1.AppendBytes(Sig2));
        }

        public bool CheckSignature(string prefixHash, string publicKey, string signature)
        {
            ge_p2 tmp2 = new ge_p2();
            ge_p3 tmp3 = new ge_p3();
            byte[] Hash = HexStringToByteArray(prefixHash);
            byte[] PubKey = HexStringToByteArray(publicKey);
            byte[] Comm = new byte[32];
            byte[] Sig = HexStringToByteArray(signature);

            if (ge_frombytes_vartime(tmp3, PubKey) != 0) return false;
            if (sc_check(Sig.SubBytes(0, 32)) != 0 || sc_check(Sig.SubBytes(32, 32)) != 0) return false;

            ge_double_scalarmult_base_vartime(tmp2, Sig.SubBytes(0, 32), tmp3, Sig.SubBytes(32, 32));
            ge_tobytes(ref Comm, tmp2);

            byte[] tmp = Keccak.Hash(Hash.AppendBytes(PubKey).AppendBytes(Comm));
            sc_reduce32(ref tmp);

            sc_sub(ref tmp, tmp, Sig);

            return sc_isnonzero(tmp) == 0;
        }

        public string GenerateKeyImage(string publicKey, string privateKey)
        {
            throw new NotImplementedException();
        }

        public string ScalarmultKey(string keyImageA, string keyImageB)
        {
            throw new NotImplementedException();
        }
    }
}
