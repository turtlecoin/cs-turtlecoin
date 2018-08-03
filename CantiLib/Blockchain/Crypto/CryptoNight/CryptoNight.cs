// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Numerics;

using Canti.Data;
using Canti.Blockchain.Crypto.AES;
using Canti.Blockchain.Crypto.Keccak;

namespace Canti.Blockchain.Crypto.CryptoNight
{
    public static class CryptoNight
    {
        /* TODO: Take memory, iterations, etc as input struct, or possibly
           use an interface */
        /* Returns a 32 byte hash */
        public static byte[] CryptoNightVersionZero(byte[] data)
        {
            /* CryptoNight Step 1: Use Keccak1600 to initialize the 'state'
             * buffer, encapsulated in cnState
             */

            CNState cnState = new CNState(Keccak.Keccak.keccak1600(data));

            /* CryptoNight Step 2:  Iteratively encrypt the results from Keccak
             * to fill the 2MB large random access buffer.
             */

            /* Expand our initial key into many for each round of pseudo aes */
            byte[] expandedKeys = AES.AES.ExpandKey(cnState.GetAESKey());

            /* Our large scratchpad, 2MB in default CN */
            byte[] scratchpad = new byte[Constants.Memory];

            byte[] text = cnState.GetText();

            for (int i = 0; i < Constants.CNIterations; i++)
            {
                for (int j = 0; j < Constants.InitSizeBlock; j++)
                {
                    /* Need to pass the array with an offset because we manip
                       it in place */
                    AES.AES.PseudoEncryptECB(expandedKeys, text,
                                             j * AES.Constants.BlockSize);
                }

                /* Write InitSizeBytes from text to the scratchpad, at the
                   offset i * InitSizeByte */
                Buffer.BlockCopy(text, 0, scratchpad,
                                 i * Constants.InitSizeByte,
                                 Constants.InitSizeByte);
            }

            byte[] a = new byte[AES.Constants.BlockSize];
            byte[] b = new byte[AES.Constants.BlockSize];
            byte[] c = new byte[AES.Constants.BlockSize];
            byte[] d = new byte[AES.Constants.BlockSize];

            byte[] k = cnState.GetK();

            for (int i = 0; i < AES.Constants.BlockSize; i++)
            {
                a[i] = (byte)(k[i] ^ k[i+32]);
                b[i] = (byte)(k[i+16] ^ k[i+48]);
            }

            /* CryptoNight Step 3: Bounce randomly 1,048,576 times (1<<20)
             * through the mixing scratchpad, using 524,288 iterations of the
             * following mixing function.  Each execution performs two reads
             * and writes from the mixing scratchpad.
             */

            for (int i = 0; i < Constants.Iterations / 2; i++)
            {
                for (int iteration = 1; iteration < 3; iteration++)
                {
                    /* Get our 'memory' address we're using for this round */
                    int j = e2i(a);

                    /* Load c from the scratchpad */
                    CopyBlockFromScratchpad(scratchpad, c, j);

                    /* ITERATION ONE */
                    if (iteration == 1)
                    {
                        AES.AES.EncryptionRound(a, c);

                        XORBlocks(b, c);

                        SwapBlocks(b, c);
                    }
                    /* ITERATION TWO */
                    else
                    {
                        /* Multiply a and c together, and place the result in
                           b, a and c are ulongs stored as byte arrays, d is a
                           128 bit value stored as a byte array */
                        Multiply128(a, c, d);

                        SumHalfBlocks(b, d);

                        SwapBlocks(b, c);

                        XORBlocks(b, c);
                    }

                    /* Write c back to the scratchpad */
                    CopyBlockToScratchpad(scratchpad, c, j);

                    SwapBlocks(a, b);
                }
            }

            /* Reinitialize text from state */
            text = cnState.GetText();

            /* Expand our initial key into many for each round of pseudo aes */
            expandedKeys = AES.AES.ExpandKey(cnState.GetAESKey2());


            /* CryptoNight Step 4:  Sequentially pass through the mixing buffer
             * and use 10 rounds of AES encryption to mix the random data back
             * into the 'text' buffer. 'text' was originally created with the
             * output of Keccak1600.
             */

            for (int i = 0; i < Constants.CNIterations; i++)
            {
                for (int j = 0; j < Constants.InitSizeBlock; j++)
                {
                    int offsetA = j * AES.Constants.BlockSize;
                    int offsetB = (i * Constants.InitSizeByte) + offsetA;

                    XORBlocks(text, scratchpad, offsetA, offsetB);

                    /* Need to pass the array with an offset because we manip
                       it in place */
                    AES.AES.PseudoEncryptECB(expandedKeys, text, offsetA);
                }
            }

            /* CryptoNight Step 5:  Apply Keccak to the state again, and then
             * use the resulting data to select which of four finalizer
             * hash functions to apply to the data (Blake, Groestl, JH,
             * or Skein). Use this hash to squeeze the state array down
             * to the final 256 bit hash output.
             */

            /* Copy text back to state */
            cnState.SetText(text);

            /* Get the state buffer as an array of ulongs rather than bytes */
            ulong[] hashState = cnState.GetHashState();

            Keccak.Keccak.keccakf(hashState, 24);

            /* Set the state buffer from the coerced hash state */
            cnState.SetHashState(hashState);

            /* Get the actual state buffer finally */
            byte[] state = cnState.GetState();
            
            /* Choose the final hashing function to use based on the value of
               state[0] */
            switch(state[0] % 4)
            {
                case 0:
                {
                    return Blake256.blake256(state);
                }
                case 1:
                {
                    return Groestl.groestl(state);
                }
                case 2:
                {
                    return Skein.skein(state);
                }
                default:
                {
                    return JH.jh(state);
                }
            }
        }

