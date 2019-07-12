//
// Copyright 2011 Markku-Juhani O. Saarinen
// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018-2019 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using static Canti.Utils;

namespace Canti.Cryptography.Native
{
    // This is pre NIST keccak before the sha-3 revisions
    public sealed class Keccak : IHashProvider
    {
        #region Constants

        private static readonly ulong[] keccakf_rndc =
        {
            0x0000000000000001, 0x0000000000008082, 0x800000000000808a,
            0x8000000080008000, 0x000000000000808b, 0x0000000080000001,
            0x8000000080008081, 0x8000000000008009, 0x000000000000008a,
            0x0000000000000088, 0x0000000080008009, 0x000000008000000a,
            0x000000008000808b, 0x800000000000008b, 0x8000000000008089,
            0x8000000000008003, 0x8000000000008002, 0x8000000000000080,
            0x000000000000800a, 0x800000008000000a, 0x8000000080008081,
            0x8000000000008080, 0x0000000080000001, 0x8000000080008008
        };

        private static readonly int[] keccakf_rotc =
        {
            1,  3,  6,  10, 15, 21, 28, 36, 45, 55, 2,  14,
            27, 41, 56, 8,  25, 43, 62, 18, 39, 61, 20, 44
        };

        private static readonly int[] keccakf_piln =
        {
            10, 7,  11, 17, 18, 3, 5,  16, 8,  21, 24, 4,
            15, 23, 19, 13, 12, 2, 20, 14, 22, 9,  6,  1
        };

        private const int KeccakRounds = 24;

        private const int HashDataArea = 136;

        #endregion

        #region Methods

        public static void Keccakf(ulong[] state, int rounds = KeccakRounds)
        {
            ulong t;

            ulong[] bc = new ulong[5];

            for (int round = 0; round < rounds; round++)
            {
                /* Theta */
                for (int i = 0; i < 5; i++)
                {
                    bc[i] = state[i] ^ state[i + 5] ^ state[i + 10] ^ state[i + 15]
                                     ^ state[i + 20];
                }

                for (int i = 0; i < 5; i++)
                {
                    t = bc[(i + 4) % 5] ^ ROTL64(bc[(i + 1) % 5], 1);

                    for (int j = 0; j < 25; j += 5)
                    {
                        state[i + j] ^= t;
                    }
                }

                /* Rho Pi */
                t = state[1];

                for (int i = 0; i < 24; i++)
                {
                    int j = keccakf_piln[i];
                    bc[0] = state[j];
                    state[j] = ROTL64(t, (ulong)(keccakf_rotc[i]));
                    t = bc[0];
                }

                /* Chi */
                for (int j = 0; j < 25; j += 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        bc[i] = state[i + j];
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        state[i + j] ^= (~bc[(i + 1) % 5]) & bc[(i + 2) % 5];
                    }
                }

                /* Iota */
                state[0] ^= keccakf_rndc[round];
            }
        }

        /* Compute a hash of length outputSize from input */
        private static byte[] _Keccak(byte[] input, int outputSize)
        {
            ulong[] state = new ulong[25];

            int rsiz = HashDataArea;

            if (outputSize != 200)
            {
                rsiz = 200 - 2 * outputSize;
            }

            int rsizw = rsiz / 8;

            int inputLength = input.Length;

            /* Offset of input array */
            int offset = 0;

            for (; inputLength >= rsiz; inputLength -= rsiz, offset += rsiz)
            {
                for (int i = 0; i < rsizw; i++)
                {
                    /* Read 8 bytes as a ulong, need to multiply i by 8
                       because we're reading chunks of 8 at once */
                    state[i] ^= ByteArrayToInteger<ulong>(input, offset + (i * 8));
                }

                Keccakf(state);
            }

            byte[] tmp = new byte[144];

            /* Copy inputLength bytes from input to tmp at an offset of
               offset from input */
            Buffer.BlockCopy(input, offset, tmp, 0, inputLength);

            tmp[inputLength++] = 1;

            /* Zero (rsiz - inputLength) bytes in tmp, at an offset of
               inputLength */
            Array.Clear(tmp, inputLength, rsiz - inputLength);

            tmp[rsiz - 1] |= 0x80;

            for (int i = 0; i < rsizw; i++)
            {
                /* Read 8 bytes as a ulong - need to read at (i * 8) because
                   we're reading chunks of 8 at once, rather than overlapping
                   chunks of 8 */
                state[i] ^= ByteArrayToInteger<ulong>(tmp, i * 8);
            }

            Keccakf(state, KeccakRounds);

            byte[] output = new byte[outputSize];

            Buffer.BlockCopy(state, 0, output, 0, outputSize);

            return output;
        }

        private static ulong ROTL64(ulong x, ulong y)
        {
            return x << (int)y | x >> (int)((64 - y));
        }

        /* Hashes the given input with keccak, into an output hash of 200
           bytes. */
        public static byte[] Keccak1600(byte[] input)
        {
            return _Keccak(input, 200);
        }

        /* Hashes the given input with keccak, into an output hash of 32 bytes.
           Copies outputLength bytes of the output and returns it. Output
           length cannot be larger than 32. */
        public byte[] Hash(byte[] input)
        {
            return KeccakHash(input, 32);
        }

        public static byte[] KeccakHash(byte[] input, int outputLength = 32)
        {
            if (outputLength > 32)
            {
                throw new ArgumentException(
                    "Output length must be 32 bytes or less!"
                );
            }

            byte[] result = _Keccak(input, 32);

            byte[] output = new byte[outputLength];

            /* Don't overflow input array */
            for (int i = 0; i < Math.Min(outputLength, 32); i++)
            {
                output[i] = result[i];
            }

            return output;
        }

        #endregion
    }
}
