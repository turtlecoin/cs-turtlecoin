//
// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Canti.Cryptography.Native.CryptoNight
{
    public static class Portable
    {
        public static byte[] CryptoNightV0(byte[] input, ICryptoNight cnParams)
        {
            /* CryptoNight Step 1: Use Keccak1600 to initialize the 'state'
             * buffer, encapsulated in cnState
             */
            CNState cnState = new CNState(Keccak.Keccak1600(input));

            byte[] scratchpad = CryptoNight.FillScratchpad(cnState, cnParams);

            MixScratchpadV0(cnState, cnParams, scratchpad);

            EncryptScratchpadToText(cnState, cnParams, scratchpad);

            return CryptoNight.HashFinalState(cnState);
        }

        public static byte[] CryptoNightV1(byte[] input, ICryptoNight cnParams)
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
            CNState cnState = new CNState(Keccak.Keccak1600(input));

            byte[] scratchpad = CryptoNight.FillScratchpad(cnState, cnParams);

            byte[] tweak = CryptoNight.VariantOneInit(cnState, input);

            MixScratchpadV1(cnState, cnParams, scratchpad, tweak);

            EncryptScratchpadToText(cnState, cnParams, scratchpad);

            return CryptoNight.HashFinalState(cnState);
        }

        public static byte[] CryptoNightV2(byte[] input, ICryptoNight cnParams)
        {
            /* CryptoNight Step 1: Use Keccak1600 to initialize the 'state'
             * buffer, encapsulated in cnState
             */
            CNState cnState = new CNState(Keccak.Keccak1600(input));

            byte[] scratchpad = CryptoNight.FillScratchpad(cnState, cnParams);

            MixScratchpadV2(cnState, cnParams, scratchpad);

            EncryptScratchpadToText(cnState, cnParams, scratchpad);

            return CryptoNight.HashFinalState(cnState);
        }

        /* CryptoNight Step 3: Bounce randomly 1,048,576 times (1<<20)
         * through the mixing scratchpad, using 524,288 iterations of the
         * following mixing function.  Each execution performs two reads
         * and writes from the mixing scratchpad.
         */
        private static unsafe void MixScratchpadV0(
            CNState cnState,
            ICryptoNight cnParams,
            byte[] scratchpad)
        {
            byte[] aArray = new byte[AES.BLOCK_SIZE];
            byte[] bArray = new byte[AES.BLOCK_SIZE * 2];
            byte[] cArray = new byte[AES.BLOCK_SIZE];
            byte[] c1Array = new byte[AES.BLOCK_SIZE];
            byte[] dArray = new byte[AES.BLOCK_SIZE];

            fixed(byte *scratchpadPtr = scratchpad,
                        a = aArray,
                        b = bArray,
                        c = cArray,
                        c1 = c1Array,
                        d = dArray,
                        k = cnState.GetK())
            {
                CryptoNight.InitMixingState((ulong *)a, (ulong *)b, (ulong *)k);

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* ITERATION ONE */

                    /* Get the memory address */
                    int j = CryptoNight.StateIndex(*(ulong *)a, cnParams.ScratchModulus);

                    /* Get a pointer to the memory address in the scratchpad */
                    ulong *p = (ulong *)(scratchpadPtr + j);
                    
                    /* Perform AES round from/to scratchpad, with A as the
                       expanded key */
                    AES.AESBSingleRound((uint *)p, (uint *)a);

                    /* Copy a block from p to c1 */
                    CopyBlock(c1, p);

                    XORBlocks(p, (ulong *)b);

                    /* ITERATION TWO */

                    /* Get new memory address */
                    j = CryptoNight.StateIndex(*(ulong *)c1, cnParams.ScratchModulus);

                    /* Update pointer to scratchpad */
                    p = (ulong *)(scratchpadPtr + j);

                    /* Copy a block from p to c */
                    CopyBlock(c, p);

                    Multiply64((ulong *)c1, (ulong *)c, (ulong *)d);

                    /* Sum half blocks */
                    ((ulong *)a)[0] += ((ulong *)d)[0];
                    ((ulong *)a)[1] += ((ulong *)d)[1];

                    SwapBlocks((ulong *)a, (ulong *)c);

                    XORBlocks((ulong *)a, (ulong *)c);

                    /* Copy a block from c to p */
                    CopyBlock(p, c);
                    
                    /* Copy a block from c1 to b */
                    CopyBlock(b, c1);
                }
            }
        }
        
        /* CryptoNight Step 3: Bounce randomly 1,048,576 times (1<<20)
         * through the mixing scratchpad, using 524,288 iterations of the
         * following mixing function.  Each execution performs two reads
         * and writes from the mixing scratchpad.
         */
        private static unsafe void MixScratchpadV1(
            CNState cnState,
            ICryptoNight cnParams,
            byte[] scratchpad,
            byte[] tweak)
        {
            byte[] aArray = new byte[AES.BLOCK_SIZE];
            byte[] bArray = new byte[AES.BLOCK_SIZE * 2];
            byte[] cArray = new byte[AES.BLOCK_SIZE];
            byte[] c1Array = new byte[AES.BLOCK_SIZE];
            byte[] dArray = new byte[AES.BLOCK_SIZE];

            fixed(byte *scratchpadPtr = scratchpad,
                        a = aArray,
                        b = bArray,
                        c = cArray,
                        c1 = c1Array,
                        d = dArray,
                        k = cnState.GetK(),
                        tweakByte = tweak)
            {
                CryptoNight.InitMixingState((ulong *)a, (ulong *)b, (ulong *)k);

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* ITERATION ONE */

                    /* Get the memory address */
                    int j = CryptoNight.StateIndex(*(ulong *)a, cnParams.ScratchModulus);

                    /* Get a pointer to the memory address in the scratchpad */
                    ulong *p = (ulong *)(scratchpadPtr + j);
                    
                    /* Perform AES round from/to scratchpad, with A as the
                       expanded key */
                    AES.AESBSingleRound((uint *)p, (uint *)a);

                    /* Copy a block from p to c1 */
                    CopyBlock(c1, p);

                    XORBlocks(p, (ulong *)b);

                    CryptoNight.VariantOneStepOne((byte *)p);

                    /* ITERATION TWO */

                    /* Get new memory address */
                    j = CryptoNight.StateIndex(*(ulong *)c1, cnParams.ScratchModulus);

                    /* Update pointer to scratchpad */
                    p = (ulong *)(scratchpadPtr + j);

                    /* Copy a block from p to c */
                    CopyBlock(c, p);

                    Multiply64((ulong *)c1, (ulong *)c, (ulong *)d);

                    /* Sum half blocks */
                    ((ulong *)a)[0] += ((ulong *)d)[0];
                    ((ulong *)a)[1] += ((ulong *)d)[1];

                    SwapBlocks((ulong *)a, (ulong *)c);

                    XORBlocks((ulong *)a, (ulong *)c);

                    CryptoNight.VariantOneStepTwo((ulong *)(c + 8), (ulong *)tweakByte);

                    /* Copy a block from c to p */
                    CopyBlock(p, c);
                    
                    /* Copy a block from c1 to b */
                    CopyBlock(b, c1);
                }
            }
        }

        private static unsafe void MixScratchpadV2(
            CNState cnState,
            ICryptoNight cnParams,
            byte[] scratchpad)
        {
            byte[] aArray = new byte[AES.BLOCK_SIZE];
            byte[] bArray = new byte[AES.BLOCK_SIZE * 2];
            byte[] cArray = new byte[AES.BLOCK_SIZE];
            byte[] c1Array = new byte[AES.BLOCK_SIZE];
            byte[] dArray = new byte[AES.BLOCK_SIZE];

            fixed(byte *scratchpadPtr = scratchpad,
                        a = aArray,
                        b = bArray,
                        c = cArray,
                        c1 = c1Array,
                        d = dArray,
                        k = cnState.GetK(),
                        hashState = cnState.GetState())
            {
                ulong divisionResult;
                ulong sqrtResult;

                CryptoNight.InitMixingState((ulong *)a, (ulong *)b, (ulong *)k);

                /* Init */
                {
                    ulong *_b = (ulong *)b;
                    ulong *w = (ulong *)hashState;

                    _b[2] = w[8] ^ w[10];
                    _b[3] = w[9] ^ w[11];

                    divisionResult = w[12];
                    sqrtResult = w[13];
                }

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* ITERATION ONE */

                    /* Get the memory address */
                    int j = CryptoNight.StateIndex(*(ulong *)a, cnParams.ScratchModulus);

                    /* Get a pointer to the memory address in the scratchpad */
                    ulong *p = (ulong *)(scratchpadPtr + j);

                    /* Perform AES round from/to scratchpad, with A as the
                       expanded key */
                    AES.AESBSingleRound((uint *)p, (uint *)a);

                    /* Copy a block from p to c1 */
                    CopyBlock(c1, p);

                    VariantTwoShuffleAdd(scratchpadPtr, j, (ulong *)b, (ulong *)a);

                    XORBlocks(p, (ulong *)b);

                    /* ITERATION TWO */

                    /* Get new memory address */
                    j = CryptoNight.StateIndex(*(ulong *)c1, cnParams.ScratchModulus);

                    /* Update pointer to scratchpad */
                    p = (ulong *)(scratchpadPtr + j);

                    /* Copy a block from p to c */
                    CopyBlock(c, p);

                    VariantTwoIntegerMath(
                        (ulong *)c,
                        (ulong *)c1,
                        ref divisionResult,
                        ref sqrtResult
                    );

                    Multiply64((ulong *)c1, (ulong *)c, (ulong *)d);

                    /* Variant 2 */
                    XORBlocks((ulong *)(scratchpadPtr + (j ^ 0x10)), (ulong *)d);
                    XORBlocks((ulong *)d, (ulong *)(scratchpadPtr + (j ^ 0x20)));

                    VariantTwoShuffleAdd(scratchpadPtr, j, (ulong *)b, (ulong *)a);

                    /* Sum half blocks */
                    ((ulong *)a)[0] += ((ulong *)d)[0];
                    ((ulong *)a)[1] += ((ulong *)d)[1];

                    SwapBlocks((ulong *)a, (ulong *)c);

                    XORBlocks((ulong *)a, (ulong *)c);

                    /* Copy a block from c to p */
                    CopyBlock(p, c);

                    /* Copy first half of b to second half */
                    CopyBlock(b + AES.BLOCK_SIZE, b);

                    /* Copy a block from c1 to b */
                    CopyBlock(b, c1);
                }
            }
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
            byte[] expandedKeys = AES.ExpandKey(cnState.GetAESKey2(), false);

            for (int i = 0; i < cnParams.InitRounds; i++)
            {
                for (int j = 0; j < Constants.INIT_SIZE_BLOCK; j++)
                {
                    int offsetA = j * AES.BLOCK_SIZE;
                    int offsetB = (i * Constants.INIT_SIZE_BYTE) + offsetA;

                    XORBlocks(text, scratchpad, offsetA, offsetB);

                    /* Need to pass the array with an offset because we manip
                       it in place */
                    AES.AESBPseudoRound(expandedKeys, text, offsetA);
                }
            }

            cnState.SetText(text);
        }
        
        
        private unsafe static void VariantTwoShuffleAdd(
            byte *basePtr,
            int offset,
            ulong *b,
            ulong *a)
        {
            ulong *chunk1 = (ulong *)(basePtr + (offset ^ 0x10));
            ulong *chunk2 = (ulong *)(basePtr + (offset ^ 0x20));
            ulong *chunk3 = (ulong *)(basePtr + (offset ^ 0x30));

            ulong[] chunk1_old = new ulong[] { chunk1[0], chunk1[1] };

            chunk1[0] = chunk3[0] + b[2];
            chunk1[1] = chunk3[1] + b[3];

            chunk3[0] = chunk2[0] + a[0];
            chunk3[1] = chunk2[1] + a[1];

            chunk2[0] = chunk1_old[0] + b[0];
            chunk2[1] = chunk1_old[1] + b[1];
        }

        private unsafe static void VariantTwoIntegerMath(
            ulong *b,
            ulong *ptr,
            ref ulong divisionResult,
            ref ulong sqrtResult)
        {
            b[0] ^= divisionResult ^ (sqrtResult << 32);

            ulong dividend = ptr[1];
            uint divisor = (uint)(ptr[0] + (sqrtResult << 1)) | 0x80000001;

            divisionResult = ((uint)(dividend / divisor)) + ((ulong)(dividend % divisor) << 32);

            ulong sqrtInput = ptr[0] + divisionResult;

            sqrtResult = IntegerSquareRootV2(sqrtInput);
        }

        private static uint IntegerSquareRootV2(ulong n)
        {
            ulong r = 1UL << 63;

            for (ulong bit = 1UL << 60; bit != 0; bit >>= 2)
            {
                bool b = (n < r + bit);
                ulong nNext = n - (r + bit);
                ulong rNext = r + bit * 2;
                n = b ? n : nNext;
                r = b ? r : rNext;
                r >>= 1;
            }

            return (uint)(r * 2 + ((n > r) ? 1UL : 0UL));
        }

        private static unsafe void Multiply64(ulong *a, ulong *b, ulong *result)
        {
            result[0] = Multiply64(*a, *b, result + 1);
        }

        private static unsafe ulong Multiply64(ulong x, ulong y, ulong *lowPointer)
        {
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

            *lowPointer = result_lo;

            return result_hi;
        }

        private static unsafe void CopyBlock(void *destination, void *source)
        {
            Buffer.MemoryCopy(source, destination, AES.BLOCK_SIZE, AES.BLOCK_SIZE);
        }

        /* Swap A and B - Can't swap the underlying arrays since the pointer
           is fixed to the original position. Can't swap pointers either in
           a fixed context. */
        private static unsafe void SwapBlocks(ulong *a, ulong *b)
        {
            ulong[] tmp = new ulong[2];
            tmp[0] = a[0];
            tmp[1] = a[1];

            a[0] = b[0];
            a[1] = b[1];

            b[0] = tmp[0];
            b[1] = tmp[1];
        }

        /* XOR a and b, with optional offsets in the array */
        public static void XORBlocks(byte[] a, byte[] b, int offsetA = 0, int offsetB = 0)
        {
            for (int i = 0; i < AES.BLOCK_SIZE; i++)
            {
                a[i + offsetA] ^= b[i + offsetB];
            }
        }

        public static unsafe void XORBlocks(ulong *a, ulong *b)
        {
            a[0] ^= b[0];
            a[1] ^= b[1];
        }
    }
}
