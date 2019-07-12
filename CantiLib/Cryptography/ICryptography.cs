//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

namespace Canti.Cryptography
{
    // Interfaces cryptographic functions so we can choose to use native c# or pinvoked methods
    public interface ICryptography
    {
        #region Hashing

        string CN_FastHash(string Data);
        string CN_FastHash(byte[] Data);
        string CN_SlowHashV0(string Data);
        string CN_SlowHashV1(string Data);
        string CN_SlowHashV2(string Data);
        string CN_LiteSlowHashV0(string Data);
        string CN_LiteSlowHashV1(string Data);
        string CN_LiteSlowHashV2(string Data);
        string CN_DarkSlowHashV0(string Data);
        string CN_DarkSlowHashV1(string Data);
        string CN_DarkSlowHashV2(string Data);
        string CN_DarkLiteSlowHashV0(string Data);
        string CN_DarkLiteSlowHashV1(string Data);
        string CN_DarkLiteSlowHashV2(string Data);
        string CN_TurtleSlowHashV0(string Data);
        string CN_TurtleSlowHashV1(string Data);
        string CN_TurtleSlowHashV2(string Data);
        string CN_TurtleLiteSlowHashV0(string Data);
        string CN_TurtleLiteSlowHashV1(string Data);
        string CN_TurtleLiteSlowHashV2(string Data);
        string CN_SoftShellSlowHashV0(string Data, uint height);
        string CN_SoftShellSlowHashV1(string Data, uint height);
        string CN_SoftShellSlowHashV2(string Data, uint height);
        string ChukwaSlowHash(string Data);
        string TreeHash(string[] Hashes);
        string TreeHashFromBranch(string[] Branch, int Depth, ref string Leaf, ref string Path);

        #endregion

        #region KeyOps

        string GeneratePrivateViewKeyFromPrivateSpendKey(string spendPrivateKey);
        KeyPair GenerateViewKeysFromPrivateSpendKey(string spendPrivateKey);
        KeyPair GenerateKeys();
        bool CheckKey(string publicKey);
        string SecretKeyToPublicKey(string privateKey);
        string GenerateKeyDerivation(string publicKey, string privateKey);
        string DerivePublicKey(string derivation, uint outputIndex, string publicKey);
        string DeriveSecretKey(string derivation, uint outputIndex, string privateKey);
        string UnderivePublicKey(string derivation, uint outputIndex, string derivedKey);
        string GenerateSignature(string prefixHash, string publicKey, string privateKey);
        bool CheckSignature(string prefixHash, string publicKey, string signature);
        string[] GenerateRingSignatures(string PrefixHash, string KeyImage, string[] PublicKeys, string TransactionSecretKey, ulong realOutput);
        public bool CheckRingSignatures(string PrefixHash, string KeyImage, string[] PublicKeys, string[] Signatures);
        string GenerateKeyImage(string publicKey, string privateKey);
        string ScalarmultKey(string keyImageA, string keyImageB);

        #endregion

        #region Utility

        string HashToEllipticCurve(string hash);
        string ScReduce32(string input);
        string HashToScalar(string hash);

        #endregion
    }
}
