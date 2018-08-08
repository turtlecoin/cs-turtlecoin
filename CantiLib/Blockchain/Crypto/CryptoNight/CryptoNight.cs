//
// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using Canti.Data;
using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.AES;
using Canti.Blockchain.Crypto.Keccak;
using Canti.Blockchain.Crypto.Blake;
using Canti.Blockchain.Crypto.Groestl;
using Canti.Blockchain.Crypto.Skein;
using Canti.Blockchain.Crypto.JH;

namespace Canti.Blockchain.Crypto.CryptoNight
{
    public static class CryptoNight
    {
        public static byte[] SlowHash(byte[] input, ICryptoNight cnParams)
        {
            switch(cnParams.Variant())
            {
                case 0:
                {
                    return CryptoNightV0(input, cnParams);
                }
                case 1:
                {
                    return CryptoNightV1(input, cnParams);
                }
                default:
                {
                    throw new ArgumentException(
                        "Variants other than 0 and 1 are not supported!"
                    );
                }
            }
        }

        private static byte[] CryptoNightV0(byte[] input, ICryptoNight cnParams)
        {
            /* CryptoNight Step 1: Use Keccak1600 to initialize the 'state'
             * buffer, encapsulated in cnState
             */
            CNState cnState = new CNState(Keccak.Keccak.keccak1600(input));

            byte[] scratchpad = FillScratchpad(cnState, cnParams);

            MixScratchpadV0(cnState, cnParams, scratchpad);

            EncryptScratchpadToText(cnState, cnParams, scratchpad);

            return HashFinalState(cnState);
        }

        private static byte[] CryptoNightV1(byte[] input, ICryptoNight cnParams)
        {
            if (input.Length < 43)
            {
                throw new ArgumentException(
                    "Input to CryptoNightV1 must be at least 43 bytes!"
                );
            }

            /* CryptoNight Step 1: Use Keccak1600 to initialize the 'state'
             * buffer, encapsulated in cnState
             */
            CNState cnState = new CNState(Keccak.Keccak.keccak1600(input));

            byte[] scratchpad = FillScratchpad(cnState, cnParams);

            byte[] tweak = VariantOneInit(cnState, input);

            MixScratchpadV1(cnState, cnParams, scratchpad, tweak);

            EncryptScratchpadToText(cnState, cnParams, scratchpad);

            return HashFinalState(cnState);
        }

        /* CryptoNight Step 2:  Iteratively encrypt the results from Keccak
         * to fill the large scratchpad
         */
        private static byte[] FillScratchpad(CNState cnState,
                                             ICryptoNight cnParams)
        {
            /* Expand our initial key into many for each round of pseudo aes */
            byte[] expandedKeys = AES.AES.ExpandKey(cnState.GetAESKey());

            /* Our large scratchpad, 2MB in default CN */
            byte[] scratchpad = new byte[cnParams.Memory()];

            byte[] text = cnState.GetText();

            /* Fill the scratchpad with AES encryption of text */
            for (int i = 0; i < cnParams.Memory() / Constants.InitSizeByte; i++)
            {
                for (int j = 0; j < Constants.InitSizeBlock; j++)
                {
                    /* Need to pass the array with an offset because we manip
                       it in place */
                    AES.AES.PseudoEncryptECB(expandedKeys, text,
                                             j * AES.Constants.BlockSize);
                }

                /* Write text to the scratchpad, at the offset
                   i * InitSizeByte */
                Buffer.BlockCopy(text, 0, scratchpad,
                                 i * Constants.InitSizeByte, text.Length);
            }

            return scratchpad;
        }

