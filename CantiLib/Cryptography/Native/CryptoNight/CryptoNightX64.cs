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
    public static class X64
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

                CryptoNight.InitMixingState(a, b, (ulong *)k);

                Vector128<byte> _a;
                Vector128<byte> _b = Sse2.LoadVector128(bByte);
                Vector128<byte> _c;

                int j;

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* Get our 'memory' address we're using for this round */
                    j = CryptoNight.StateIndex(*a, cnParams.ScratchModulus);

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
                    j = CryptoNight.StateIndex(*c, cnParams.ScratchModulus);

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
        private unsafe static void MixScratchpadV1(
            CNState cnState,
            ICryptoNight cnParams,
            byte[] scratchpad, byte[] tweak)
        {
            byte[] aArray = new byte[AES.BLOCK_SIZE];
            byte[] bArray = new byte[AES.BLOCK_SIZE * 2];
            byte[] cArray = new byte[AES.BLOCK_SIZE];

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

                CryptoNight.InitMixingState(a, b, (ulong *)k);

                Vector128<byte> _a;
                Vector128<byte> _b = Sse2.LoadVector128(bByte);
                Vector128<byte> _c;

                int j;

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* Get our 'memory' address we're using for this round */
                    j = CryptoNight.StateIndex(*a, cnParams.ScratchModulus);

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

                    CryptoNight.VariantOneStepOne(scratchpadPtr + j);
                    
                    /* Get the new 'memory' address we're using for this round */
                    j = CryptoNight.StateIndex(*c, cnParams.ScratchModulus);

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

                    CryptoNight.VariantOneStepTwo(p + 1, tweak1_2);

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
        private static unsafe void MixScratchpadV2(
            CNState cnState,
            ICryptoNight cnParams,
            byte[] scratchpad)
        {
            byte[] aArray = new byte[AES.BLOCK_SIZE];
            byte[] bArray = new byte[AES.BLOCK_SIZE * 2];
            byte[] cArray = new byte[AES.BLOCK_SIZE];

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

                CryptoNight.InitMixingState((ulong *)a, (ulong *)b, (ulong *)k);

                int j;

                ulong *w = (ulong *)hashState;

                b[2] = w[8] ^ w[10];
                b[3] = w[9] ^ w[11];

                ulong divisionResult = w[12];
                ulong sqrtResult = w[13];

                Vector128<byte> _a;
                Vector128<byte> _b = Sse2.LoadVector128(bByte);
                Vector128<byte> _b1 = Sse2.LoadVector128(bByte + 16);
                Vector128<byte> _c;

                for (int i = 0; i < cnParams.AesRounds; i++)
                {
                    /* Get our 'memory' address we're using for this round */
                    j = CryptoNight.StateIndex(*a, cnParams.ScratchModulus);

                    /* Load C from the memory address in the scratchpad */
                    _c = Sse2.LoadVector128(scratchpadPtr + j);

                    /* Reload A from the buffer */
                    _a = Sse2.LoadVector128(aByte);

                    /* Use AES to mix scratchpad into c */
                    _c = Aes.Encrypt(_c, _a);

                    VariantTwoShuffleAdd(
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
                    j = CryptoNight.StateIndex(*c, cnParams.ScratchModulus);

                    /* Grab a pointer to the spot of memory we're interested */
                    p = (ulong *)(scratchpadPtr + j);

                    /* Load b from the memory address in the scratchpad */
                    b[0] = p[0];
                    b[1] = p[1];

                    VariantTwoIntegerMath(b, c, ref divisionResult, ref sqrtResult);

                    /* 64 bit multiply the low parts of C and B */
                    hi = Bmi2.X64.MultiplyNoFlags(c[0], b[0], loPtr);

                    *(ulong *)(scratchpadPtr + (j ^ 0x10)) ^= hi;
                    *((ulong *)(scratchpadPtr + (j ^ 0x10)) + 1) ^= lo;
                    hi ^= *(ulong *)(scratchpadPtr + (j ^ 0x20));
                    lo ^= *((ulong *)(scratchpadPtr + (j ^ 0x20)) + 1);

                    VariantTwoShuffleAdd(
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
            byte[] expandedKeys = AES.ExpandKey(cnState.GetAESKey2(), true);

            for (int i = 0; i < cnParams.InitRounds; i++)
            {
                AES.AESPseudoRoundXOR(
                    expandedKeys,
                    text,
                    scratchpad,
                    i * Constants.INIT_SIZE_BYTE
                );
            }

            cnState.SetText(text);
        }
        
        private unsafe static void VariantTwoShuffleAdd(
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
            uint divisor = (uint)((ptr[0] + (uint)(sqrtResult << 1)) | 0x80000001UL);
            divisionResult = ((uint)(dividend / divisor)) + ((ulong)(dividend % divisor) << 32);
            ulong sqrtInput = ptr[0] + divisionResult;

            Vector128<ulong> expDoubleBias = Vector128.Create(1023UL << 52, 0);
            Vector128<double> x = Sse2.Add(Sse2.X64.ConvertScalarToVector128UInt64(sqrtInput >> 12), expDoubleBias).AsDouble();
            x = Sse2.SqrtScalar(Vector128.Create(0).AsDouble(), x);
            sqrtResult = Sse2.X64.ConvertToUInt64(Sse2.Subtract(x.AsUInt64(), expDoubleBias)) >> 19;

            VariantTwoSqrtFixup(ref sqrtResult, sqrtInput);
        }

        private static void VariantTwoSqrtFixup(ref ulong r, ulong sqrtInput)
        {
            ulong s = r >> 1;
            ulong b = r & 1;
            ulong r2 = s * (s + b) + (r << 32);
            long tmp = ((r2 + b > sqrtInput) ? -1L : 0L) + ((r2 + (1UL << 32) < sqrtInput - s) ? 1L : 0L);
            r = (ulong)((long)r + tmp);
        }
    }
}
