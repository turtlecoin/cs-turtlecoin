//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System.IO;

namespace Canti.Cryptography
{
    // Provides a means of static cryptography access
    public static class Crypto
    {
        private static ICryptography _cryptoProvider;
        private static ICryptography CryptoProvider
        {
            get
            {
                if (_cryptoProvider == null)
                {
                    if (File.Exists(TurtleCoinCrypto.LIBRARY_LOCATION))
                    {
                        _cryptoProvider = new TurtleCoinCrypto();
                    }
                    else
                    {
                        _cryptoProvider = new NativeCrypto();
                    }
                }
                return _cryptoProvider;
            }
        }

        public static string CN_FastHash(string Data) => CryptoProvider.CN_FastHash(Data);
        public static string CN_FastHash(byte[] Data) => CryptoProvider.CN_FastHash(Data);
        public static string CN_SlowHashV0(string Data) => CryptoProvider.CN_SlowHashV0(Data);
        public static string CN_SlowHashV1(string Data) => CryptoProvider.CN_SlowHashV1(Data);
        public static string CN_SlowHashV2(string Data) => CryptoProvider.CN_SlowHashV2(Data);
        public static string CN_LiteSlowHashV0(string Data) => CryptoProvider.CN_LiteSlowHashV0(Data);
        public static string CN_LiteSlowHashV1(string Data) => CryptoProvider.CN_LiteSlowHashV1(Data);
        public static string CN_LiteSlowHashV2(string Data) => CryptoProvider.CN_LiteSlowHashV2(Data);
        public static string CN_DarkSlowHashV0(string Data) => CryptoProvider.CN_DarkSlowHashV0(Data);
        public static string CN_DarkSlowHashV1(string Data) => CryptoProvider.CN_DarkSlowHashV1(Data);
        public static string CN_DarkSlowHashV2(string Data) => CryptoProvider.CN_DarkSlowHashV2(Data);
        public static string CN_DarkLiteSlowHashV0(string Data) => CryptoProvider.CN_DarkLiteSlowHashV0(Data);
        public static string CN_DarkLiteSlowHashV1(string Data) => CryptoProvider.CN_DarkLiteSlowHashV1(Data);
        public static string CN_DarkLiteSlowHashV2(string Data) => CryptoProvider.CN_DarkLiteSlowHashV2(Data);
        public static string CN_TurtleSlowHashV0(string Data) => CryptoProvider.CN_TurtleSlowHashV0(Data);
        public static string CN_TurtleSlowHashV1(string Data) => CryptoProvider.CN_TurtleSlowHashV1(Data);
        public static string CN_TurtleSlowHashV2(string Data) => CryptoProvider.CN_TurtleSlowHashV2(Data);
        public static string CN_TurtleLiteSlowHashV0(string Data) => CryptoProvider.CN_TurtleLiteSlowHashV0(Data);
        public static string CN_TurtleLiteSlowHashV1(string Data) => CryptoProvider.CN_TurtleLiteSlowHashV1(Data);
        public static string CN_TurtleLiteSlowHashV2(string Data) => CryptoProvider.CN_TurtleLiteSlowHashV2(Data);
        public static string CN_SoftShellSlowHashV0(string Data, uint height) => CryptoProvider.CN_SoftShellSlowHashV0(Data, height);
        public static string CN_SoftShellSlowHashV1(string Data, uint height) => CryptoProvider.CN_SoftShellSlowHashV1(Data, height);
        public static string CN_SoftShellSlowHashV2(string Data, uint height) => CryptoProvider.CN_SoftShellSlowHashV2(Data, height);
        public static string ChukwaSlowHash(string Data) => CryptoProvider.ChukwaSlowHash(Data);
        public static string TreeHash(string[] Hashes) => CryptoProvider.TreeHash(Hashes);
        public static string TreeHashFromBranch(string[] Branch, int Depth, ref string Leaf, ref string Path) => CryptoProvider.TreeHashFromBranch(Branch, Depth, ref Leaf, ref Path);
        public static string GeneratePrivateViewKeyFromPrivateSpendKey(string spendPrivateKey) => CryptoProvider.GeneratePrivateViewKeyFromPrivateSpendKey(spendPrivateKey);
        public static KeyPair GenerateViewKeysFromPrivateSpendKey(string spendPrivateKey) => CryptoProvider.GenerateViewKeysFromPrivateSpendKey(spendPrivateKey);
        public static KeyPair GenerateKeys() => CryptoProvider.GenerateKeys();
        public static bool CheckKey(string publicKey) => CryptoProvider.CheckKey(publicKey);
        public static string SecretKeyToPublicKey(string privateKey) => CryptoProvider.SecretKeyToPublicKey(privateKey);
        public static string GenerateKeyDerivation(string publicKey, string privateKey) => CryptoProvider.GenerateKeyDerivation(publicKey, privateKey);
        public static string DerivePublicKey(string derivation, uint outputIndex, string publicKey) => CryptoProvider.DerivePublicKey(derivation, outputIndex, publicKey);
        public static string DeriveSecretKey(string derivation, uint outputIndex, string privateKey) => CryptoProvider.DeriveSecretKey(derivation, outputIndex, privateKey);
        public static string UnderivePublicKey(string derivation, uint outputIndex, string derivedKey) => CryptoProvider.UnderivePublicKey(derivation, outputIndex, derivedKey);
        public static string GenerateSignature(string prefixHash, string publicKey, string privateKey) => CryptoProvider.GenerateSignature(prefixHash, publicKey, privateKey);
        public static bool CheckSignature(string prefixHash, string publicKey, string signature) => CryptoProvider.CheckSignature(prefixHash, publicKey, signature);
        public static string GenerateKeyImage(string publicKey, string privateKey) => CryptoProvider.GenerateKeyImage(publicKey, privateKey);
        public static string ScalarmultKey(string keyImageA, string keyImageB) => CryptoProvider.ScalarmultKey(keyImageA, keyImageB);
        public static string HashToEllipticCurve(string hash) => CryptoProvider.HashToEllipticCurve(hash);
        public static string ScReduce32(string input) => CryptoProvider.ScReduce32(input);
        public static string HashToScalar(string hash) => CryptoProvider.HashToScalar(hash);
    }
}
