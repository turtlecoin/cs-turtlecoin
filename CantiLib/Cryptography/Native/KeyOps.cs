//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

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
            throw new NotImplementedException();
        }

        public bool CheckSignature(string prefixHash, string publicKey, string signature)
        {
            throw new NotImplementedException();
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
