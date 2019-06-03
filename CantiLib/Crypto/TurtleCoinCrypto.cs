//
// Copyright (c) 2019 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Runtime.InteropServices;

namespace Canti
{
    // TODO - generateRingSignatures
    // TODO - checkRingSignature
    /// <summary>
    /// TurtleCoin cryptography functions
    /// </summary>
    public sealed partial class Crypto
    {
        #region Structs

        /// <summary>
        /// ED25519 keypair
        /// </summary>
        public struct KeyPair
        {
            public string PrivateKey;
            public string PublicKey;
        }

        #endregion

        #region Constants

        // Minimum variation bytes required for certain hash functions
        private const int MINIMUM_VARIATION_BYTES = 43 * 2;

        // Location of file
        private const string LIBRARY_LOCATION = "turtlecoin-crypto-shared.dll";

        #endregion

        #region Methods

        #region Externs

        #region Hashing

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_fast_hash([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_slow_hash_v0([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_slow_hash_v1([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_slow_hash_v2([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_lite_slow_hash_v0([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_lite_slow_hash_v1([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_lite_slow_hash_v2([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_dark_slow_hash_v0([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_dark_slow_hash_v1([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_dark_slow_hash_v2([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_dark_lite_slow_hash_v0([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_dark_lite_slow_hash_v1([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_dark_lite_slow_hash_v2([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_turtle_slow_hash_v0([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_turtle_slow_hash_v1([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_turtle_slow_hash_v2([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_turtle_lite_slow_hash_v0([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_turtle_lite_slow_hash_v1([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_turtle_lite_slow_hash_v2([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_soft_shell_slow_hash_v0([MarshalAs(UnmanagedType.LPStr)]string input, uint height, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_soft_shell_slow_hash_v1([MarshalAs(UnmanagedType.LPStr)]string input, uint height, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _cn_soft_shell_slow_hash_v2([MarshalAs(UnmanagedType.LPStr)]string input, uint height, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _chukwa_slow_hash([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        #endregion

        #region Keys and Signatures

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _generatePrivateViewKeyFromPrivateSpendKey([MarshalAs(UnmanagedType.LPStr)]string spendPrivateKey, ref IntPtr viewPrivateKey);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _generateViewKeysFromPrivateSpendKey([MarshalAs(UnmanagedType.LPStr)]string spendPrivateKey, ref IntPtr viewPrivateKey, ref IntPtr viewPublicKey);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _generateKeys(ref IntPtr privateKey, ref IntPtr publicKey);

        [DllImport(LIBRARY_LOCATION)]
        private static extern int _checkKey([MarshalAs(UnmanagedType.LPStr)]string publicKey);

        [DllImport(LIBRARY_LOCATION)]
        private static extern int _secretKeyToPublicKey([MarshalAs(UnmanagedType.LPStr)]string privateKey, ref IntPtr publicKey);

        [DllImport(LIBRARY_LOCATION)]
        private static extern int _generateKeyDerivation([MarshalAs(UnmanagedType.LPStr)]string publicKey, [MarshalAs(UnmanagedType.LPStr)]string privateKey, ref IntPtr derivation);

        [DllImport(LIBRARY_LOCATION)]
        private static extern int _derivePublicKey([MarshalAs(UnmanagedType.LPStr)]string derivation, uint outputIndex, [MarshalAs(UnmanagedType.LPStr)]string publicKey, ref IntPtr derivedKey);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _deriveSecretKey([MarshalAs(UnmanagedType.LPStr)]string derivation, uint outputIndex, [MarshalAs(UnmanagedType.LPStr)]string privateKey, ref IntPtr derivedKey);

        [DllImport(LIBRARY_LOCATION)]
        private static extern int _underivePublicKey([MarshalAs(UnmanagedType.LPStr)]string derivation, uint outputIndex, [MarshalAs(UnmanagedType.LPStr)]string derivedKey, ref IntPtr publicKey);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _generateSignature([MarshalAs(UnmanagedType.LPStr)]string prefixHash, [MarshalAs(UnmanagedType.LPStr)]string privateKey, [MarshalAs(UnmanagedType.LPStr)]string publicKey, ref IntPtr signature);

        [DllImport(LIBRARY_LOCATION)]
        private static extern bool _checkSignature([MarshalAs(UnmanagedType.LPStr)]string prefixHash, [MarshalAs(UnmanagedType.LPStr)]string publicKey, [MarshalAs(UnmanagedType.LPStr)]string signature);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _generateKeyImage([MarshalAs(UnmanagedType.LPStr)]string publicKey, [MarshalAs(UnmanagedType.LPStr)]string privateKey, ref IntPtr keyImage);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _scalarmultKey([MarshalAs(UnmanagedType.LPStr)]string keyImageA, [MarshalAs(UnmanagedType.LPStr)]string keyImageB, ref IntPtr keyImageC);

        #endregion

        #region Conversion and Utility

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _hashToEllipticCurve([MarshalAs(UnmanagedType.LPStr)]string hash, ref IntPtr ec);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _scReduce32([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _hashToScalar([MarshalAs(UnmanagedType.LPStr)]string hash, ref IntPtr scalar);

        #endregion

        #endregion

        #region Private

        private static bool IsKey(string key)
        {
            if (key.Length % 2 == 0 && key.Length == 64) return true;

            return false;
        }

        #endregion

        #region Public

        #region Hashing

        public static string CN_FastHash(string data)
        {
            if (data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_fast_hash(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_SlowHashV0(string data)
        {
            if (data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_slow_hash_v0(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_SlowHashV1(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_slow_hash_v1(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_SlowHashV2(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_slow_hash_v2(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_LiteSlowHashV0(string data)
        {
            if (data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_lite_slow_hash_v0(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_LiteSlowHashV1(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_lite_slow_hash_v1(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_LiteSlowHashV2(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_lite_slow_hash_v2(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_DarkSlowHashV0(string data)
        {
            if (data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_dark_slow_hash_v0(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_DarkSlowHashV1(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_slow_hash_v1(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_DarkSlowHashV2(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_slow_hash_v2(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_DarkLiteSlowHashV0(string data)
        {
            if (data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_dark_lite_slow_hash_v0(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_DarkLiteSlowHashV1(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_lite_slow_hash_v1(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_DarkLiteSlowHashV2(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_lite_slow_hash_v2(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_TurtleSlowHashV0(string data)
        {
            if (data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_slow_hash_v0(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_TurtleSlowHashV1(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_slow_hash_v1(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_TurtleSlowHashV2(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_slow_hash_v2(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_TurtleLiteSlowHashV0(string data)
        {
            if (data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_lite_slow_hash_v0(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_TurtleLiteSlowHashV1(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_lite_slow_hash_v1(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_TurtleLiteSlowHashV2(string data)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_lite_slow_hash_v2(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_SoftShellSlowHashV0(string data, uint height)
        {
            if (data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_soft_shell_slow_hash_v0(data, height, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_SoftShellSlowHashV1(string data, uint height)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_soft_shell_slow_hash_v1(data, height, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string CN_SoftShellSlowHashV2(string data, uint height)
        {
            if (data.Length % 2 != 0 || data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_soft_shell_slow_hash_v2(data, height, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string ChukwaSlowHash(string data)
        {
            if (data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _chukwa_slow_hash(data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Keys and Signatures

        public static string GeneratePrivateViewKeyFromPrivateSpendKey(string spendPrivateKey)
        {
            if (!IsKey(spendPrivateKey)) return null;

            IntPtr viewPrivateKey = new IntPtr();

            _generatePrivateViewKeyFromPrivateSpendKey(spendPrivateKey, ref viewPrivateKey);

            return Marshal.PtrToStringAnsi(viewPrivateKey);
        }

        public static KeyPair GenerateViewKeysFromPrivateSpendKey(string spendPrivateKey)
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

        public static KeyPair GenerateKeys()
        {
            IntPtr privateKey = new IntPtr();

            IntPtr publicKey = new IntPtr();

            _generateKeys(ref privateKey, ref publicKey);

            KeyPair keys = new KeyPair();

            keys.PrivateKey = Marshal.PtrToStringAnsi(privateKey);

            keys.PublicKey = Marshal.PtrToStringAnsi(publicKey);

            return keys;
        }

        public static bool CheckKey(string publicKey)
        {
            if (!IsKey(publicKey)) return false;

            int success = _checkKey(publicKey);

            if (success == 1) return true;

            return false;
        }

        public static string SecretKeyToPublicKey(string privateKey)
        {
            if (!IsKey(privateKey)) return null;

            IntPtr publicKey = new IntPtr();

            int success = _secretKeyToPublicKey(privateKey, ref publicKey);

            if (success == 1) return Marshal.PtrToStringAnsi(publicKey);

            return null;
        }

        public static string GenerateKeyDerivation(string publicKey, string privateKey)
        {
            if (!IsKey(publicKey) || !IsKey(privateKey)) return null;

            IntPtr derivation = new IntPtr();

            int success = _generateKeyDerivation(publicKey, privateKey, ref derivation);

            if (success == 1) return Marshal.PtrToStringAnsi(derivation);

            return null;
        }

        public static string DerivePublicKey(string derivation, uint outputIndex, string publicKey)
        {
            if (!IsKey(derivation) || !IsKey(publicKey)) return null;

            IntPtr derivedKey = new IntPtr();

            int success = _derivePublicKey(derivation, outputIndex, publicKey, ref derivedKey);

            if (success == 1) return Marshal.PtrToStringAnsi(derivedKey);

            return null;
        }

        public static string DeriveSecretKey(string derivation, uint outputIndex, string privateKey)
        {
            if (!IsKey(derivation) || !IsKey(privateKey)) return null;

            IntPtr derivedKey = new IntPtr();

            _deriveSecretKey(derivation, outputIndex, privateKey, ref derivedKey);

            return Marshal.PtrToStringAnsi(derivedKey);
        }

        public static string UnderivePublicKey(string derivation, uint outputIndex, string derivedKey)
        {
            if (!IsKey(derivation) || !IsKey(derivedKey)) return null;

            IntPtr publicKey = new IntPtr();

            int success = _underivePublicKey(derivation, outputIndex, derivedKey, ref publicKey);

            if (success == 1) return Marshal.PtrToStringAnsi(publicKey);

            return null;
        }

        public static string GenerateSignature(string prefixHash, string publicKey, string privateKey)
        {
            if (!IsKey(prefixHash) || !IsKey(publicKey) || !IsKey(privateKey)) return null;

            IntPtr signature = new IntPtr();

            _generateSignature(prefixHash, publicKey, privateKey, ref signature);

            return Marshal.PtrToStringAnsi(signature);
        }

        public static bool CheckSignature(string prefixHash, string publicKey, string signature)
        {
            if (!IsKey(prefixHash) || !IsKey(publicKey)) return false;

            if (!CheckKey(publicKey)) return false;

            return _checkSignature(prefixHash, publicKey, signature);
        }

        public static string GenerateKeyImage(string publicKey, string privateKey)
        {
            if (!IsKey(publicKey) || !IsKey(privateKey)) return null;

            IntPtr keyImage = new IntPtr();

            _generateKeyImage(publicKey, privateKey, ref keyImage);

            return Marshal.PtrToStringAnsi(keyImage);
        }

        public static string ScalarmultKey(string keyImageA, string keyImageB)
        {
            if (!IsKey(keyImageA) || !IsKey(keyImageB)) return null;

            IntPtr keyImageC = new IntPtr();

            _scalarmultKey(keyImageA, keyImageB, ref keyImageC);

            return Marshal.PtrToStringAnsi(keyImageC);
        }

        #endregion

        #region Conversion and Utility

        public static string HashToEllipticCurve(string hash)
        {
            if (!IsKey(hash)) return null;

            IntPtr ec = new IntPtr();

            _hashToEllipticCurve(hash, ref ec);

            return Marshal.PtrToStringAnsi(ec);
        }

        public static string ScReduce32(string input)
        {
            if (!IsKey(input)) return null;

            IntPtr output = new IntPtr();

            _scReduce32(input, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public static string HashToScalar(string hash)
        {
            if (!IsKey(hash)) return null;

            IntPtr scalar = new IntPtr();

            _hashToScalar(hash, ref scalar);

            return Marshal.PtrToStringAnsi(scalar);
        }

        #endregion

        #endregion

        #endregion
    }
}
