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

        public void HashToEllipticCurve(ge_p3 res, byte[] key)
        {
            ge_p2 point = new ge_p2();
            ge_p1p1 point2 = new ge_p1p1();
            byte[] h = Keccak.Keccak1600(key);
            ge_fromfe_frombytes_vartime(point, h);
            ge_mul8(point2, point);
            ge_p1p1_to_p3(res, point2);
        }

        public void HashToScalar(ref byte[] res, byte[] data)
        {
            res = Keccak.Hash(data);
            sc_reduce32(ref res);
        }

        private class SignatureBuffer
        {
            internal byte[] A;
            internal byte[] B;
            internal byte[] Output
            {
                get
                {
                    return A.AppendBytes(B);
                }
            }
            internal SignatureBuffer()
            {
                A = new byte[32];
                B = new byte[32];
            }
        };

        public string[] GenerateRingSignatures(string prefixHash, string keyImage, string[] publicKeys, string transactionSecretKey, ulong realOutput)
        {
            // Convert input variables
            byte[] PrefixHash = HexStringToByteArray(prefixHash);
            byte[] KeyImage = HexStringToByteArray(keyImage);
            byte[][] PublicKeys = new byte[publicKeys.Length][];
            for (int i = 0; i < publicKeys.Length; i++)
            {
                PublicKeys[i] = HexStringToByteArray(publicKeys[i]);
            }
            byte[] SecretKey = HexStringToByteArray(transactionSecretKey);

            // Create some ED25519 points
            ge_p3 Image_Unp = new ge_p3();
            ge_cached[] Image_Pre = new ge_cached[]
            {
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached()
            };

            // Declare a few more variables
            byte[] Sum = new byte[32];
            byte[] Scalar = new byte[32];
            byte[] Hash = new byte[32];

            // Create a signature buffer array and output array
            SignatureBuffer[] Buffer = new SignatureBuffer[PublicKeys.Length];
            SignatureBuffer[] Signatures = new SignatureBuffer[PublicKeys.Length];
            
            // Verify key image
            if (ge_frombytes_vartime(Image_Unp, KeyImage) != 0) return null;

            // Precomp
            ge_dsm_precomp(ref Image_Pre, Image_Unp);

            // Loop through all given keys
            for (ulong i = 0; i < (ulong)PublicKeys.Length; i++)
            {
                // Assign a new signature buffers
                Buffer[i] = new SignatureBuffer();
                Signatures[i] = new SignatureBuffer();

                // Create temporary points
                ge_p2 tmp2 = new ge_p2();
                ge_p3 tmp3 = new ge_p3();

                // This is the real output index
                if (i == realOutput)
                {
                    // Generate a random scalar
                    random_scalar(ref Scalar);
                    //random_scalar(k);

                    ge_scalarmult_base(tmp3, Scalar);
                    //ge_scalarmult_base(tmp3, k);

                    // Signature Part A
                    ge_p3_tobytes(ref Buffer[i].A, tmp3);
                    //ge_p3_tobytes(reinterpret_cast < unsigned char *> (&buf->ab[i].a), &tmp3);

                    // Turn public key at this index into an elliptic curve, then multiply
                    HashToEllipticCurve(tmp3, PublicKeys[i]);
                    //hash_to_ec(publicKeys[i], tmp3);

                    ge_scalarmult(tmp2, Scalar, tmp3);
                    //ge_scalarmult(&tmp2, reinterpret_cast < unsigned char *> (&k), &tmp3);

                    // Signature Part B
                    ge_tobytes(ref Buffer[i].B, tmp2);
                   // ge_tobytes(reinterpret_cast < unsigned char *> (&buf->ab[i].b), &tmp2);
                }

                // This is a generated signature index
                else
                {
                    // Create a random scalar for both signature parts
                    random_scalar(ref Signatures[i].A);
                    random_scalar(ref Signatures[i].B);

                    // Get bytes from this public key (also verifies it)
                    if (ge_frombytes_vartime(tmp3, PublicKeys[i]) != 0) return null;

                    // Multiply first part of signature
                    ge_double_scalarmult_base_vartime(tmp2, Signatures[i].A, tmp3, Signatures[i].B);

                    // Signature Part A
                    ge_tobytes(ref Buffer[i].A, tmp2);

                    // Turn public key at this index into an elliptic curve, then multiply
                    HashToEllipticCurve(tmp3, PublicKeys[i]);
                    ge_double_scalarmult_precomp_vartime(tmp2, Signatures[i].B, tmp3, Signatures[i].A, Image_Pre);

                    // Signature Part B
                    ge_tobytes(ref Buffer[i].B, tmp2);

                    // Add to sum
                    sc_add(ref Sum, Sum, Signatures[i].A);
                }
            }

            // Combine buffer and convert to a scalar
            byte[] SigHash = PrefixHash;
            for (int i = 0; i < Buffer.Length; i++)
            {
                SigHash = SigHash.AppendBytes(Buffer[i].Output);
            }
            HashToScalar(ref Hash, SigHash);

            // Perform final math
            sc_sub(ref Signatures[realOutput].A, Hash, Sum);
            sc_mulsub(ref Signatures[realOutput].B, Signatures[realOutput].A, SecretKey, Scalar);

            // Create an output array and return it
            string[] Output = new string[Signatures.Length];
            for (int i = 0; i < Signatures.Length; i++)
            {
                Output[i] = ByteArrayToHexString(Signatures[i].Output);
            }
            return Output;
        }

        public bool CheckRingSignatures(string prefixHash, string keyImage, string[] publicKeys, string[] signatures)
        {
            // Convert input variables
            byte[] PrefixHash = HexStringToByteArray(prefixHash);
            byte[] KeyImage = HexStringToByteArray(keyImage);
            byte[][] PublicKeys = new byte[publicKeys.Length][];
            for (int i = 0; i < publicKeys.Length; i++)
            {
                PublicKeys[i] = HexStringToByteArray(publicKeys[i]);
            }

            // Convert signature list to signature buffer array
            SignatureBuffer[] Signatures = new SignatureBuffer[signatures.Length];
            for (int i = 0; i < Signatures.Length; i++)
            {
                byte[] Signature = HexStringToByteArray(signatures[i]);
                Signatures[i] = new SignatureBuffer
                {
                    A = Signature.SubBytes(0, 32),
                    B = Signature.SubBytes(32, 32)
                };
            }

            // Create some ED25519 points
            ge_p3 Image_Unp = new ge_p3();
            ge_cached[] Image_Pre = new ge_cached[]
            {
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached()
            };

            // Declare a few more variables
            byte[] Sum = new byte[32];
            byte[] Hash = new byte[32];

            // Create a signature buffer array
            SignatureBuffer[] Buffer = new SignatureBuffer[PublicKeys.Length];

            // Verify key image
            if (ge_frombytes_vartime(Image_Unp, KeyImage) != 0) return false;

            // Precomp
            ge_dsm_precomp(ref Image_Pre, Image_Unp);

            // Another check
            if (ge_check_subgroup_precomp_vartime(Image_Pre) != 0) return false;

            // Loop through all given signatures
            for (int i = 0; i < PublicKeys.Length; i++)
            {
                // Assign a new signature buffer
                Buffer[i] = new SignatureBuffer();

                // Create temporary points
                ge_p2 tmp2 = new ge_p2();
                ge_p3 tmp3 = new ge_p3();

                // Verify information
                if (sc_check(Signatures[i].A) != 0 || sc_check(Signatures[i].B) != 0) return false;
                if (ge_frombytes_vartime(tmp3, PublicKeys[i]) != 0) return false;

                // Signature Part A
                ge_double_scalarmult_base_vartime(tmp2, Signatures[i].A, tmp3, Signatures[i].B);
                ge_tobytes(ref Buffer[i].A, tmp2);

                // Convert public key to elliptic curve point
                HashToEllipticCurve(tmp3, PublicKeys[i]);

                // Signature Part B
                ge_double_scalarmult_precomp_vartime(tmp2, Signatures[i].B, tmp3, Signatures[i].A, Image_Pre);
                ge_tobytes(ref Buffer[i].B, tmp2);

                // Add to sum
                sc_add(ref Sum, Sum, Signatures[i].A);
            }

            // Combine buffer and convert to a scalar
            byte[] SigHash = PrefixHash;
            for (int i = 0; i < Buffer.Length; i++)
            {
                SigHash = SigHash.AppendBytes(Buffer[i].Output);
            }
            HashToScalar(ref Hash, SigHash);

            // Perform final math
            sc_sub(ref Hash, Hash, Sum);

            // Return result of final check
            return sc_isnonzero(Hash) == 0;
        }
    }
}
