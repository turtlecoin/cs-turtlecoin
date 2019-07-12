//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using Canti.Cryptography.Native;
using Canti.Cryptography.Native.CryptoNight;
using System;
using static Canti.Cryptography.Native.ED25519;
using static Canti.Utils;

namespace Canti.Cryptography
{
    public sealed class NativeCrypto : ICryptography
    {
        #region Hashing

        public string CN_FastHash(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            return CN_FastHash(HexStringToByteArray(Data));
        }

        public string CN_FastHash(byte[] Data)
        {
            byte[] Output = Keccak.KeccakHash(Data);

            return ByteArrayToHexString(Output);
        }

        public string CN_SlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_SlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_SlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_LiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNLiteV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_LiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNLiteV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_LiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNLiteV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkLiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkLiteV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkLiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkLiteV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkLiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkLiteV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleLiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleLiteV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleLiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleLiteV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleLiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleLiteV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_SoftShellSlowHashV0(string Data, uint Height)
        {
            throw new NotImplementedException();
        }

        public string CN_SoftShellSlowHashV1(string Data, uint Height)
        {
            throw new NotImplementedException();
        }

        public string CN_SoftShellSlowHashV2(string Data, uint Height)
        {
            throw new NotImplementedException();
        }

        public string ChukwaSlowHash(string Data)
        {
            throw new NotImplementedException();
        }

        public string TreeHash(string[] Hashes)
        {
            byte[][] hs = new byte[Hashes.Length][];
            for (int i = 0; i < Hashes.Length; i++)
            {
                hs[i] = HexStringToByteArray(Hashes[i]);
            }

            byte[] output = Native.TreeHash.Hash(hs);

            return ByteArrayToHexString(output);
        }

        public string TreeHashFromBranch(string[] Branch, int Depth, ref string Leaf, ref string Path)
        {
            byte[][] b = new byte[Branch.Length][];
            for (int i = 0; i < Branch.Length; i++)
            {
                b[i] = HexStringToByteArray(Branch[i]);
            }
            byte[] l = HexStringToByteArray(Leaf);
            byte[] p = string.IsNullOrEmpty(Path) ? null : HexStringToByteArray(Path);

            byte[] output = Native.TreeHash.HashFromBranch(b, Depth, ref l, ref p);

            Leaf = ByteArrayToHexString(l);
            Path = ByteArrayToHexString(p);

            return ByteArrayToHexString(output);
        }

        #endregion

        #region KeyOps

        public string GeneratePrivateViewKeyFromPrivateSpendKey(string SpendPrivateKey)
        {
            byte[] b = HexStringToByteArray(SpendPrivateKey);
            byte[] h = Keccak.KeccakHash(b);
            sc_reduce32(ref h);
            return ByteArrayToHexString(h);
        }

        public KeyPair GenerateViewKeysFromPrivateSpendKey(string SpendPrivateKey)
        {
            byte[] b = HexStringToByteArray(SpendPrivateKey);
            byte[] h = Keccak.KeccakHash(b);
            sc_reduce32(ref h);
            KeyPair k = new KeyPair();
            k.PrivateKey = ByteArrayToHexString(h);
            k.PublicKey = SecretKeyToPublicKey(k.PrivateKey);
            return k;
        }

        public KeyPair GenerateKeys()
        {
            KeyPair k = new KeyPair();

            byte[] pbk = new byte[32];
            byte[] pvk = new byte[32];

            KeyOps.GenerateKeys(ref pbk, ref pvk);

            k.PublicKey = ByteArrayToHexString(pbk);
            k.PrivateKey = ByteArrayToHexString(pvk);

            return k;
        }

        public bool CheckKey(string PublicKey)
        {
            byte[] k = HexStringToByteArray(PublicKey);

            return KeyOps.CheckKey(k);
        }

        public string SecretKeyToPublicKey(string PrivateKey)
        {
            ge_p3 p = new ge_p3();
            ge_scalarmult_base(p, HexStringToByteArray(PrivateKey));
            byte[] b = new byte[32];
            ge_p3_tobytes(ref b, p);
            return ByteArrayToHexString(b);
        }

        public string GenerateKeyDerivation(string PublicKey, string PrivateKey)
        {
            throw new NotImplementedException();
        }

        public string DerivePublicKey(string Derivation, uint OutputIndex, string PublicKey)
        {
            throw new NotImplementedException();
        }

