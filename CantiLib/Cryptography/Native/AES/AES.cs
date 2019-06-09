/* 
 * ---------------------------------------------------------------------------
 * OpenAES License
 * ---------------------------------------------------------------------------
 * Copyright (c) 2012, Nabil S. Al Ramli, www.nalramli.com
 * Copyright (c) 2018-2019, The TurtleCoin Developers
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
 * Copyright (c) 2018-2019, The TurtleCoin Developers. All rights reserved.
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

namespace Canti.Cryptography.Native
{
    public static partial class AES
    {
        public static unsafe void AESPseudoRoundXOR(
            byte[] keys,
            byte[] input,
            byte[] xor,
            int offset)
        {
            Vector128<byte>[] roundKeys = new Vector128<byte>[10];

            fixed(byte* roundKeyPtr = keys)
            {
                roundKeys[0] = Sse2.LoadVector128(roundKeyPtr + 0);
                roundKeys[1] = Sse2.LoadVector128(roundKeyPtr + 16);
                roundKeys[2] = Sse2.LoadVector128(roundKeyPtr + 32);
                roundKeys[3] = Sse2.LoadVector128(roundKeyPtr + 48);
                roundKeys[4] = Sse2.LoadVector128(roundKeyPtr + 64);
                roundKeys[5] = Sse2.LoadVector128(roundKeyPtr + 80);
                roundKeys[6] = Sse2.LoadVector128(roundKeyPtr + 96);
                roundKeys[7] = Sse2.LoadVector128(roundKeyPtr + 112);
                roundKeys[8] = Sse2.LoadVector128(roundKeyPtr + 128);
                roundKeys[9] = Sse2.LoadVector128(roundKeyPtr + 144);
            }

            fixed(byte* keyPtr = input, xorPtr = xor)
            {
                Vector128<byte> d;

                int xorOffset = offset;

                for (int i = 0; i < CryptoNight.Constants.INIT_SIZE_BLOCK; i++)
                {
                    d = Sse2.LoadVector128(keyPtr + (i * BLOCK_SIZE));

                    d = Sse2.Xor(d, Sse2.LoadVector128(xorPtr + xorOffset));

                    /* Increase offset by 128 bits (16 bytes) */
                    xorOffset += 16;

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

                    Sse2.Store(keyPtr + (i * BLOCK_SIZE), d);
                }
            }
        }

        public static unsafe void AESPseudoRoundNative(byte[] keys, byte[] input)
        {
            Vector128<byte>[] roundKeys = new Vector128<byte>[10];

            fixed(byte* roundKeyPtr = keys)
            {
                roundKeys[0] = Sse2.LoadVector128(roundKeyPtr + 0);
                roundKeys[1] = Sse2.LoadVector128(roundKeyPtr + 16);
                roundKeys[2] = Sse2.LoadVector128(roundKeyPtr + 32);
                roundKeys[3] = Sse2.LoadVector128(roundKeyPtr + 48);
                roundKeys[4] = Sse2.LoadVector128(roundKeyPtr + 64);
                roundKeys[5] = Sse2.LoadVector128(roundKeyPtr + 80);
                roundKeys[6] = Sse2.LoadVector128(roundKeyPtr + 96);
                roundKeys[7] = Sse2.LoadVector128(roundKeyPtr + 112);
                roundKeys[8] = Sse2.LoadVector128(roundKeyPtr + 128);
                roundKeys[9] = Sse2.LoadVector128(roundKeyPtr + 144);
            }

            fixed(byte* keyPtr = input)
            {
                Vector128<byte> d;

                for (int i = 0; i < CryptoNight.Constants.INIT_SIZE_BLOCK; i++)
                {
                    d = Sse2.LoadVector128(keyPtr + (i * BLOCK_SIZE));

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

                    Sse2.Store(keyPtr + (i * BLOCK_SIZE), d);
                }
            }
        }

        private static void Aes256Assist1(ref Vector128<byte> t1, ref Vector128<byte> t2)
        {
            Vector128<byte> t4;

            t2 = Sse2.Shuffle(t2.AsInt32(), 0xff).AsByte();
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

        private static unsafe byte[] ExpandKeyNative(byte[] key)
        {
            byte[] expandedKey = new byte[240];

            fixed(byte* keyPtr = key, expandedKeyPtr = expandedKey)
            {
                Vector128<byte> t1;
                Vector128<byte> t2;
                Vector128<byte> t3;

                t1 = Sse2.LoadVector128(keyPtr);
                t3 = Sse2.LoadVector128(keyPtr + 16);

                Sse2.Store(expandedKeyPtr, t1);
                Sse2.Store(expandedKeyPtr + 16, t3);

                t2 = Aes.KeygenAssist(t3, 0x01);
                Aes256Assist1(ref t1, ref t2);
                Sse2.Store(expandedKeyPtr + 32, t1);
                Aes256Assist2(ref t1, ref t3);
                Sse2.Store(expandedKeyPtr + 48, t3);

                t2 = Aes.KeygenAssist(t3, 0x02);
                Aes256Assist1(ref t1, ref t2);
                Sse2.Store(expandedKeyPtr + 64, t1);
                Aes256Assist2(ref t1, ref t3);
                Sse2.Store(expandedKeyPtr + 80, t3);

                t2 = Aes.KeygenAssist(t3, 0x04);
                Aes256Assist1(ref t1, ref t2);
                Sse2.Store(expandedKeyPtr + 96, t1);
                Aes256Assist2(ref t1, ref t3);
                Sse2.Store(expandedKeyPtr + 112, t3);

                t2 = Aes.KeygenAssist(t3, 0x08);
                Aes256Assist1(ref t1, ref t2);
                Sse2.Store(expandedKeyPtr + 128, t1);
                Aes256Assist2(ref t1, ref t3);
                Sse2.Store(expandedKeyPtr + 144, t3);

                t2 = Aes.KeygenAssist(t3, 0x10);
                Aes256Assist1(ref t1, ref t2);
                Sse2.Store(expandedKeyPtr + 160, t1);

                return expandedKey;
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
                for (int i = 0; i < CryptoNight.Constants.INIT_SIZE_BLOCK; i++)
                {
                    /* Need to pass the array with an offset because we manip
                       it in place */
                    AESBPseudoRound(keys, input, i * BLOCK_SIZE);
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

        private static unsafe void Round(uint* y, uint* x, uint* key, int keyOffset)
        {
            y[0] = key[keyOffset] ^ FourTablesA(x);
            y[1] = key[keyOffset + 1] ^ FourTablesB(x);
            y[2] = key[keyOffset + 2] ^ FourTablesC(x);
            y[3] = key[keyOffset + 3] ^ FourTablesD(x);
        }

        private static unsafe uint FourTablesA(uint* x)
        {
            return SB_DATA[0][BVal(x[0], 0)] ^ SB_DATA[1][BVal(x[1], 1)] ^
                   SB_DATA[2][BVal(x[2], 2)] ^ SB_DATA[3][BVal(x[3], 3)];
        }

        private static unsafe uint FourTablesB(uint* x)
        {
            return SB_DATA[0][BVal(x[1], 0)] ^ SB_DATA[1][BVal(x[2], 1)] ^
                   SB_DATA[2][BVal(x[3], 2)] ^ SB_DATA[3][BVal(x[0], 3)];
        }

        private static unsafe uint FourTablesC(uint* x)
        {
            return SB_DATA[0][BVal(x[2], 0)] ^ SB_DATA[1][BVal(x[3], 1)] ^
                   SB_DATA[2][BVal(x[0], 2)] ^ SB_DATA[3][BVal(x[1], 3)];
        }

        private static unsafe uint FourTablesD(uint* x)
        {
            return SB_DATA[0][BVal(x[3], 0)] ^ SB_DATA[1][BVal(x[0], 1)] ^
                   SB_DATA[2][BVal(x[1], 2)] ^ SB_DATA[3][BVal(x[2], 3)];
        }

        public static unsafe void AESBSingleRound(
            uint *input, uint* keys)
        {
            uint[] output = new uint[4];

            fixed (uint *outputPtr = output)
            {
                Round(outputPtr, input, keys, 0);
                Buffer.MemoryCopy(outputPtr, input, 16, 16);
            }
        }

        public static byte[] ExpandKey(byte[] key, bool enableIntrinsics = true)
        {
            if (Sse2.IsSupported && Aes.IsSupported && enableIntrinsics)
            {
                return ExpandKeyNative(key);
            }

            int keyBase = key.Length / ROUND_KEY_LENGTH;
            int numKeys = keyBase + ROUND_BASE;

            int expandedKeyLength = numKeys * ROUND_KEY_LENGTH * COLUMN_LENGTH;

            byte[] expanded = new byte[expandedKeyLength];

            /* First key is a direct copy of input key */
            Buffer.BlockCopy(key, 0, expanded, 0, key.Length);

            /* Apply expand key algorithm for remaining keys */
            for (int i = keyBase; i < numKeys * ROUND_KEY_LENGTH; i++)
            {
                byte[] tmp = new byte[COLUMN_LENGTH];

                /* Copy column length bytes from expanded to tmp, with an
                   offset of (i - 1) * RoundKeyLength from expanded. */
                Buffer.BlockCopy(expanded, (i - 1) * ROUND_KEY_LENGTH,
                                 tmp, 0, COLUMN_LENGTH);

                if (i % keyBase == 0)
                {
                    tmp = RotLeft(tmp);

                    for (int j = 0; j < COLUMN_LENGTH; j++)
                    {
                        tmp[j] = SubByte(tmp[j]);
                    }

                    tmp[0] = (byte)(tmp[0] ^ GF8[(i / keyBase) - 1]);
                }
                else if (keyBase > 6 && i % keyBase == 4)
                {
                    for (int j = 0; j < COLUMN_LENGTH; j++)
                    {
                        tmp[j] = SubByte(tmp[j]);
                    }
                }

                for (int j = 0; j < COLUMN_LENGTH; j++)
                {
                    int index = ((i - keyBase) * ROUND_KEY_LENGTH) + j;

                    expanded[(i * ROUND_KEY_LENGTH) + j]
                        = (byte)(expanded[index] ^ tmp[j]);
                }
            }

            return expanded;
        }

        private static void Round(uint[] y, uint[] x, uint[] key, int keyOffset)
        {
            y[0] = key[keyOffset] ^ FourTablesA(x);
            y[1] = key[keyOffset + 1] ^ FourTablesB(x);
            y[2] = key[keyOffset + 2] ^ FourTablesC(x);
            y[3] = key[keyOffset + 3] ^ FourTablesD(x);
        }

        private static uint FourTablesA(uint[] x)
        {
            return SB_DATA[0][BVal(x[0], 0)] ^ SB_DATA[1][BVal(x[1], 1)] ^
                   SB_DATA[2][BVal(x[2], 2)] ^ SB_DATA[3][BVal(x[3], 3)];
        }

        private static uint FourTablesB(uint[] x)
        {
            return SB_DATA[0][BVal(x[1], 0)] ^ SB_DATA[1][BVal(x[2], 1)] ^
                   SB_DATA[2][BVal(x[3], 2)] ^ SB_DATA[3][BVal(x[0], 3)];
        }

        private static uint FourTablesC(uint[] x)
        {
            return SB_DATA[0][BVal(x[2], 0)] ^ SB_DATA[1][BVal(x[3], 1)] ^
                   SB_DATA[2][BVal(x[0], 2)] ^ SB_DATA[3][BVal(x[1], 3)];
        }

        private static uint FourTablesD(uint[] x)
        {
            return SB_DATA[0][BVal(x[3], 0)] ^ SB_DATA[1][BVal(x[0], 1)] ^
                   SB_DATA[2][BVal(x[1], 2)] ^ SB_DATA[3][BVal(x[2], 3)];
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

            return SUB_BYTE_VALUE[y, x];
        }

        private static byte[] RotLeft(byte[] input)
        {
            return new byte[] { input[1], input[2], input[3], input[0] };
        }
    }
}
