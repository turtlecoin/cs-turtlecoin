//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using static Canti.Cryptography.Native.ED25519;

namespace Canti.Cryptography.Native
{
    public static class KeyOps
    {
        /*public string GeneratePrivateViewKeyFromPrivateSpendKey(string spendPrivateKey)
        {
            throw new NotImplementedException();
        }

        public KeyPair GenerateViewKeysFromPrivateSpendKey(string spendPrivateKey)
        {
            throw new NotImplementedException();
        }*/

        public static void GenerateKeys(ref byte[] PublicKey, ref byte[] PrivateKey)
        {
            ge_p3 point = new ge_p3();
            HashBuffer k = new HashBuffer();
            random_scalar(ref PrivateKey);
            ge_scalarmult_base(point, PrivateKey);
            ge_p3_tobytes(ref PublicKey, point);
        }

        public static bool CheckKey(byte[] Key)
        {
            ge_p3 point = new ge_p3();
            return ge_frombytes_vartime(point, Key) == 0;
        }

        /*public string SecretKeyToPublicKey(string privateKey)
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
        }*/

        public static byte[] GenerateSignature(byte[] PrefixHash, byte[] PublicKey, byte[] PrivateKey)
        {
            // Create some ED25519 points
            ge_p3 Point = new ge_p3();
            ge_p3 TestPoint = new ge_p3();

            // Declare a few more variables
            byte[] Comm = new byte[32];
            byte[] Scalar = new byte[32];
            byte[] SigBytes = new byte[64];

            // Verify private key
            if (sc_check(PrivateKey) != 0)
            {
                // Invalid secret key
                return null;
            }

            // Verify public key
            ge_scalarmult_base(TestPoint, PrivateKey);
            byte[] Derived = new byte[32];
            ge_p3_tobytes(ref Derived, TestPoint);
            if (!PublicKey.Matches(Derived))
            {
                // Invalid public key
                return null;
            }

            // Get a random scalar
            random_scalar(ref Scalar);

            // Convert scalar to bytes
            ge_scalarmult_base(Point, Scalar);
            ge_p3_tobytes(ref Comm, Point);

            // Combine hashes and get another scalar
            HashToScalar(ref SigBytes, PrefixHash.AppendBytes(PublicKey).AppendBytes(Comm));
            HashBuffer Signature = new HashBuffer()
            {
                A = SigBytes.SubBytes(0, 32),
                B = SigBytes.SubBytes(32, 32)
            };

            // Perform final math
            sc_mulsub(ref Signature.B, Signature.A, PrivateKey, Scalar);

            // Return output signature
            return Signature.Output;
        }

        public static bool CheckSignature(byte[] PrefixHash, byte[] PublicKey, byte[] Signature)
        {
            // Create some ED25519 points
            ge_p2 tmp2 = new ge_p2();
            ge_p3 tmp3 = new ge_p3();

            // Declare a few more variables
            byte[] Comm = new byte[32];
            byte[] SigBytes = new byte[64];

            // Verify public key
            if (ge_frombytes_vartime(tmp3, PublicKey) != 0) return false;

            // Create a signature buffer from the given signature
            HashBuffer Buffer = new HashBuffer(Signature);

            // Verify signature bytes
            if (sc_check(Buffer.A) != 0 || sc_check(Buffer.B) != 0) return false;

            // Signature Part A
            ge_double_scalarmult_base_vartime(tmp2, Buffer.A, tmp3, Buffer.B);
            ge_tobytes(ref Comm, tmp2);

            // Combine hashes and convert to a scalar
            HashToScalar(ref SigBytes, PrefixHash.AppendBytes(PublicKey).AppendBytes(Comm));

            // Perform final math
            sc_sub(ref SigBytes, SigBytes, Signature);

            // Return result
            return sc_isnonzero(SigBytes) == 0;
        }

        public static byte[] GenerateKeyImage(byte[] PublicKey, byte[] PrivateKey)
        {
            ge_p3 point = new ge_p3();
            ge_p2 point2 = new ge_p2();
            if (sc_check(PrivateKey) != 0) return null;
            HashToEllipticCurve(point, PublicKey);
            ge_scalarmult(point2, PrivateKey, point);
            byte[] image = new byte[32];
            ge_tobytes(ref image, point2);
            return image;
        }

        public static byte[] ScalarmultKey(byte[] KeyImageA, byte[] KeyImageB)
        {
            ge_p3 A = new ge_p3();
            ge_p2 R = new ge_p2();
            ge_frombytes_vartime(A, KeyImageA);
            ge_scalarmult(R, KeyImageB, A);
            byte[] aP = new byte[32];
            ge_tobytes(ref aP, R);
            return aP;
        }