        /* CryptoNight Step 3: Bounce randomly 1,048,576 times (1<<20)
         * through the mixing scratchpad, using 524,288 iterations of the
         * following mixing function.  Each execution performs two reads
         * and writes from the mixing scratchpad.
         */
        private static void MixScratchpadV0(CNState cnState,
                                            ICryptoNight cnParams,
                                            byte[] scratchpad)
        {
            MixScratchpadState mixingState = new MixScratchpadState(cnState);

            for (int i = 0; i < cnParams.Iterations() / 2; i++)
            {
                for (int iteration = 1; iteration < 3; iteration++)
                {
                    /* Get our 'memory' address we're using for this round */
                    int j = e2i(mixingState.a, cnParams.Memory());

                    /* Load c from the scratchpad */
                    CopyBlockFromScratchpad(scratchpad, mixingState.c, j);

                    /* Perform the mixing function */
                    if (iteration == 1)
                    {
                        MixScratchpadIterationOne(mixingState);
                    }
                    else
                    {
                        MixScratchpadIterationTwo(mixingState);
                    }

                    /* Write c back to the scratchpad */
                    CopyBlockToScratchpad(scratchpad, mixingState.c, j);

                    SwapBlocks(mixingState.a, mixingState.b);
                }
            }
        }

        /* CryptoNight Step 3: Bounce randomly 1,048,576 times (1<<20)
         * through the mixing scratchpad, using 524,288 iterations of the
         * following mixing function.  Each execution performs two reads
         * and writes from the mixing scratchpad.
         */
        private static void MixScratchpadV1(CNState cnState,
                                            ICryptoNight cnParams,
                                            byte[] scratchpad, byte[] tweak)
        {
            MixScratchpadState mixingState = new MixScratchpadState(cnState);

            for (int i = 0; i < cnParams.Iterations() / 2; i++)
            {
                for (int iteration = 1; iteration < 3; iteration++)
                {
                    /* Get our 'memory' address we're using for this round */
                    int j = e2i(mixingState.a, cnParams.Memory());

                    /* Load c from the scratchpad */
                    CopyBlockFromScratchpad(scratchpad, mixingState.c, j);

                    /* Perform the mixing function */
                    if (iteration == 1)
                    {
                        MixScratchpadIterationOne(mixingState);
                    }
                    else
                    {
                        MixScratchpadIterationTwo(mixingState);
                        /* Perform the variant one tweak, second iteration */
                        VariantOneStepTwo(mixingState.c, 8, tweak);
                    }

                    /* Write c back to the scratchpad */
                    CopyBlockToScratchpad(scratchpad, mixingState.c, j);

                    SwapBlocks(mixingState.a, mixingState.b);

                    /* Perform the variant one tweak, first iteration */
                    if (iteration == 1)
                    {
                        VariantOneStepOne(scratchpad, j);
                    }
                }
            }
        }

        private static void MixScratchpadIterationOne(MixScratchpadState
                                                      mixingState)
        {
            AES.AES.EncryptionRound(mixingState.a, mixingState.c);

            XORBlocks(mixingState.b, mixingState.c);

            SwapBlocks(mixingState.b, mixingState.c);
        }

        private static void MixScratchpadIterationTwo(MixScratchpadState
                                                      mixingState)
        {
            /* Multiply a and c together, and place the result in
               b, a and c are ulongs stored as byte arrays, d is a
               128 bit value stored as a byte array */
            Multiply128(mixingState.a, mixingState.c, mixingState.d);

            SumHalfBlocks(mixingState.b, mixingState.d);

            SwapBlocks(mixingState.b, mixingState.c);

            XORBlocks(mixingState.b, mixingState.c);
        }

        /* CryptoNight Step 4: Sequentially pass through the mixing buffer
         * and use 10 rounds of AES encryption to mix the random data back
         * into the 'text' buffer.
         */
        private static void EncryptScratchpadToText(CNState cnState,
                                                    ICryptoNight cnParams, 
                                                    byte[] scratchpad)
        {
            /* Reinitialize text from state */
            byte[] text = cnState.GetText();

            /* Expand our initial key into many for each round of pseudo aes */
            byte[] expandedKeys = AES.AES.ExpandKey(cnState.GetAESKey2());
            
            for (int i = 0; i < cnParams.Memory() / Constants.InitSizeByte; i++)
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

            cnState.SetText(text);
        }
        
