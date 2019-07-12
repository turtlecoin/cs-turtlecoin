//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using static Canti.Utils;

namespace Canti.Cryptography
{
    public sealed partial class TurtleCoinCrypto : ICryptography
    {
        #region Hashing

        #region Fast Hash

        public string CN_FastHash(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_fast_hash(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }
        public string CN_FastHash(byte[] Data)
        {
            return CN_FastHash(ByteArrayToHexString(Data));
        }

        #endregion

        #region Slow Hash

        public string CN_SlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_SlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_SlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Lite Slow Hash

        public string CN_LiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_lite_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_LiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_lite_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_LiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_lite_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Dark Slow Hash

        public string CN_DarkSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_dark_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_DarkSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_DarkSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Dark Lite Slow Hash

        public string CN_DarkLiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_dark_lite_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_DarkLiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_lite_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_DarkLiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_lite_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Turtle Slow Hash

        public string CN_TurtleSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_TurtleSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_TurtleSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Turtle Lite Slow Hash

        public string CN_TurtleLiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_lite_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_TurtleLiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_lite_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_TurtleLiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_lite_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Soft Shell Slow Hash

        public string CN_SoftShellSlowHashV0(string Data, uint height)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_soft_shell_slow_hash_v0(Data, height, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_SoftShellSlowHashV1(string Data, uint height)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_soft_shell_slow_hash_v1(Data, height, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_SoftShellSlowHashV2(string Data, uint height)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_soft_shell_slow_hash_v2(Data, height, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Chukwa Slow Hash

        public string ChukwaSlowHash(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _chukwa_slow_hash(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Tree Hash

        public string TreeHash(string[] Hashes)
        {
            IntPtr output = new IntPtr();

            _tree_hash(Hashes, (ulong)Hashes.Length, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        // TODO - needs testing
        public string TreeHashFromBranch(string[] Branch, int Depth, ref string Leaf, ref string Path)
        {
            IntPtr output = new IntPtr();

            _tree_hash_from_branch(Branch, (ulong)Branch.Length, (ulong)Depth, ref Leaf, ref Path, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #endregion

        #region KeyOps

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

        public string[] GenerateRingSignatures(string PrefixHash, string KeyImage, string[] PublicKeys, string TransactionSecretKey, ulong realOutput)
        {
            IntPtr[] output = new IntPtr[PublicKeys.Length];
            for (int i = 0; i < PublicKeys.Length; i++) output[i] = new IntPtr();

            ulong Count = 0;

            _generateRingSignatures(PrefixHash, KeyImage, PublicKeys, (ulong)PublicKeys.Length, TransactionSecretKey, realOutput, ref output, ref Count);


            return output.Select(x => Marshal.PtrToStringAnsi(x)).ToArray();
            //return output;//new string[0];// output.Select(x => Marshal.PtrToStringAnsi(x)).ToArray();
        }

        public bool CheckRingSignatures(string PrefixHash, string KeyImage, string[] PublicKeys, string[] Signatures)
        {
            throw new NotImplementedException();
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

        #endregion
    }
}
