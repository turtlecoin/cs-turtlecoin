/*
Copyright 2014 Diego Alejandro Gómez <diego.gomezy@udea.edu.co>
Copyright 2018 The TurtleCoin Developers

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied. See the License for the specific language governing
permissions and limitations under the License.
*/

using System;
using System.IO;

namespace Canti.Cryptography.Native
{
    public partial class Groestl : IHashProvider
    {
        public byte[] Hash(byte[] input)
        {
            // The following variables follow the same naming convention used in the
            // specification:
            //     l: length of each message block, in bits.
            //     t: number of blocks the whole message uses.
            //     T: total number of blocks.
            //     N: number of bits in the input message.
            //     w: number of zeroes for padding.
            int l = BUFFER_SIZE * 8;
            ulong N = 0;
            ulong t = 0;
            ulong T = 0;
            ulong w = 0;

            byte[] h = new byte[BUFFER_SIZE];
            Array.Copy(IV, h, BUFFER_SIZE);

            /* Buffer for each block of the input */
            byte[] buffer = new byte[BUFFER_SIZE];

            int bytesRead = 0;

            for (int i = 0; i < input.Length; i+= BUFFER_SIZE)
            {
                /* Can't read a whole block */
                if ((input.Length - i) < BUFFER_SIZE)
                {
                    bytesRead = input.Length - i;
                    Buffer.BlockCopy(input, i, buffer, 0, input.Length - i);
                    break;
                }

                Buffer.BlockCopy(input, i, buffer, 0, BUFFER_SIZE);

                N = N + (ulong)((BUFFER_SIZE * 8));
                h = Compression(h, buffer);
                t++;
            }

            N = N + (ulong)((bytesRead * 8));
            w = (ulong)((((-(long)N - 65) % l )+ l) % l);
            T = (N + w + 65) / (ulong)l;
            int blocksLeft = (int)(T - t);
            buffer[bytesRead] = 0x80;
            int numberOfZeroBytes = BUFFER_SIZE - bytesRead - 1;
            byte[] zeroes = new byte[numberOfZeroBytes];
            Array.Copy(zeroes, 0, buffer, bytesRead + 1, numberOfZeroBytes);

            for (uint i = 0; i < blocksLeft; i++)
            {
                if (i == blocksLeft - 1)
                {
                    byte[] bytes = BitConverter.GetBytes((ulong)T);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }

                    Array.Copy(bytes, 0, buffer, BUFFER_SIZE - 8, 8);
                }

                h = Compression(h, buffer);
                buffer = new byte[BUFFER_SIZE];
            }

            h = Truncation(XOR(P(h), h));

            return h;
        }

        private static byte[] Compression(byte[] h, byte[] m)
        {
            return XOR(XOR(P(XOR(h, m)), Q(m)), h);
        }

        private static byte[] P(byte[] input)
        {
            int R = 10;

            byte[][] state = BytesToMatrixMap(input);
            for (byte r = 0; r < R; r++)
            {
                AddRoundConstant(state, r, 0);
                SubBytes(state);
                ShiftBytes(state, 0);
                MixBytes(state);
            }
            return MatrixToBytesMap(state);
        }

        private static byte[] Q(byte[] input)
        {
            int R = 10;

            byte[][] state = BytesToMatrixMap(input);
            for (byte r = 0; r < R; r++)
            {
                AddRoundConstant(state, r, 1);
                SubBytes(state);
                ShiftBytes(state, 1);
                MixBytes(state);
            }
            return MatrixToBytesMap(state);
        }