        public static byte[][] GenerateRingSignatures(byte[] PrefixHash, byte[] KeyImage, byte[][] PublicKeys, byte[] TransactionSecretKey, ulong RealOutput)
        {
            // Create some ED25519 points
            ge_p3 Image_Unp = new ge_p3();
            ge_dsmp Image_Pre = new ge_dsmp();

            // Declare a few more variables
            byte[] Sum = new byte[32];
            byte[] Scalar = new byte[32];
            byte[] Hash = new byte[32];

            // Create a signature buffer array and output array
            HashBuffer[] Buffer = new HashBuffer[PublicKeys.Length];
            HashBuffer[] Signatures = new HashBuffer[PublicKeys.Length];
            
            // Verify key image
            if (ge_frombytes_vartime(Image_Unp, KeyImage) != 0) return null;

            // Precomp
            ge_dsm_precomp(ref Image_Pre, Image_Unp);

            // Loop through all given keys
            for (ulong i = 0; i < (ulong)PublicKeys.Length; i++)
            {
                // Assign a new signature buffers
                Buffer[i] = new HashBuffer();
                Signatures[i] = new HashBuffer();

                // Create temporary points
                ge_p2 tmp2 = new ge_p2();
                ge_p3 tmp3 = new ge_p3();

                // This is the real output index
                if (i == RealOutput)
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
            }

            // Combine buffer and convert to a scalar
            byte[] SigHash = PrefixHash;
            for (int i = 0; i < Buffer.Length; i++)
            {
                SigHash = SigHash.AppendBytes(Buffer[i].Output);
            }
            HashToScalar(ref Hash, SigHash);

            // Perform final math
            sc_sub(ref Signatures[RealOutput].A, Hash, Sum);
            sc_mulsub(ref Signatures[RealOutput].B, Signatures[RealOutput].A, TransactionSecretKey, Scalar);

            // Create an output array and return it
            byte[][] Output = new byte[Signatures.Length][];
            for (int i = 0; i < Signatures.Length; i++)
            {
                Output[i] = Signatures[i].Output;
            }
            return Output;
        }

        public static bool CheckRingSignatures(byte[] PrefixHash, byte[] KeyImage, byte[][] PublicKeys, byte[][] Signatures)
        {
            // Convert signature list to signature buffer array
            HashBuffer[] SignaturesBuffer = new HashBuffer[Signatures.Length];
            for (int i = 0; i < SignaturesBuffer.Length; i++)
            {
                SignaturesBuffer[i] = new HashBuffer(Signatures[i]);
            }

            // Create some ED25519 points
            ge_p3 Image_Unp = new ge_p3();
            ge_dsmp Image_Pre = new ge_dsmp();

            // Declare a few more variables
            byte[] Sum = new byte[32];
            byte[] Hash = new byte[32];

            // Create a signature buffer array
            HashBuffer[] Buffer = new HashBuffer[PublicKeys.Length];

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
                Buffer[i] = new HashBuffer();

                // Create temporary points
                ge_p2 tmp2 = new ge_p2();
                ge_p3 tmp3 = new ge_p3();

                // Verify information
                if (sc_check(SignaturesBuffer[i].A) != 0 || sc_check(SignaturesBuffer[i].B) != 0) return false;
                if (ge_frombytes_vartime(tmp3, PublicKeys[i]) != 0) return false;

                // Signature Part A
                ge_double_scalarmult_base_vartime(tmp2, SignaturesBuffer[i].A, tmp3, SignaturesBuffer[i].B);
                ge_tobytes(ref Buffer[i].A, tmp2);

                // Convert public key to elliptic curve point
                HashToEllipticCurve(tmp3, PublicKeys[i]);

                // Signature Part B
                ge_double_scalarmult_precomp_vartime(tmp2, SignaturesBuffer[i].B, tmp3, SignaturesBuffer[i].A, Image_Pre);
                ge_tobytes(ref Buffer[i].B, tmp2);

                // Add to sum
                sc_add(ref Sum, Sum, SignaturesBuffer[i].A);
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

        public static void HashToEllipticCurve(ge_p3 res, byte[] key)
        {
            ge_p2 point = new ge_p2();
            ge_p1p1 point2 = new ge_p1p1();
            byte[] h = Keccak.Keccak1600(key);
            ge_fromfe_frombytes_vartime(point, h);
            ge_mul8(point2, point);
            ge_p1p1_to_p3(res, point2);
        }

        public static void HashToScalar(ref byte[] res, byte[] data)
        {
            res = Keccak.Keccak1600(data);
            sc_reduce32(ref res);
        }
    }
}