        /* TODO: Verify correctness of this function */
        private static void SumHalfBlocks(byte[] a, byte[] b)
        {
            /* Read the two byte arrays into 4 separate ulongs */
            ulong a0 = Encoding.ByteArrayToInteger<ulong>(a, 0, 8);
            ulong a1 = Encoding.ByteArrayToInteger<ulong>(a, 8, 8);
            ulong b0 = Encoding.ByteArrayToInteger<ulong>(b, 0, 8);
            ulong b1 = Encoding.ByteArrayToInteger<ulong>(b, 8, 8);

            a0 += b0;
            a1 += b1;

            byte[] tmpA0 = Encoding.IntegerToByteArray<ulong>(a0);
            byte[] tmpA1 = Encoding.IntegerToByteArray<ulong>(a1);

            /* Copy tmpA0 into a[0..7], and tmpA1 into a[8..15] */
            Buffer.BlockCopy(tmpA0, 0, a, 0, 8);
            Buffer.BlockCopy(tmpA1, 0, a, 8, 8);
        }

        /* TODO: Verify correctness of this function */
        private static void Multiply128(byte[] a, byte[] b, byte[] result)
        {
            /* Read 8 bytes from a and b as a ulong */
            ulong a0 = Encoding.ByteArrayToInteger<ulong>(a, 0, 8);
            ulong b0 = Encoding.ByteArrayToInteger<ulong>(b, 0, 8);

            /* Multiply as big ints then convert back to byte array.
               This format has the low bits first, rather than the high bits
               first as the result expects */
            byte[] multiplied = (new BigInteger(a0)
                               * new BigInteger(b0)).ToByteArray();

            /* Need to copy a max of 16 bytes, so we don't overflow the result
               array (sometimes ignoring sign bit) but we need to make sure
               we don't copy more than the length of the multiplied array,
               as it uses less bytes when the value is smaller */
            int toCopy = Math.Min(multiplied.Length, 16);

            Buffer.BlockCopy(multiplied, 0, result, 0, toCopy);

            /* Reverse to put the high bits first */
            Array.Reverse(result);
        }

        /* Copy blocksize bytes from the scratchpad at an offset of
           offset * AES.Constants.BlockSize to destination */
        private static void CopyBlockFromScratchpad(byte[] scratchpad,
                                                    byte[] destination,
                                                    int offset)
        {
            Buffer.BlockCopy(scratchpad, offset * AES.Constants.BlockSize,
                             destination, 0, AES.Constants.BlockSize);
        }

