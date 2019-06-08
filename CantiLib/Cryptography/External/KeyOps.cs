//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Runtime.InteropServices;
using static Canti.Utils;

namespace Canti.Cryptography
{
    // TODO - generateRingSignatures
    // TODO - checkRingSignature
    public sealed partial class TurtleCoinCrypto : ICryptography
    {
        public string GeneratePrivateViewKeyFromPrivateSpendKey(string spendPrivateKey)
        {
            if (!IsKey(spendPrivateKey)) return null;

            IntPtr viewPrivateKey = new IntPtr();

            _generatePrivateViewKeyFromPrivateSpendKey(spendPrivateKey, ref viewPrivateKey);

            return Marshal.PtrToStringAnsi(viewPrivateKey);
        }

        public KeyPair GenerateViewKeysFromPrivateSpendKey(string spendPrivateKey)
        {
            if (!IsKey(spendPrivateKey)) return new KeyPair();

            IntPtr viewPrivateKey = new IntPtr();

            IntPtr viewPublicKey = new IntPtr();

            KeyPair viewKeys = new KeyPair();

            _generateViewKeysFromPrivateSpendKey(spendPrivateKey, ref viewPrivateKey, ref viewPublicKey);

            viewKeys.PrivateKey = Marshal.PtrToStringAnsi(viewPrivateKey);

            viewKeys.PublicKey = Marshal.PtrToStringAnsi(viewPublicKey);

            return viewKeys;
        }

        public KeyPair GenerateKeys()
        {
            IntPtr privateKey = new IntPtr();

            IntPtr publicKey = new IntPtr();

            _generateKeys(ref privateKey, ref publicKey);

            KeyPair keys = new KeyPair();

            keys.PrivateKey = Marshal.PtrToStringAnsi(privateKey);

            keys.PublicKey = Marshal.PtrToStringAnsi(publicKey);

            return keys;
        }

        public bool CheckKey(string publicKey)
        {
            if (!IsKey(publicKey)) return false;

            int success = _checkKey(publicKey);

            if (success == 1) return true;

            return false;
        }

        public string SecretKeyToPublicKey(string privateKey)
        {
            if (!IsKey(privateKey)) return null;

            IntPtr publicKey = new IntPtr();

            int success = _secretKeyToPublicKey(privateKey, ref publicKey);

            if (success == 1) return Marshal.PtrToStringAnsi(publicKey);

            return null;
        }

        public string GenerateKeyDerivation(string publicKey, string privateKey)
        {
            if (!IsKey(publicKey) || !IsKey(privateKey)) return null;

            IntPtr derivation = new IntPtr();

            int success = _generateKeyDerivation(publicKey, privateKey, ref derivation);

            if (success == 1) return Marshal.PtrToStringAnsi(derivation);

            return null;
        }

        public string DerivePublicKey(string derivation, uint outputIndex, string publicKey)
        {
            if (!IsKey(derivation) || !IsKey(publicKey)) return null;

            IntPtr derivedKey = new IntPtr();

            int success = _derivePublicKey(derivation, outputIndex, publicKey, ref derivedKey);

            if (success == 1) return Marshal.PtrToStringAnsi(derivedKey);

            return null;
        }

        public string DeriveSecretKey(string derivation, uint outputIndex, string privateKey)
        {
            if (!IsKey(derivation) || !IsKey(privateKey)) return null;

            IntPtr derivedKey = new IntPtr();

            _deriveSecretKey(derivation, outputIndex, privateKey, ref derivedKey);

            return Marshal.PtrToStringAnsi(derivedKey);
        }

        public string UnderivePublicKey(string derivation, uint outputIndex, string derivedKey)
        {
            if (!IsKey(derivation) || !IsKey(derivedKey)) return null;

            IntPtr publicKey = new IntPtr();

            int success = _underivePublicKey(derivation, outputIndex, derivedKey, ref publicKey);

            if (success == 1) return Marshal.PtrToStringAnsi(publicKey);

            return null;
        }

        public string GenerateSignature(string prefixHash, string publicKey, string privateKey)
        {
            if (!IsKey(prefixHash) || !IsKey(publicKey) || !IsKey(privateKey)) return null;

            IntPtr signature = new IntPtr();

            _generateSignature(prefixHash, publicKey, privateKey, ref signature);

            return Marshal.PtrToStringAnsi(signature);
        }

        public bool CheckSignature(string prefixHash, string publicKey, string signature)
        {
            if (!IsKey(prefixHash) || !IsKey(publicKey)) return false;

            if (!CheckKey(publicKey)) return false;

            return _checkSignature(prefixHash, publicKey, signature);
        }

        public string GenerateKeyImage(string publicKey, string privateKey)
        {
            if (!IsKey(publicKey) || !IsKey(privateKey)) return null;

            IntPtr keyImage = new IntPtr();

            _generateKeyImage(publicKey, privateKey, ref keyImage);

            return Marshal.PtrToStringAnsi(keyImage);
        }

        public string ScalarmultKey(string keyImageA, string keyImageB)
        {
            if (!IsKey(keyImageA) || !IsKey(keyImageB)) return null;

            IntPtr keyImageC = new IntPtr();

            _scalarmultKey(keyImageA, keyImageB, ref keyImageC);

            return Marshal.PtrToStringAnsi(keyImageC);
        }
    }
}
