//
// Copyright 2011 Markku-Juhani O. Saarinen
// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using Canti.Data;
using Canti.Blockchain.Crypto;

/* This is pre NIST keccak before the sha-3 revisions */
namespace Canti.Blockchain.Crypto.Keccak
{
    public class Keccak : IHashProvider
    {
        public byte[] Hash(byte[] input)
        {
            return keccak(input);
        }

        public static void keccakf(ulong[] state, 
                                   int rounds = Constants.KeccakRounds)
        {
            ulong t;

            ulong[] bc = new ulong[5];

            for (int round = 0; round < rounds; round++)
            {
                /* Theta */
                for (int i = 0; i < 5; i++)
                {
                    bc[i] = state[i] ^ state[i+5] ^ state[i+10] ^ state[i+15]
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

        /* Compute a hash of length outputSize from input */
        private static byte[] _keccak(byte[] input, int outputSize)
        {
            ulong[] state = new ulong[25];

            int rsiz = Constants.HashDataArea;

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
                    state[i] ^= Encoding.ByteArrayToInteger<ulong>(
                        input, offset + (i * 8), 8
                    );
                }

                keccakf(state);
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
                state[i] ^= Encoding.ByteArrayToInteger<ulong>(tmp, i * 8, 8);
            }

            keccakf(state, Constants.KeccakRounds);

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
        public static byte[] keccak1600(byte[] input)
        {
            return _keccak(input, 200);
        }

        /* Hashes the given input with keccak, into an output hash of 32 bytes.
           Copies outputLength bytes of the output and returns it. Output
           length cannot be larger than 32. */
        public static byte[] keccak(byte[] input, int outputLength = 32)
        {
            if (outputLength > 32)
            {
                throw new ArgumentException(
                    "Output length must be 32 bytes or less!"
                );
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
