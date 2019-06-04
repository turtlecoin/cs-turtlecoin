//
// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

using Canti.Data;

namespace Canti.Blockchain.Crypto.CryptoNight
{
    public static class CryptoNight
    {
        public static byte[] SlowHash(byte[] input, ICryptoNight cnParams)
        {
            switch(cnParams.Variant)
            {
                case 0:
                {
                    return CryptoNightV0(input, cnParams);
                }
                case 1:
                {
                    return CryptoNightV1(input, cnParams);
                }
                case 2:
                {
                    return CryptoNightV2(input, cnParams);
                }
                default:
                {
                    throw new ArgumentException(
                        "Variants other than 0, 1, and 2 are not supported!"
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

        private static byte[] CryptoNightV2(byte[] input, ICryptoNight cnParams)
        {
            /* CryptoNight Step 1: Use Keccak1600 to initialize the 'state'
             * buffer, encapsulated in cnState
             */
            CNState cnState = new CNState(Keccak.Keccak.keccak1600(input));

            byte[] scratchpad = FillScratchpad(cnState, cnParams);

            MixScratchpadV2(cnState, cnParams, scratchpad);

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
            byte[] expandedKeys = AES.AES.ExpandKey(cnState.GetAESKey(), cnParams.Intrinsics);

            /* Our large scratchpad, 2MB in default CN */
            byte[] scratchpad = new byte[cnParams.Memory];

            byte[] text = cnState.GetText();

            /* Fill the scratchpad with AES encryption of text */
            for (int i = 0; i < cnParams.InitRounds; i++)
            {
                AES.AES.AESPseudoRound(expandedKeys, text, cnParams.Intrinsics);

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
        private static unsafe void MixScratchpadV0(
            CNState cnState,
            ICryptoNight cnParams,
            byte[] scratchpad)
        {
            if (Sse2.IsSupported && Bmi2.IsSupported && cnParams.Intrinsics)
            {
                MixScratchpadV0Native(cnState, cnParams, scratchpad);
                return;
            }

            byte[] aArray = new byte[AES.Constants.BlockSize];
            byte[] bArray = new byte[AES.Constants.BlockSize * 2];
            byte[] cArray = new byte[AES.Constants.BlockSize];
            byte[] c1Array = new byte[AES.Constants.BlockSize];
            byte[] dArray = new byte[AES.Constants.BlockSize];

            fixed(byte *scratchpadPtr = scratchpad,
                        a = aArray,
                        b = bArray,
                        c = cArray,
                        c1 = c1Array,
                        d = dArray,
                        k = cnState.GetK())
            {
                InitMixingState((ulong *)a, (ulong *)b, (ulong *)k);

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* ITERATION ONE */

                    /* Get the memory address */
                    int j = StateIndex(*(ulong *)a, cnParams.ScratchModulus);

                    /* Get a pointer to the memory address in the scratchpad */
                    ulong *p = (ulong *)(scratchpadPtr + j);
                    
                    /* Perform AES round from/to scratchpad, with A as the
                       expanded key */
                    AES.AES.AESBSingleRound((uint *)p, (uint *)a);

                    /* Copy a block from p to c1 */
                    CopyBlock(c1, p);

                    XORBlocks(p, (ulong *)b);

                    /* ITERATION TWO */

                    /* Get new memory address */
                    j = StateIndex(*(ulong *)c1, cnParams.ScratchModulus);

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
        private static unsafe void MixScratchpadV0Native(
            CNState cnState,
            ICryptoNight cnParams,
            byte[] scratchpad)
        {
            byte[] aArray = new byte[AES.Constants.BlockSize];
            byte[] bArray = new byte[AES.Constants.BlockSize * 2];
            byte[] cArray = new byte[AES.Constants.BlockSize];

            fixed(byte* scratchpadPtr = scratchpad,
                        aByte = aArray,
                        bByte = bArray,
                        cByte = cArray,
                        k = cnState.GetK())
            {
                ulong lo;
                ulong hi;

                ulong *loPtr = &lo;
                ulong *p;

                ulong *a = (ulong *)aByte;
                ulong *b = (ulong *)bByte;
                ulong *c = (ulong *)cByte;

                InitMixingState(a, b, (ulong *)k);

                Vector128<byte> _a;
                Vector128<byte> _b = Sse2.LoadVector128(bByte);
                Vector128<byte> _c;

                int j;

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* Get our 'memory' address we're using for this round */
                    j = StateIndex(*a, cnParams.ScratchModulus);

                    /* Load C from the memory address in the scratchpad */
                    _c = Sse2.LoadVector128(scratchpadPtr + j);

                    /* Reload A from the buffer */
                    _a = Sse2.LoadVector128(aByte);

                    /* Use AES to mix scratchpad into c */
                    _c = Aes.Encrypt(_c, _a);

                    /* Store C back in it's original spot */
                    Sse2.Store(cByte, _c);

                    /* XOR b and C, and store back at the 'memory' address
                       in the scratchpad */
                    Sse2.Store(scratchpadPtr + j, Sse2.Xor(_b, _c));
                    
                    /* Get the new 'memory' address we're using for this round */
                    j = StateIndex(*c, cnParams.ScratchModulus);

                    /* Grab a pointer to the spot of memory we're interested */
                    p = (ulong *)(scratchpadPtr + j);

                    /* Load b from the memory address in the scratchpad */
                    b[0] = p[0];
                    b[1] = p[1];

                    /* 64 bit multiply the low parts of C and B */
                    hi = Bmi2.X64.MultiplyNoFlags(c[0], b[0], loPtr);

                    /* Sum the result halves of the multiplication with each half of A */
                    a[0] += hi;
                    a[1] += lo;

                    /* Write the modified A back to the scratchpad */
                    p[0] = a[0];
                    p[1] = a[1];

                    /* Xor the two 64 bit halves of A and B */
                    a[0] ^= b[0];
                    a[1] ^= b[1];

                    /* Store C in B */
                    _b = _c;
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
            if (Sse2.IsSupported && Bmi2.IsSupported && cnParams.Intrinsics)
            {
                MixScratchpadV1Native(cnState, cnParams, scratchpad, tweak);
                return;
            }

            byte[] aArray = new byte[AES.Constants.BlockSize];
            byte[] bArray = new byte[AES.Constants.BlockSize * 2];
            byte[] cArray = new byte[AES.Constants.BlockSize];
            byte[] c1Array = new byte[AES.Constants.BlockSize];
            byte[] dArray = new byte[AES.Constants.BlockSize];

            fixed(byte *scratchpadPtr = scratchpad,
                        a = aArray,
                        b = bArray,
                        c = cArray,
                        c1 = c1Array,
                        d = dArray,
                        k = cnState.GetK(),
                        tweakByte = tweak)
            {
                InitMixingState((ulong *)a, (ulong *)b, (ulong *)k);

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* ITERATION ONE */

                    /* Get the memory address */
                    int j = StateIndex(*(ulong *)a, cnParams.ScratchModulus);

                    /* Get a pointer to the memory address in the scratchpad */
                    ulong *p = (ulong *)(scratchpadPtr + j);
                    
                    /* Perform AES round from/to scratchpad, with A as the
                       expanded key */
                    AES.AES.AESBSingleRound((uint *)p, (uint *)a);

                    /* Copy a block from p to c1 */
                    CopyBlock(c1, p);

                    XORBlocks(p, (ulong *)b);

                    VariantOneStepOnePtr((byte *)p);

                    /* ITERATION TWO */

                    /* Get new memory address */
                    j = StateIndex(*(ulong *)c1, cnParams.ScratchModulus);

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

                    VariantOneStepTwoPtr((ulong *)(c + 8), (ulong *)tweakByte);

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
        private unsafe static void MixScratchpadV1Native(
            CNState cnState,
            ICryptoNight cnParams,
            byte[] scratchpad, byte[] tweak)
        {
            byte[] aArray = new byte[AES.Constants.BlockSize];
            byte[] bArray = new byte[AES.Constants.BlockSize * 2];
            byte[] cArray = new byte[AES.Constants.BlockSize];

            fixed(byte* scratchpadPtr = scratchpad,
                        aByte = aArray,
                        bByte = bArray,
                        cByte = cArray,
                        k = cnState.GetK(),
                        tweakByte = tweak)
            {
                ulong lo;
                ulong hi;

                ulong *loPtr = &lo;
                ulong *p;

                ulong *a = (ulong *)aByte;
                ulong *b = (ulong *)bByte;
                ulong *c = (ulong *)cByte;
                ulong *tweak1_2 = (ulong *)tweakByte;

                InitMixingState(a, b, (ulong *)k);

                Vector128<byte> _a;
                Vector128<byte> _b = Sse2.LoadVector128(bByte);
                Vector128<byte> _c;

                int j;

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* Get our 'memory' address we're using for this round */
                    j = StateIndex(*a, cnParams.ScratchModulus);

                    /* Load C from the memory address in the scratchpad */
                    _c = Sse2.LoadVector128(scratchpadPtr + j);

                    /* Reload A from the buffer */
                    _a = Sse2.LoadVector128(aByte);

                    /* Use AES to mix scratchpad into c */
                    _c = Aes.Encrypt(_c, _a);

                    /* Store C back in it's original spot */
                    Sse2.Store(cByte, _c);

                    /* XOR b and C, and store back at the 'memory' address
                       in the scratchpad */
                    Sse2.Store(scratchpadPtr + j, Sse2.Xor(_b, _c));

                    VariantOneStepOnePtr(scratchpadPtr + j);
                    
                    /* Get the new 'memory' address we're using for this round */
                    j = StateIndex(*c, cnParams.ScratchModulus);

                    /* Grab a pointer to the spot of memory we're interested */
                    p = (ulong *)(scratchpadPtr + j);

                    /* Load b from the memory address in the scratchpad */
                    b[0] = p[0];
                    b[1] = p[1];

                    /* 64 bit multiply the low parts of C and B */
                    hi = Bmi2.X64.MultiplyNoFlags(c[0], b[0], loPtr);

                    /* Sum the result halves of the multiplication with each half of A */
                    a[0] += hi;
                    a[1] += lo;

                    /* Write the modified A back to the scratchpad */
                    p[0] = a[0];
                    p[1] = a[1];

                    /* Xor the two 64 bit halves of A and B */
                    a[0] ^= b[0];
                    a[1] ^= b[1];

                    VariantOneStepTwoPtr(p + 1, tweak1_2);

                    /* Store C in B */
                    _b = _c;
                }
            }
        }

        private static unsafe void MixScratchpadV2(
            CNState cnState,
            ICryptoNight cnParams,
            byte[] scratchpad)
        {
            if (Sse2.IsSupported && Bmi2.IsSupported && cnParams.Intrinsics)
            {
                MixScratchpadV2Native(cnState, cnParams, scratchpad);
                return;
            }

            byte[] aArray = new byte[AES.Constants.BlockSize];
            byte[] bArray = new byte[AES.Constants.BlockSize * 2];
            byte[] cArray = new byte[AES.Constants.BlockSize];
            byte[] c1Array = new byte[AES.Constants.BlockSize];
            byte[] dArray = new byte[AES.Constants.BlockSize];

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

                InitMixingState((ulong *)a, (ulong *)b, (ulong *)k);

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
                    int j = StateIndex(*(ulong *)a, cnParams.ScratchModulus);

                    /* Get a pointer to the memory address in the scratchpad */
                    ulong *p = (ulong *)(scratchpadPtr + j);

                    /* Perform AES round from/to scratchpad, with A as the
                       expanded key */
                    AES.AES.AESBSingleRound((uint *)p, (uint *)a);

                    /* Copy a block from p to c1 */
                    CopyBlock(c1, p);

                    VariantTwoShuffleAdd(scratchpadPtr, j, (ulong *)b, (ulong *)a);

                    XORBlocks(p, (ulong *)b);

                    /* ITERATION TWO */

                    /* Get new memory address */
                    j = StateIndex(*(ulong *)c1, cnParams.ScratchModulus);

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
                    CopyBlock(b + AES.Constants.BlockSize, b);

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
        private static unsafe void MixScratchpadV2Native(
            CNState cnState,
            ICryptoNight cnParams,
            byte[] scratchpad)
        {
            byte[] aArray = new byte[AES.Constants.BlockSize];
            byte[] bArray = new byte[AES.Constants.BlockSize * 2];
            byte[] cArray = new byte[AES.Constants.BlockSize];

            fixed(byte* scratchpadPtr = scratchpad,
                        aByte = aArray,
                        bByte = bArray,
                        cByte = cArray,
                        k = cnState.GetK(),
                        hashState = cnState.GetState())
            {
                ulong lo;
                ulong hi;

                ulong *loPtr = &lo;
                ulong *p;

                ulong *a = (ulong *)aByte;
                ulong *b = (ulong *)bByte;
                ulong *c = (ulong *)cByte;

                Vector128<byte> _a;
                Vector128<byte> _b = Sse2.LoadVector128(bByte);
                Vector128<byte> _b1 = Sse2.LoadVector128(bByte + 1);
                Vector128<byte> _c;

                int j;

                ulong *w = (ulong *)hashState;

                InitMixingState((ulong *)a, (ulong *)b, (ulong *)k);

                b[2] = w[8] ^ w[10];
                b[3] = w[9] ^ w[11];

                ulong divisionResult = w[12];
                ulong sqrtResult = w[13];

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* Get our 'memory' address we're using for this round */
                    j = StateIndex(*a, cnParams.ScratchModulus);

                    /* Load C from the memory address in the scratchpad */
                    _c = Sse2.LoadVector128(scratchpadPtr + j);

                    /* Reload A from the buffer */
                    _a = Sse2.LoadVector128(aByte);

                    /* Use AES to mix scratchpad into c */
                    _c = Aes.Encrypt(_c, _a);

                    VariantTwoShuffleAddNative(
                        scratchpadPtr,
                        j,
                        _b1,
                        _b,
                        _a
                    );

                    /* Store C back in it's original spot */
                    Sse2.Store(cByte, _c);

                    /* XOR b and C, and store back at the 'memory' address
                       in the scratchpad */
                    Sse2.Store(scratchpadPtr + j, Sse2.Xor(_b, _c));
                    
                    /* Get the new 'memory' address we're using for this round */
                    j = StateIndex(*c, cnParams.ScratchModulus);

                    /* Grab a pointer to the spot of memory we're interested */
                    p = (ulong *)(scratchpadPtr + j);

                    /* Load b from the memory address in the scratchpad */
                    b[0] = p[0];
                    b[1] = p[1];

                    VariantTwoIntegerMathNative(b, c, ref divisionResult, ref sqrtResult);

                    /* 64 bit multiply the low parts of C and B */
                    hi = Bmi2.X64.MultiplyNoFlags(c[0], b[0], loPtr);

                    *(ulong *)(scratchpadPtr + (j ^ 0x10)) ^= hi;
                    *(ulong *)(scratchpadPtr + (j ^ 0x10) + 1) ^= lo;
                    hi ^= *(ulong *)(scratchpadPtr + (j ^ 0x20));
                    lo ^= *(ulong *)(scratchpadPtr + (j ^ 0x20) + 1);

                    VariantTwoShuffleAddNative(
                        scratchpadPtr,
                        j,
                        _b1,
                        _b,
                        _a
                    );

                    /* Sum the result halves of the multiplication with each half of A */
                    a[0] += hi;
                    a[1] += lo;

                    /* Write the modified A back to the scratchpad */
                    p[0] = a[0];
                    p[1] = a[1];

                    /* Xor the two 64 bit halves of A and B */
                    a[0] ^= b[0];
                    a[1] ^= b[1];

                    _b1 = _b;
                    /* Store C in B */
                    _b = _c;
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
            byte[] expandedKeys = AES.AES.ExpandKey(cnState.GetAESKey2(), cnParams.Intrinsics);

            if (Aes.IsSupported && cnParams.Intrinsics)
            {
                for (int i = 0; i < cnParams.InitRounds; i++)
                {
                    AES.AES.AESPseudoRoundXOR(
                        expandedKeys,
                        text,
                        scratchpad,
                        i * Constants.InitSizeByte
                    );
                }
            }
            else
            {
                for (int i = 0; i < cnParams.InitRounds; i++)
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

        private unsafe static void VariantTwoShuffleAddNative(
            byte *basePtr,
            int offset,
            Vector128<byte> _b1,
            Vector128<byte> _b,
            Vector128<byte> _a)
        {
            Vector128<ulong> chunk1 = Sse2.LoadVector128((ulong *)(basePtr + (offset ^ 0x10)));
            Vector128<ulong> chunk2 = Sse2.LoadVector128((ulong *)(basePtr + (offset ^ 0x20)));
            Vector128<ulong> chunk3 = Sse2.LoadVector128((ulong *)(basePtr + (offset ^ 0x30)));

            Sse2.Store((ulong *)(basePtr + (offset ^ 0x10)), Sse2.Add(chunk3, _b1.AsUInt64()));
            Sse2.Store((ulong *)(basePtr + (offset ^ 0x20)), Sse2.Add(chunk1, _b.AsUInt64()));
            Sse2.Store((ulong *)(basePtr + (offset ^ 0x30)), Sse2.Add(chunk2, _a.AsUInt64()));
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

        private unsafe static void VariantTwoIntegerMathNative(
            ulong *b,
            ulong *ptr,
            ref ulong divisionResult,
            ref ulong sqrtResult)
        {
            b[0] ^= divisionResult ^ (sqrtResult << 32);
            ulong dividend = ptr[1];
            uint divisor = (uint)(ptr[0] + (uint)(sqrtResult << 1)) | 0x80000001;
            divisionResult = ((uint)(dividend / divisor)) + ((ulong)(dividend % divisor) << 32);
            ulong sqrtInput = ptr[0] + divisionResult;

            Vector128<ulong> expDoubleBias = Vector128.Create(0, 1023UL << 52);

            /* TODO: Check for 64 bit support */
            Vector128<ulong> x = Sse2.Add(Sse2.X64.ConvertScalarToVector128UInt64(sqrtInput >> 12), expDoubleBias);
            sqrtResult = Sse2.X64.ConvertToUInt64(Sse2.Subtract(x, expDoubleBias)) >> 19;

            VariantTwoSqrtFixup(ref sqrtResult, sqrtInput);
        }

        private static void VariantTwoSqrtFixup(ref ulong r, ulong sqrtInput)
        {
            ulong s = r >> 1;
            ulong b = r & 1;
            ulong r2 = s * (s + b) + (r << 32);
            r = (ulong)((long)r + ((r2 + b > sqrtInput) ? -1L : 0L) + ((r2 + (1 << 32) < sqrtInput - s) ? 1L : 0L));
        }

        private static void XOR64(byte[] a, byte[] b, int offsetA = 0,
                                  int offsetB = 0)
        {
            for (int i = 0; i < 8; i++)
            {
                a[i + offsetA] ^= b[i + offsetB];
            }
        }

        private unsafe static void VariantOneStepOnePtr(byte* scratchpad)
        {
            byte tmp = scratchpad[11];

            uint table = 0x75310;

            byte index = (byte)((((tmp >> 3) & 6) | (tmp & 1)) << 1);

            scratchpad[11] = (byte)(tmp ^ ((table >> index) & 0x30));
        }

        private unsafe static void VariantOneStepTwoPtr(ulong *a, ulong *b)
        {
            *a ^= *b;
        }

        private static void VariantOneStepTwo(byte[] c, int offset,
                                              byte[] tweak)
        {
            XOR64(c, tweak, offset, 0);
        }

        private static unsafe void InitMixingState(
            ulong *a,
            ulong *b,
            ulong *k)
        {
            a[0] = (k + 0)[0] ^ (k + 4)[0];
            a[1] = (k + 0)[1] ^ (k + 4)[1];
            b[0] = (k + 2)[0] ^ (k + 6)[0];
            b[1] = (k + 2)[1] ^ (k + 6)[1];
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

        /* XOR a and b, with optional offsets in the array */
        private static void XORBlocks(byte[] a, byte[] b, int offsetA = 0,
                                                          int offsetB = 0)
        {
            for (int i = 0; i < AES.Constants.BlockSize; i++)
            {
                a[i + offsetA] ^= b[i + offsetB];
            }
        }

        private static unsafe void XORBlocks(ulong *a, ulong *b)
        {
            a[0] ^= b[0];
            a[1] ^= b[1];
        }

        private static unsafe void CopyBlock(void *destination, void *source)
        {
            Buffer.MemoryCopy(
                source,
                destination,
                AES.Constants.BlockSize,
                AES.Constants.BlockSize
            );
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
        
        private static int StateIndex(ulong j, int modulus)
        {
            return (int)(j >> 4 & (ulong)modulus) << 4;
        }
    }
}