        /* CryptoNight Step 5: Apply Keccak to the state again, and then
         * use the resulting data to select which of four finalizer
         * hash functions to apply to the data (Blake, Groestl, JH,
         * or Skein). Use this hash to squeeze the state array down
         * to the final 32 byte hash output.
         */
        private static byte[] HashFinalState(CNState cnState)
        {
            /* Get the state buffer as an array of ulongs rather than bytes */
            ulong[] hashState = cnState.GetHashState();

            Keccak.Keccak.keccakf(hashState, 24);

            /* Set the state buffer from the coerced hash state */
            cnState.SetHashState(hashState);

            /* Get the actual state buffer finally */
            byte[] state = cnState.GetState();

            IHashProvider p;

            /* Choose the final hashing function to use based on the value of
               state[0] */
            switch(state[0] % 4)
            {
                case 0:
                {
                    p = new Blake.Blake();
                    return p.Hash(state);
                }
                case 1:
                {
                    p = new Groestl.Groestl();
                    return p.Hash(state);
                }
                case 2:
                {
                    p = new JH.JH();
                    return p.Hash(state);
                }
                default:
                {
                    p = new Skein.Skein();
                    return p.Hash(state);
                }
            }
        }

        private static byte[] VariantOneInit(CNState state, byte[] input)
        {
            byte[] tweak = state.GetTweak();

            XOR64(tweak, input, 0, 35);

            return tweak;
        }

        private static void XOR64(byte[] a, byte[] b, int offsetA = 0,
                                  int offsetB = 0)
        {
            for (int i = 0; i < 8; i++)
            {
                a[i + offsetA] ^= b[i + offsetB];
            }
        }

        private static void VariantOneStepOne(byte[] scratchpad, int j)
        {
            int offset = (j * AES.Constants.BlockSize) + 11;

            byte tmp = scratchpad[offset];

            uint table = 0x75310;

            byte index = (byte)((((tmp >> 3) & 6) | (tmp & 1)) << 1);

            scratchpad[offset] = (byte)(tmp ^ ((table >> index) & 0x30));
        }

        private static void VariantOneStepTwo(byte[] c, int offset,
                                              byte[] tweak)
        {
            XOR64(c, tweak, offset, 0);
        }

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

        /* Thanks to https://stackoverflow.com/a/42426934/8737306 */
        private static void Multiply128(byte[] a, byte[] b, byte[] result)
        {
            /* Read 8 bytes from a and b as a ulong */
            ulong x = Encoding.ByteArrayToInteger<ulong>(a, 0, 8);
            ulong y = Encoding.ByteArrayToInteger<ulong>(b, 0, 8);

            ulong x_lo = x & 0xffffffff;
            ulong x_hi = x >> 32;

            ulong y_lo = y & 0xffffffff;
            ulong y_hi = y >> 32;

            ulong mul_lo = x_lo * y_lo;
            ulong mul_hi = (x_hi * y_lo) + (mul_lo >> 32);
            ulong mul_carry = (x_lo * y_hi) + (mul_hi & 0xffffffff);

            ulong result_hi = (x_hi * y_hi) + (mul_hi >> 32) 
                                            + (mul_carry >> 32);

            ulong result_lo = (mul_carry << 32) + (mul_lo & 0xffffffff);

            /* Convert the ulongs back into byte arrays */
            byte[] low = Encoding.IntegerToByteArray<ulong>(result_lo);
            byte[] high = Encoding.IntegerToByteArray<ulong>(result_hi);

            /* Copy the 8 high bits to result[0..7] */
            Buffer.BlockCopy(high, 0, result, 0, 8);
            /* Copy the 8 low bits to result[8..15] */
            Buffer.BlockCopy(low, 0, result, 8, 8);
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
        private static int e2i(byte[] input, int memorySize)
        {
            /* Read 8 bytes as a ulong */
            ulong j = Encoding.ByteArrayToInteger<ulong>(input, 0, 8);

            /* Divide by aes block size */
            j /= AES.Constants.BlockSize;

            /* Bitwise AND with (memorySize / blocksize) - 1*/
            return (int)(j & (ulong)(memorySize / AES.Constants.BlockSize - 1));
        }
    }
}
