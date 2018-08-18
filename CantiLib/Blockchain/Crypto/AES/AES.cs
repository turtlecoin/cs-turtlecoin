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
           Assumes input is Constants.ColumnLength long */
        private static byte[] RotLeft(byte[] input)
        {
            byte[] output = new byte[Constants.ColumnLength];

            for (int i = 0; i < Constants.ColumnLength; i++)
            {
                output[i] = input[(i + 1) % Constants.ColumnLength];
            }

            return output;
        }

        public static byte[] ExpandKey(byte[] key)
        {
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
    }
}
