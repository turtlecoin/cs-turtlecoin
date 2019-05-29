/* 
 * ---------------------------------------------------------------------------
 * OpenAES License
 * ---------------------------------------------------------------------------
 * Copyright (c) 2012, Nabil S. Al Ramli, www.nalramli.com
 * Copyright (c) 2018, The TurtleCoin Developers
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   - Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   - Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * ---------------------------------------------------------------------------
 */

/*
 * ---------------------------------------------------------------------------
 * Copyright (c) 1998-2013, Brian Gladman, Worcester, UK. All rights reserved.
 * Copyright (c) 2018, The TurtleCoin Developers. All rights reserved.
 * 
 * The redistribution and use of this software (with or without changes)
 * is allowed without the payment of fees or royalties provided that:
 * 
 *   source code distributions include the above copyright notice, this
 *   list of conditions and the following disclaimer;
 * 
 *   binary distributions include the above copyright notice, this list
 *   of conditions and the following disclaimer in their documentation.
 * 
 * This software is provided 'as is' with no explicit or implied warranties
 * in respect of its operation, including, but not limited to, correctness
 * and fitness for purpose.
 * ---------------------------------------------------------------------------
 * Issue Date: 20/12/2007
 */

using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Canti.Blockchain.Crypto.AES
{
    public static class AES
    {
        public static void AESBSingleRound(byte[] keys, byte[] input,
                                           int inputOffset)
        {
            uint[] b0 = new uint[4];
            uint[] b1 = new uint[4];

            /* Copy 16 bytes from input[inputOffset] to b0 (4 uints) */
            Buffer.BlockCopy(input, inputOffset, b0, 0, 16);

            uint[] keysAsUint = new uint[keys.Length / 4];

            /* Possibly do this with a pointer instead for speed? */
            Buffer.BlockCopy(keys, 0, keysAsUint, 0, keys.Length);

            Round(b1, b0, keysAsUint, 0);

            /* Copy 16 bytes from b1 to input[inputOffset] (4 uints) */
            Buffer.BlockCopy(b1, 0, input, inputOffset, 16);
        }

        private static void AESPseudoRoundNative(byte[] keys, byte[] input)
        {
            Vector128<byte> d;

            Vector128<byte>[] roundKeys = new Vector128<byte>[10];

            for (int i = 0; i < 10; i++)
            {
                roundKeys[i] = Vector128.Create(
                    keys[i * 16 + 0],
                    keys[i * 16 + 1],
                    keys[i * 16 + 2],
                    keys[i * 16 + 3],
                    keys[i * 16 + 4],
                    keys[i * 16 + 5],
                    keys[i * 16 + 6],
                    keys[i * 16 + 7],
                    keys[i * 16 + 8],
                    keys[i * 16 + 9],
                    keys[i * 16 + 10],
                    keys[i * 16 + 11],
                    keys[i * 16 + 12],
                    keys[i * 16 + 13],
                    keys[i * 16 + 14],
                    keys[i * 16 + 15]
                );
            }

            unsafe
            {
                fixed(byte* keyPtr = input)
                {
                    for (int i = 0; i < CryptoNight.Constants.InitSizeBlock; i++)
                    {
                        d = Sse2.LoadVector128(keyPtr + (i * Constants.BlockSize));

                        d = Aes.Encrypt(d, roundKeys[0]);
                        d = Aes.Encrypt(d, roundKeys[1]);
                        d = Aes.Encrypt(d, roundKeys[2]);
                        d = Aes.Encrypt(d, roundKeys[3]);
                        d = Aes.Encrypt(d, roundKeys[4]);
                        d = Aes.Encrypt(d, roundKeys[5]);
                        d = Aes.Encrypt(d, roundKeys[6]);
                        d = Aes.Encrypt(d, roundKeys[7]);
                        d = Aes.Encrypt(d, roundKeys[8]);
                        d = Aes.Encrypt(d, roundKeys[9]);

                        Sse2.Store(keyPtr + (i * Constants.BlockSize), d);
                    }
                }
            }
        }

        public static void AESPseudoRound(byte[] keys, byte[] input, bool enableIntrinsics = true)
        {
            if (Sse2.IsSupported && Aes.IsSupported && enableIntrinsics)
            {
                AESPseudoRoundNative(keys, input);
            }
            else
            {
                for (int i = 0; i < CryptoNight.Constants.InitSizeBlock; i++)
                {
                    /* Need to pass the array with an offset because we manip
                       it in place */
                    AESBPseudoRound(keys, input, i * Constants.BlockSize);
                }
            }
        }

        public static void AESBPseudoRound(byte[] keys, byte[] input,
                                           int inputOffset)
        {
            uint[] b0 = new uint[4];
            uint[] b1 = new uint[4];

            /* Copy 16 bytes from input[inputOffset] to b0 (4 uints) */
            Buffer.BlockCopy(input, inputOffset, b0, 0, 16);

            uint[] keysAsUint = new uint[keys.Length / 4];

            /* Possibly do this with a pointer instead for speed? */
            Buffer.BlockCopy(keys, 0, keysAsUint, 0, keys.Length);

            /* Manually unrolling for MOARR SPEEEEED */
            Round(b1, b0, keysAsUint, 0);
            Round(b0, b1, keysAsUint, 4);
            Round(b1, b0, keysAsUint, 8);
            Round(b0, b1, keysAsUint, 12);
            Round(b1, b0, keysAsUint, 16);
            Round(b0, b1, keysAsUint, 20);
            Round(b1, b0, keysAsUint, 24);
            Round(b0, b1, keysAsUint, 28);
            Round(b1, b0, keysAsUint, 32);
            Round(b0, b1, keysAsUint, 36);

            /* Copy 16 bytes from b0 to input[inputOffset] (4 uints) */
            Buffer.BlockCopy(b0, 0, input, inputOffset, 16);
        }

        private static void AssignVectorToArray<T>(Vector128<T> vec, T[] arr, int offset = 0) where T : struct
        {
            for (int i = 0; i < 16; i++)
            {
                arr[i + offset] = vec.GetElement(i);
            }
        }

        private static void Aes256Assist1(ref Vector128<byte> t1, ref Vector128<byte> t2)
        {
            Vector128<byte> t4;
            t2 = Sse2.Shuffle(t2.AsInt32(), 0xff).AsByte();
            
            /* This operation corresponds to _mm_bslli_si128, not _mm_slli_si128
               which is used in the original slow hash code. However, the
               two descriptions in the intel manual appear to be exactly the
               same: https://software.intel.com/sites/landingpage/IntrinsicsGuide/#text=_mm_slli_si128&expand=5236,3954,5288,5315,5315&techs=SSE2
                     https://software.intel.com/sites/landingpage/IntrinsicsGuide/#text=_mm_bslli_si128&expand=5236,3954,5288,5315,5315,586&techs=SSE2 */
            t4 = Sse2.ShiftLeftLogical128BitLane(t1, 0x04);
            t1 = Sse2.Xor(t1, t4);
            t4 = Sse2.ShiftLeftLogical128BitLane(t4, 0x04);
            t1 = Sse2.Xor(t1, t4);
            t4 = Sse2.ShiftLeftLogical128BitLane(t4, 0x04);
            t1 = Sse2.Xor(t1, t4);
            t1 = Sse2.Xor(t1, t2);
        }

        private static void Aes256Assist2(ref Vector128<byte> t1, ref Vector128<byte> t3)
        {
            Vector128<byte> t2, t4;

            t4 = Aes.KeygenAssist(t1, 0x00);
            t2 = Sse2.Shuffle(t4.AsInt32(), 0xaa).AsByte();
            t4 = Sse2.ShiftLeftLogical128BitLane(t3, 0x04);
            t3 = Sse2.Xor(t3, t4);
            t4 = Sse2.ShiftLeftLogical128BitLane(t4, 0x04);
            t3 = Sse2.Xor(t3, t4);
            t4 = Sse2.ShiftLeftLogical128BitLane(t4, 0x04);
            t3 = Sse2.Xor(t3, t4);
            t3 = Sse2.Xor(t3, t2);
        }

        /* https://github.com/turtlecoin/turtlecoin/blob/60f1dd1360503d606636185edf8dbf4eb1b2ace8/src/crypto/slow-hash-x86.c#L252 */
        private static byte[] ExpandKeyNative(byte[] key)
        {
            int keyBase = key.Length / Constants.RoundKeyLength;
            int numKeys = keyBase + Constants.RoundBase;

            int expandedKeyLength = numKeys * Constants.RoundKeyLength
                                            * Constants.ColumnLength;

            byte[] expandedKey = new byte[240];

            Vector128<byte> t1;
            Vector128<byte> t2;
            Vector128<byte> t3;

            unsafe
            {
                fixed(byte* keyPtr = key)
                {
                    t1 = Sse2.LoadVector128(keyPtr);
                    t3 = Sse2.LoadVector128(keyPtr + 16);
                }
            }

            AssignVectorToArray(t1, expandedKey, 0);
            AssignVectorToArray(t3, expandedKey, 16);

            t2 = Aes.KeygenAssist(t3, 0x01);
            Aes256Assist1(ref t1, ref t2);
            AssignVectorToArray(t1, expandedKey, 32);
            Aes256Assist2(ref t1, ref t3);
            AssignVectorToArray(t3, expandedKey, 48);

            t2 = Aes.KeygenAssist(t3, 0x02);
            Aes256Assist1(ref t1, ref t2);
            AssignVectorToArray(t1, expandedKey, 64);
            Aes256Assist2(ref t1, ref t3);
            AssignVectorToArray(t3, expandedKey, 80);

            t2 = Aes.KeygenAssist(t3, 0x04);
            Aes256Assist1(ref t1, ref t2);
            AssignVectorToArray(t1, expandedKey, 96);
            Aes256Assist2(ref t1, ref t3);
            AssignVectorToArray(t3, expandedKey, 112);

            t2 = Aes.KeygenAssist(t3, 0x08);
            Aes256Assist1(ref t1, ref t2);
            AssignVectorToArray(t1, expandedKey, 128);
            Aes256Assist2(ref t1, ref t3);
            AssignVectorToArray(t3, expandedKey, 144);

            t2 = Aes.KeygenAssist(t3, 0x10);
            Aes256Assist1(ref t1, ref t2);
            AssignVectorToArray(t1, expandedKey, 160);

            return expandedKey;
        }

        public static byte[] ExpandKey(byte[] key, bool enableIntrinsics = true)
        {
            if (Sse2.IsSupported && Aes.IsSupported && enableIntrinsics)
            {
                return ExpandKeyNative(key);
            }

            int keyBase = key.Length / Constants.RoundKeyLength;
            int numKeys = keyBase + Constants.RoundBase;

            int expandedKeyLength = numKeys * Constants.RoundKeyLength
                                            * Constants.ColumnLength;

            byte[] expanded = new byte[expandedKeyLength];

            /* First key is a direct copy of input key */
            Buffer.BlockCopy(key, 0, expanded, 0, key.Length);

            /* Apply expand key algorithm for remaining keys */
            for (int i = keyBase; i < numKeys * Constants.RoundKeyLength; i++)
            {
                byte[] tmp = new byte[Constants.ColumnLength];

                /* Copy column length bytes from expanded to tmp, with an
                   offset of (i - 1) * RoundKeyLength from expanded. */
                Buffer.BlockCopy(expanded, (i - 1) * Constants.RoundKeyLength,
                                 tmp, 0, Constants.ColumnLength);

                if (i % keyBase == 0)
                {
                    tmp = RotLeft(tmp);

                    for (int j = 0; j < Constants.ColumnLength; j++)
                    {
                        tmp[j] = SubByte(tmp[j]);
                    }

                    tmp[0] = (byte)(tmp[0] ^ Constants.Gf8[(i / keyBase) - 1]);
                }
                else if (keyBase > 6 && i % keyBase == 4)
                {
                    for (int j = 0; j < Constants.ColumnLength; j++)
                    {
                        tmp[j] = SubByte(tmp[j]);
                    }
                }

                for (int j = 0; j < Constants.ColumnLength; j++)
                {
                    int index = ((i - keyBase) * Constants.RoundKeyLength) + j;

                    expanded[(i * Constants.RoundKeyLength) + j]
                        = (byte)(expanded[index] ^ tmp[j]);
                }
            }

            return expanded;
        }

        /* This can of course be done with a loop and utilizing FourTables()
         * below, but we need all the speed we can get, so are manually
         * unrolling it. */
        private static void Round(uint[] y, uint[] x, uint[] key, int keyOffset)
        {
            y[0] = key[keyOffset] ^ FourTablesA(x);
            y[1] = key[keyOffset + 1] ^ FourTablesB(x);
            y[2] = key[keyOffset + 2] ^ FourTablesC(x);
            y[3] = key[keyOffset + 3] ^ FourTablesD(x);
        }

        /* FourTablesA, B, C, D, are just this with the i variable prefilled in
         * for a tad more speed.
         * 
        private static uint FourTables(uint[] x, uint i)
        {
            return Constants.SbData[0][BVal(x[(0 + i) % 4], 0)] ^
            Constants.SbData[1][BVal(x[(1 + i) % 4], 1)] ^
            Constants.SbData[2][BVal(x[(2 + i) % 4], 2)] ^
            Constants.SbData[3][BVal(x[(3 + i) % 4], 3)];
        }
        */

        private static uint FourTablesA(uint[] x)
        {
            return Constants.SbData[0][BVal(x[0], 0)] ^
            Constants.SbData[1][BVal(x[1], 1)] ^
            Constants.SbData[2][BVal(x[2], 2)] ^
            Constants.SbData[3][BVal(x[3], 3)];
        }

        private static uint FourTablesB(uint[] x)
        {
            return Constants.SbData[0][BVal(x[1], 0)] ^
            Constants.SbData[1][BVal(x[2], 1)] ^
            Constants.SbData[2][BVal(x[3], 2)] ^
            Constants.SbData[3][BVal(x[0], 3)];
        }

        private static uint FourTablesC(uint[] x)
        {
            return Constants.SbData[0][BVal(x[2], 0)] ^
            Constants.SbData[1][BVal(x[3], 1)] ^
            Constants.SbData[2][BVal(x[0], 2)] ^
            Constants.SbData[3][BVal(x[1], 3)];
        }

        private static uint FourTablesD(uint[] x)
        {
            return Constants.SbData[0][BVal(x[3], 0)] ^
            Constants.SbData[1][BVal(x[0], 1)] ^
            Constants.SbData[2][BVal(x[1], 2)] ^
            Constants.SbData[3][BVal(x[2], 3)];
        }

        private static uint ForwardVar(uint[] x, uint r, uint i)
        {
            return x[(r + i) % 4];
        }

        private static uint BVal(uint x, uint n)
        {
            return (uint)(((ulong)x >> (int)(8 * n)) & 0xff);
        }

        private static byte SubByte(byte input)
        {
            byte x = input;
            byte y = input;

            x &= 0x0f;
            y &= 0xf0;

            y >>= 4;

            return Constants.SubByteValue[y, x];
        }

        /* Rotate array one step left, e.g. [1, 2, 3, 4] becomes [2, 3, 4, 1]
           Assumes input is Constants.ColumnLength long 

           I strongly suspect this won't work well with arrays that aren't 4 bytes long..
                    
           Old routine took 00:01:09 for 100000000 iterations
           New routine took 00:00:85 for 100000000 iterations
         */
        private static byte[] RotLeft(byte[] input)
        {
            // Constants.ColumnLength = 4
            return new byte[] { input[1], input[2], input[3], input[0] };
        }
    }
}