        public string DeriveSecretKey(string Derivation, uint OutputIndex, string PrivateKey)
        {
            throw new NotImplementedException();
        }

        public string UnderivePublicKey(string Derivation, uint OutputIndex, string DerivedKey)
        {
            throw new NotImplementedException();
        }

        public string GenerateSignature(string PrefixHash, string PublicKey, string PrivateKey)
        {
            byte[] ph = HexStringToByteArray(PrefixHash);
            byte[] pbk = HexStringToByteArray(PublicKey);
            byte[] pvk = HexStringToByteArray(PrivateKey);

            byte[] sig = KeyOps.GenerateSignature(ph, pbk, pvk);

            return ByteArrayToHexString(sig);
        }

        public bool CheckSignature(string PrefixHash, string PublicKey, string Signature)
        {
            byte[] ph = HexStringToByteArray(PrefixHash);
            byte[] pbk = HexStringToByteArray(PublicKey);
            byte[] sig = HexStringToByteArray(Signature);

            return KeyOps.CheckSignature(ph, pbk, sig);
        }

        public string GenerateKeyImage(string PublicKey, string PrivateKey)
        {
            byte[] pbk = HexStringToByteArray(PublicKey);
            byte[] pvk = HexStringToByteArray(PrivateKey);

            byte[] output = KeyOps.GenerateKeyImage(pbk, pvk);

            return ByteArrayToHexString(output);
        }

        public string ScalarmultKey(string KeyImageA, string KeyImageB)
        {
            byte[] kia = HexStringToByteArray(KeyImageA);
            byte[] kib = HexStringToByteArray(KeyImageB);

            byte[] output = KeyOps.ScalarmultKey(kia, kib);

            return ByteArrayToHexString(output);
        }

        public string[] GenerateRingSignatures(string PrefixHash, string KeyImage, string[] PublicKeys, string TransactionSecretKey, ulong realOutput)
        {
            byte[] ph = HexStringToByteArray(PrefixHash);
            byte[] ki = HexStringToByteArray(KeyImage);
            byte[][] pks = new byte[PublicKeys.Length][];
            for (int i = 0; i < PublicKeys.Length; i++)
            {
                pks[i] = HexStringToByteArray(PublicKeys[i]);
            }
            byte[] tsk = HexStringToByteArray(TransactionSecretKey);

            byte[][] sigs = KeyOps.GenerateRingSignatures(ph, ki, pks, tsk, realOutput);

            string[] output = new string[sigs.Length];
            for (int i = 0; i < sigs.Length; i++)
            {
                output[i] = ByteArrayToHexString(sigs[i]);
            }

            return output;
        }

        public bool CheckRingSignatures(string PrefixHash, string KeyImage, string[] PublicKeys, string[] Signatures)
        {
            byte[] ph = HexStringToByteArray(PrefixHash);
            byte[] ki = HexStringToByteArray(KeyImage);
            byte[][] pks = new byte[PublicKeys.Length][];
            for (int i = 0; i < PublicKeys.Length; i++)
            {
                pks[i] = HexStringToByteArray(PublicKeys[i]);
            }
            byte[][] sigs = new byte[Signatures.Length][];
            for (int i = 0; i < Signatures.Length; i++)
            {
                sigs[i] = HexStringToByteArray(Signatures[i]);
            }

            return KeyOps.CheckRingSignatures(ph, ki, pks, sigs);
        }

        public string HashToEllipticCurve(string Hash)
        {
            if (!IsKey(Hash)) return null;

            byte[] tmp = HexStringToByteArray(Hash);

            ge_p3 tmp3 = new ge_p3();

            KeyOps.HashToEllipticCurve(tmp3, tmp);

            byte[] output = new byte[32];
            ED25519.ge_p3_tobytes(ref output, tmp3);

            return ByteArrayToHexString(output);
        }

        public string ScReduce32(string Input)
        {
            if (!IsKey(Input)) return null;

            byte[] tmp = HexStringToByteArray(Input);

            ED25519.sc_reduce32(ref tmp);

            return ByteArrayToHexString(tmp);
        }

        public string HashToScalar(string Hash)
        {
            if (Hash.Length % 2 != 0) return null;

            byte[] tmp = HexStringToByteArray(Hash);

            KeyOps.HashToScalar(ref tmp, tmp);

            return ByteArrayToHexString(tmp);
        }

        #endregion
    }
}