        /* Copy blocksize bytes from the source to the scratchpad at an offset
           of offset * AES.Constants.BlockSize in the scratchpad */
        private static void CopyBlockToScratchpad(byte[] scratchpad,
                                                  byte[] source,
                                                  int offset)
        {
            Buffer.BlockCopy(source, 0, scratchpad,
                             offset * AES.Constants.BlockSize,
                             AES.Constants.BlockSize);
        }


        /* XOR a and b, with optional offsets in the array */
        private static void XORBlocks(byte[] a, byte[] b, int offsetA = 0,
                                                          int offsetB = 0)
        {
            for (int i = 0; i < AES.Constants.BlockSize; i++)
            {
                a[i + offsetA] ^= b[i + offsetB];
            }
        }

        /* Swap the values of a and b */
        private static void SwapBlocks(byte[] a, byte[] b)
        {
            for (int i = 0; i < AES.Constants.BlockSize; i++)
            {
                byte tmp = a[i];
                a[i] = b[i];
                b[i] = tmp;
            }
        }

        /* Get a memory address to work with in our scratchpad */
        private static int e2i(byte[] input)
        {
            /* Read 8 bytes as a ulong */
            ulong j = Encoding.ByteArrayToInteger<ulong>(input, 0, 8);

            /* Divide by aes block size */
            j /= AES.Constants.BlockSize;

            /* Bitwise AND with (memory / blocksize) - 1*/
            return (int)(j & (Constants.Memory / AES.Constants.BlockSize - 1));
        }

        /* This class encapsulates the different ways we index the 200 byte
           state buffer. There are different bits we use each section for,
           of different lengths and offsets. */
        private sealed class CNState
        {
            /* State is a 200 byte buffer */
            public CNState(byte[] state)
            {
                this.state = state;
            }

            /* AESKey is the first 32 bytes of our 200 byte state buffer */
            public byte[] GetAESKey()
            {
                byte[] AESKey = new byte[AES.Constants.KeySize];

                /* Copy 32 bytes from the state array to the AESKey array */
                Buffer.BlockCopy(state, 0, AESKey, 0, AESKey.Length);

                return AESKey;
            }

            /* AESKey2 is a 32 byte array, offset by 32 in state, e.g.
               state[32:64] */
            public byte[] GetAESKey2()
            {
                byte[] AESKey2 = new byte[AES.Constants.KeySize];

                /* Copy 32 bytes from the state array, at an offset of 32, to the
                   AESKey2 array */
                Buffer.BlockCopy(state, 32, AESKey2, 0, AESKey2.Length);

                return AESKey2;
            }

            /* K is the first 64 bytes of our 200 byte state buffer */
            public byte[] GetK()
            {
                byte[] k = new byte[64];

                /* Copy 64 bytes from the state array to the k array */
                Buffer.BlockCopy(state, 0, k, 0, k.Length);

                return k;
            }

            /* Text is a 128 byte buffer, offset by 64 bytes in state, e.g.
               state[64:192] */
            public byte[] GetText()
            {
                byte[] text = new byte[Constants.InitSizeByte];

                /* Copy 128 bytes from the state array, at an offset of 64, to
                   the text array */
                Buffer.BlockCopy(state, 64, text, 0, text.Length);

                return text;
            }

            public void SetText(byte[] text)
            {
                /* Copy 128 bytes from the text array, to the state array at an
                   offset of 64 */
                Buffer.BlockCopy(text, 0, state, 64, text.Length);
            }

            public ulong[] GetHashState()
            {
                ulong[] hashState = new ulong[state.Length / 8];

                /* Coerce state into an array of ulongs rather than bytes */
                Buffer.BlockCopy(state, 0, hashState, 0, state.Length);

                return hashState;
            }

            public void SetHashState(ulong[] hashState)
            {
                /* Coerce hashState back into an array of bytes */
                Buffer.BlockCopy(hashState, 0, state, 0, state.Length);
            }

            public byte[] GetState()
            {
                return state;
            }

            /* The underlying 200 byte array */
            private byte[] state;
        }
    }
}
