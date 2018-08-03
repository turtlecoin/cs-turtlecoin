// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using Canti.Data;

namespace Canti.Blockchain.Crypto.Keccak
{
    public static class Keccak
    {
        public static void keccakf(ulong[] state, int rounds)
        {
            ulong t;

            ulong[] bc = new ulong[5];

            for (int round = 0; round < rounds; round++)
            {
                /* Theta */
                for (int i = 0; i < 5; i++)
                {
                    bc[i] = state[i] ^ state[i+5] ^ state[i+10] ^ state[i+15] ^ state[i + 20];
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
                    int j = Constants.keccakf_piln[i];
                    bc[0] = state[j];
                    state[j] = ROTL64(t, (ulong)(Constants.keccakf_rotc[i]));
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
                state[0] ^= Constants.keccakf_rndc[round];
            }
        }

        /* This can throw if the input size or outputSize is not of a valid
           value. Suggestes output size values are 32 and 200, and suggested
           input values are 1 - 136 */
        private static byte[] _keccak(byte[] input, int outputSize)
        {
            ulong[] state = new ulong[25];

            int rsiz;

            /* sizeof (state) == outputSize */
            if ((state.Length * 8) == outputSize)
            {
                rsiz = Constants.HASH_DATA_AREA;
            }
            else
            {
                rsiz = 200 - 2 * outputSize;
            }

            int inputLength = input.Length;

            int offset = 0;

            /* We index the array 8 bytes at a time, and treat each set of
               bytes as a long */
            for (; inputLength >= rsiz; inputLength -= rsiz, offset += rsiz * 8)
            {
                for (int i = 0; i < rsiz; i += 8)
                {
                    /* Read 8 bytes as a ulong */
                    state[i / 8] ^= Encoding.ByteArrayToInteger<ulong>(input, offset + i, 8);
                }

                keccakf(state, Constants.KECCAK_ROUNDS);
            }

            byte[] temp = new byte[144];

            for (int i = 0; i < inputLength; i++)
            {
                temp[i] = input[i + offset];
            }

            temp[inputLength++] = 1;

            for (int i = inputLength; i < rsiz - inputLength; i++)
            {
                temp[i] = 0;
            }

            temp[rsiz - 1] |= 0x80;

            /* We proceed in chunks of 8 bytes as once */
            for (int i = 0; i < rsiz; i += 8)
            {
                /* Read 8 bytes as a ulong */
                state[i / 8] ^= Encoding.ByteArrayToInteger<ulong>(temp, i, 8);
            }

            keccakf(state, Constants.KECCAK_ROUNDS);

            byte[] output = new byte[outputSize];

            /* Copy the ulong state into the output array as bytes */
            Buffer.BlockCopy(state, 0, output, 0, output.Length);

            return output;
        }

        private static ulong ROTL64(ulong x, ulong y)
        {
            return x << (int)y | x >> (int)((64 - y));
        }

        /* Hashes the given input with keccak, into an output hash of 200
           bytes. */
        public static byte[] keccak1600(byte[] input)
        {
            byte[] result = _keccak(input, 200);

            byte[] output = new byte[200];

            /* Copy result to output */
            Buffer.BlockCopy(result, 0, output, 0, 200);

            return output;
        }

        /* Hashes the given input with keccak, into an output hash of 32 bytes.
           Copies outputLength bytes of the output and returns it. Output
           length cannot be larger than 32. */
        public static byte[] keccak(byte[] input, int outputLength = 32)
        {
            if (outputLength > 32)
            {
                throw new ArgumentException("Output length must be 32 bytes or less!");
            }

            byte[] result = _keccak(input, 32);

            byte[] output = new byte[outputLength];

            /* Don't overflow input array */
            for (int i = 0; i < Math.Min(outputLength, 32); i++)
            {
                output[i] = result[i];
            }

            return output;
        }
    }
}
