//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Runtime.InteropServices;

namespace Canti.Cryptography
{
    public sealed partial class TurtleCoinCrypto
    {
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

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _tree_hash([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]string[] hashes, ulong hashesLength, ref IntPtr output);
        
        [DllImport(LIBRARY_LOCATION)]
        private static extern void _tree_hash_from_branch([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]string[] branches, ulong branchesLength, ulong depth, [MarshalAs(UnmanagedType.LPStr)]ref string leaf, [MarshalAs(UnmanagedType.LPStr)]ref string path, ref IntPtr output);

        #endregion

        #region KeyOps

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
        private static extern void _generateRingSignatures([MarshalAs(UnmanagedType.LPStr)]string prefixHash, [MarshalAs(UnmanagedType.LPStr)]string keyImage, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]string[] publicKeys, ulong publicKeysCount, [MarshalAs(UnmanagedType.LPStr)]string transactionSecretKey, ulong realOutputIndex,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]ref IntPtr[] signatures, ref ulong signaturesCount);
        /*
        [DllImport(LIBRARY_LOCATION)]
        static bool checkRingSignature(
                const std::string prefixHash,
                const std::string keyImage,
                const std::vector<std::string> publicKeys,
                const std::vector<std::string> signatures
            );
        */

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _generateKeyImage([MarshalAs(UnmanagedType.LPStr)]string publicKey, [MarshalAs(UnmanagedType.LPStr)]string privateKey, ref IntPtr keyImage);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _scalarmultKey([MarshalAs(UnmanagedType.LPStr)]string keyImageA, [MarshalAs(UnmanagedType.LPStr)]string keyImageB, ref IntPtr keyImageC);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _hashToEllipticCurve([MarshalAs(UnmanagedType.LPStr)]string hash, ref IntPtr ec);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _scReduce32([MarshalAs(UnmanagedType.LPStr)]string input, ref IntPtr output);

        [DllImport(LIBRARY_LOCATION)]
        private static extern void _hashToScalar([MarshalAs(UnmanagedType.LPStr)]string hash, ref IntPtr scalar);

        #endregion
    }
}