        private static void AddRoundConstant(byte[][] state, byte r, byte permutation)
        {
            int nColumns = 8;

            /* permutation == 0 -> P ^ permutation == 1 -> Q */
            if (permutation == 0)
            {
                byte[] C = {(byte)(0x00 ^ r), (byte)(0x10 ^ r), (byte)(0x20 ^ r),
                           (byte)(0x30 ^ r), (byte)(0x40 ^ r), (byte)(0x50 ^ r),
                           (byte)(0x60 ^ r), (byte)(0x70 ^ r), (byte)(0x80 ^ r),
                           (byte)(0x90 ^ r), (byte)(0xa0 ^ r), (byte)(0xb0 ^ r),
                           (byte)(0xc0 ^ r), (byte)(0xd0 ^ r), (byte)(0xe0 ^ r),
                           (byte)(0xf0 ^ r)};
                for (int j = 0; j < nColumns; j++)
                {
                    state[0][j] = (byte)(state[0][j] ^ C[j]);
                }
            }
            else
            {
                byte[] C = {(byte)(0xff ^ r), (byte)(0xef ^ r), (byte)(0xdf ^ r),
                           (byte)(0xcf ^ r), (byte)(0xbf ^ r), (byte)(0xaf ^ r),
                           (byte)(0x9f ^ r), (byte)(0x8f ^ r), (byte)(0x7f ^ r),
                           (byte)(0x6f ^ r), (byte)(0x5f ^ r), (byte)(0x4f ^ r),
                           (byte)(0x3f ^ r), (byte)(0x2f ^ r), (byte)(0x1f ^ r),
                           (byte)(0x0f ^ r)};
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < nColumns; j++)
                    {
                        state[i][j] = (byte)(state[i][j] ^ 0xff);
                    }
                }
                for (int j = 0; j < nColumns; j++)
                {
                    state[7][j] = (byte)(state[7][j] ^ C[j]);
                }
            }
        }

        private static void SubBytes(byte[][] state)
        {
            int nColumns = 8;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < nColumns; j++)
                {
                    state[i][j] = S_BOX[state[i][j]];
                }
            }
        }

        private static void ShiftBytes(byte[][] state, byte permutation)
        {
            /* permutation == 0 -> P ^ permutation == 1 -> Q */
            byte[] sigma;

            if (permutation == 0)
            {
                sigma = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            }
            else
            {
                sigma = new byte[] { 1, 3, 5, 7, 0, 2, 4, 6 };
            }

            for (int i = 0; i < 8; i++)
            {
                state[i] = ShiftRow(state[i], sigma[i]);
            }
        }

        private static void MixBytes(byte[][] state)
        {
            int nColumns = 8;

            byte[] x = new byte[8];
            byte[] y = new byte[8];
            byte[] z = new byte[8];
            for (int j = 0; j < nColumns; j++)
            {
                x[0] = (byte)(state[0][j] ^ state[(0 + 1) % 8][j]);
                x[1] = (byte)(state[1][j] ^ state[(1 + 1) % 8][j]);
                x[2] = (byte)(state[2][j] ^ state[(2 + 1) % 8][j]);
                x[3] = (byte)(state[3][j] ^ state[(3 + 1) % 8][j]);
                x[4] = (byte)(state[4][j] ^ state[(4 + 1) % 8][j]);
                x[5] = (byte)(state[5][j] ^ state[(5 + 1) % 8][j]);
                x[6] = (byte)(state[6][j] ^ state[(6 + 1) % 8][j]);
                x[7] = (byte)(state[7][j] ^ state[(7 + 1) % 8][j]);
                y[0] = (byte)(x[0] ^ x[(0 + 3) % 8]);
                y[1] = (byte)(x[1] ^ x[(1 + 3) % 8]);
                y[2] = (byte)(x[2] ^ x[(2 + 3) % 8]);
                y[3] = (byte)(x[3] ^ x[(3 + 3) % 8]);
                y[4] = (byte)(x[4] ^ x[(4 + 3) % 8]);
                y[5] = (byte)(x[5] ^ x[(5 + 3) % 8]);
                y[6] = (byte)(x[6] ^ x[(6 + 3) % 8]);
                y[7] = (byte)(x[7] ^ x[(7 + 3) % 8]);
                z[0] = (byte)(x[0] ^ x[(0 + 2) % 8] ^ state[(0 + 6) % 8][j]);
                z[1] = (byte)(x[1] ^ x[(1 + 2) % 8] ^ state[(1 + 6) % 8][j]);
                z[2] = (byte)(x[2] ^ x[(2 + 2) % 8] ^ state[(2 + 6) % 8][j]);
                z[3] = (byte)(x[3] ^ x[(3 + 2) % 8] ^ state[(3 + 6) % 8][j]);
                z[4] = (byte)(x[4] ^ x[(4 + 2) % 8] ^ state[(4 + 6) % 8][j]);
                z[5] = (byte)(x[5] ^ x[(5 + 2) % 8] ^ state[(5 + 6) % 8][j]);
                z[6] = (byte)(x[6] ^ x[(6 + 2) % 8] ^ state[(6 + 6) % 8][j]);
                z[7] = (byte)(x[7] ^ x[(7 + 2) % 8] ^ state[(7 + 6) % 8][j]);
                state[0][j] = (byte)(Doubling((byte)(Doubling(y[(0 + 3) % 8]) ^ z[(0 + 7) % 8])) ^ z[(0 + 4) % 8]);
                state[1][j] = (byte)(Doubling((byte)(Doubling(y[(1 + 3) % 8]) ^ z[(1 + 7) % 8])) ^ z[(1 + 4) % 8]);
                state[2][j] = (byte)(Doubling((byte)(Doubling(y[(2 + 3) % 8]) ^ z[(2 + 7) % 8])) ^ z[(2 + 4) % 8]);
                state[3][j] = (byte)(Doubling((byte)(Doubling(y[(3 + 3) % 8]) ^ z[(3 + 7) % 8])) ^ z[(3 + 4) % 8]);
                state[4][j] = (byte)(Doubling((byte)(Doubling(y[(4 + 3) % 8]) ^ z[(4 + 7) % 8])) ^ z[(4 + 4) % 8]);
                state[5][j] = (byte)(Doubling((byte)(Doubling(y[(5 + 3) % 8]) ^ z[(5 + 7) % 8])) ^ z[(5 + 4) % 8]);
                state[6][j] = (byte)(Doubling((byte)(Doubling(y[(6 + 3) % 8]) ^ z[(6 + 7) % 8])) ^ z[(6 + 4) % 8]);
                state[7][j] = (byte)(Doubling((byte)(Doubling(y[(7 + 3) % 8]) ^ z[(7 + 7) % 8])) ^ z[(7 + 4) % 8]);
            }
        }

        private static byte[][] BytesToMatrixMap(byte[] input)
        {
            int nColumns = 8;
            int k = 0;

            byte[][] result = new byte[][]
            {
                new byte[nColumns],
                new byte[nColumns],
                new byte[nColumns],
                new byte[nColumns],
                new byte[nColumns],
                new byte[nColumns],
                new byte[nColumns],
                new byte[nColumns]
            };

            for (int j = 0; j < nColumns; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    result[i][j] = input[k];
                    k++;
                }
            }
            return result;
        }

        private static byte[] MatrixToBytesMap(byte[][] input)
        {
            int nBytes = 64;

            int k = 0;
            byte[] result = new byte[nBytes];
            for (int j = 0; j < nBytes / 8; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    result[k] = input[i][j];
                    k++;
                }
            }
            return result;
        }

        private static byte[] Truncation(byte[] x)
        {
            int nBytes = 32;
            byte[] result = new byte[nBytes];
            System.Buffer.BlockCopy(x, x.Length - nBytes, result, 0, nBytes);
            return result;
        }

        private static byte[] XOR(byte[] op1, byte[] op2)
        {
            byte[] result = new byte[op1.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(op1[i] ^ op2[i]);
            }

            return result;
        }

        private static byte Doubling(byte x)
        {
            return (byte)((x & 0x80) == 0x80 ? (x << 1) ^ 0x1b : x << 1);
        }

        private static byte[] ShiftRow(byte[] row, int shift)
        {
            byte[] newRow = new byte[row.Length];
            System.Buffer.BlockCopy(row, shift, newRow, 0, row.Length - shift);
            System.Buffer.BlockCopy(row, 0, newRow, row.Length - shift, shift);
            return newRow;
        }
    }
}
