//
// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using Canti.Data;

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
                    AES.AES.AESBPseudoRound(expandedKeys, text,
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
                    int j = E2I(mixingState.a, cnParams.Memory());

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

                    SwapBlocks(ref mixingState.a, ref mixingState.b);
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
                    int j = E2I(mixingState.a, cnParams.Memory());

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

                    SwapBlocks(ref mixingState.a, ref mixingState.b);

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
            AES.AES.AESBSingleRound(mixingState.a, mixingState.c, 0);

            XORBlocks(mixingState.b, mixingState.c);

            SwapBlocks(ref mixingState.b, ref mixingState.c);
        }

        private static void MixScratchpadIterationTwo(MixScratchpadState
                                                      mixingState)
        {
            /* Multiply a and c together, and place the result in
               b, a and c are ulongs stored as byte arrays, d is a
               128 bit value stored as a byte array */
            Multiply128(mixingState.a, mixingState.c, mixingState.d);

            SumHalfBlocks(mixingState.b, mixingState.d);

            SwapBlocks(ref mixingState.b, ref mixingState.c);

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
                    AES.AES.AESBPseudoRound(expandedKeys, text, offsetA);
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
            /* a0 = a[0..7] + b[0..7] */
            ulong a0 = Encoding.UnsafeByteArrayToUlong(a)
                     + Encoding.UnsafeByteArrayToUlong(b);

            /* b0 = a[8..15] + b[8..15] */
            ulong b0 = Encoding.UnsafeByteArrayToUlong(a, 8)
                     + Encoding.UnsafeByteArrayToUlong(b, 8);

            /* Copy a0 into a[0..7], and b0 into a[8..15] */
            Encoding.UnsafeWriteUlongToByteArray(a, a0, 0);
            Encoding.UnsafeWriteUlongToByteArray(a, b0, 8);
        }

        /* Thanks to https://stackoverflow.com/a/42426934/8737306 */
        private static void Multiply128(byte[] a, byte[] b, byte[] result)
        {
            /* Read 8 bytes from a and b as a ulong */
            ulong x = Encoding.UnsafeByteArrayToUlong(a);
            ulong y = Encoding.UnsafeByteArrayToUlong(b);

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

            /* Write result_hi to result[0..7] and result_lo to
             * result[8..15] */
            Encoding.UnsafeWriteUlongToByteArray(result, result_hi);
            Encoding.UnsafeWriteUlongToByteArray(result, result_lo, 8);
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

        /* 
          Swap the values of a and b 
          
          Dropped the loop.
          
          Old routine took 00:02:26 for 100000000 iterations
          New routine took 00:00:08 for 100000000 iterations
         */
        private static void SwapBlocks(ref byte[] a, ref byte[] b)
        {
            byte[] tmp = a;
            b = a;
            a = tmp;
        }
        

        /* Replaced division from routine
           Used the info in the following link to replace the division.          
           https://www.codeproject.com/Articles/17480/Optimizing-integer-divisions-with-Multiply-Shift-i

          Old routine took 00:00:41 for 100000000 iterations
          New routine took 00:00:26 for 100000000 iterations
         */
        private static int E2I(byte[] input, int memorySize)
        {
            /* Read 8 bytes as a ulong */
            ulong j = Encoding.UnsafeByteArrayToUlong(input);

            j = j * 1 >> 4;

            /* Bitwise AND with (memorySize / blocksize) - 1*/
            return (int)(j & (ulong)((memorySize * 1 >> 4) - 1));
        }
    }
}
