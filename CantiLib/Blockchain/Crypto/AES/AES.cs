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

using System;

namespace Canti.Blockchain.Crypto.AES
{
    public static class AES
    {
        /* Offset = offset of input array to access */
        /* Array is passed by reference so nothing returned */
        public static void PseudoEncryptECB(byte[] keys, byte[] input, int offset)
        {
            for (int i = 0; i < 10; i++)
            {
                byte[] key = new byte[Constants.BlockSize];

                int x = i * Constants.RoundKeyLength * Constants.ColumnLength;

                /* Extract our single key from keys */
                Buffer.BlockCopy(keys, x, key, 0, Constants.BlockSize);

                EncryptionRound(key, input, offset);
            }
        }

        public static void EncryptionRound(byte[] key, byte[] data, int offset = 0)
        {
            for (int i = 0; i < Constants.BlockSize; i++)
            {
                data[i + offset] = SubByte(data[i + offset]);
            }

            ShiftRows(data, offset);

            MixColumns(data, offset);

            for (int i = 0; i < Constants.BlockSize; i++)
            {
                data[i + offset] ^= key[i];
            }
        }

        private static void ShiftRows(byte[] input, int offset)
        {
            byte[] tmp = new byte[input.Length - offset];

            /* Copy input to tmp array to work with */
            Buffer.BlockCopy(input, offset, tmp, 0, tmp.Length);

            for (int i = 0; i < 16; i++)
            {
                tmp[i] = input[(i * 5) % 16];
            }

            /* Copy tmp array back out to output */
            Buffer.BlockCopy(tmp, 0, input, offset, tmp.Length);
        }

        private static void MixColumns(byte[] input, int offset)
        {
            byte[] tmp = new byte[Constants.BlockSize];

            for (int i = 0; i < Constants.BlockSize; i += Constants.ColumnLength)
            {
                tmp[i] = (byte)(GfMul(input[i + offset], 2)
                              ^ GfMul(input[i + 1 + offset], 3)
                              ^ input[i + 2 + offset]
                              ^ input[i + 3 + offset]);
                
                tmp[i+1] = (byte)(input[i + offset]
                                ^ GfMul(input[i + 1 + offset], 2)
                                ^ GfMul(input[i + 2 + offset], 3)
                                ^ input[i + 3 + offset]);

                tmp[i+2] = (byte)(input[i + offset]
                                ^ input[i + 1 + offset]
                                ^ GfMul(input[i + 2 + offset], 2)
                                ^ GfMul(input[i + 3 + offset], 3));

                tmp[i+3] = (byte)(GfMul(input[i + offset], 3)
                                ^ input[i + 1 + offset]
                                ^ input[i + 2 + offset]
                                ^ GfMul(input[i + 3 + offset], 2));
            }

            Buffer.BlockCopy(tmp, 0, input, offset, tmp.Length);
        }

        private static byte GfMul(byte left, byte right)
        {
            byte x = left;
            byte y = left;

            x &= 0x0f;
            y &= 0xf0;

            y >>= 4;

            switch(right)
            {
                case 0x02:
                {
                    return Constants.GfMul2[y,x];
                }
                case 0x03:
                {
                    return Constants.GfMul3[y,x];
                }
                case 0x09:
                {
                    return Constants.GfMul9[y,x];
                }
                case 0x0b:
                {
                    return Constants.GfMulb[y,x];
                }
                case 0x0d:
                {
                    return Constants.GfMuld[y,x];
                }
                case 0x0e:
                {
                    return Constants.GfMule[y,x];
                }
                default:
                {
                    return left;
                }
            }
        }

        private static byte SubByte(byte input)
        {
            byte x = input;
            byte y = input;

            x &= 0x0f;
            y &= 0xf0;

            y >>= 4;

            return Constants.SubByteValue[y,x];
        }

        /* Rotate array one step left, e.g. [1, 2, 3, 4] becomes [4, 1, 2, 3]
           Assumes input is Constants.ColumnLength long */
        private static byte[] RotLeft(byte[] input)
        {
            byte[] output = new byte[Constants.ColumnLength];

            for (int i = 0; i < Constants.ColumnLength; i++)
            {
                output[i] = input[(i +1) % Constants.ColumnLength];
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
                    int index = (i - keyBase) * Constants.RoundKeyLength + j;

                    expanded[(i * Constants.RoundKeyLength) + j]
                        = (byte)(expanded[index] ^ tmp[j]);
                }
            }

            return expanded;
        }
    }
}
