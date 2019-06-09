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
    public static partial class CryptoNight
    {
        public static byte[] SlowHash(byte[] input, ICryptoNight cnParams)
        {
            bool useX64Version = Aes.IsSupported &&  Bmi2.X64.IsSupported &&
                                 Sse2.IsSupported && cnParams.Intrinsics;

            switch(cnParams.Variant)
            {
                case 0:
                {
                    if (useX64Version)
                    {
                        return X64.CryptoNightV0(input, cnParams);
                    }
                    else
                    {
                        return Portable.CryptoNightV0(input, cnParams);
                    }
                }
                case 1:
                {
                    if (useX64Version)
                    {
                        return X64.CryptoNightV1(input, cnParams);
                    }
                    else
                    {
                        return Portable.CryptoNightV1(input, cnParams);
                    }
                }
                case 2:
                {
                    if (useX64Version)
                    {
                        return X64.CryptoNightV2(input, cnParams);
                    }
                    else
                    {
                        return Portable.CryptoNightV2(input, cnParams);
                    }
                }
                default:
                {
                    throw new ArgumentException(
                        "Variants other than 0, 1, and 2 are not supported!"
                    );
                }
            }
        }

        /* CryptoNight Step 2:  Iteratively encrypt the results from Keccak
         * to fill the large scratchpad
         */
        public static byte[] FillScratchpad(CNState cnState, ICryptoNight cnParams)
        {
            /* Expand our initial key into many for each round of pseudo aes */
            byte[] expandedKeys = AES.ExpandKey(cnState.GetAESKey(), cnParams.Intrinsics);

            /* Our large scratchpad, 2MB in default CN */
            byte[] scratchpad = new byte[cnParams.Memory];

            byte[] text = cnState.GetText();

            if (cnParams.Intrinsics && Aes.IsSupported && Sse2.IsSupported)
            {
                /* Fill the scratchpad with AES encryption of text */
                for (int i = 0; i < cnParams.InitRounds; i++)
                {
                    AES.AESPseudoRoundNative(expandedKeys, text);

                    /* Write text to the scratchpad, at the offset
                       i * InitSizeByte */
                    Buffer.BlockCopy(text, 0, scratchpad, i * Constants.INIT_SIZE_BYTE, text.Length);
                }
            }
            else
            {
                /* Fill the scratchpad with AES encryption of text */
                for (int i = 0; i < cnParams.InitRounds; i++)
                {
                    AES.AESPseudoRound(expandedKeys, text, cnParams.Intrinsics);

                    /* Write text to the scratchpad, at the offset
                       i * InitSizeByte */
                    Buffer.BlockCopy(text, 0, scratchpad, i * Constants.INIT_SIZE_BYTE, text.Length);
                }
            }

            return scratchpad;
        }

        /* CryptoNight Step 5: Apply Keccak to the state again, and then
         * use the resulting data to select which of four finalizer
         * hash functions to apply to the data (Blake, Groestl, JH,
         * or Skein). Use this hash to squeeze the state array down
         * to the final 32 byte hash output.
         */
        public static byte[] HashFinalState(CNState cnState)
        {
            /* Get the state buffer as an array of ulongs rather than bytes */
            ulong[] hashState = cnState.GetHashState();

            Keccak.Keccakf(hashState, 24);

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
                    return new Blake().Hash(state);
                }
                case 1:
                {
                    return new Groestl().Hash(state);
                }
                case 2:
                {
                    return new JH().Hash(state);
                }
                default:
                {
                    return new Skein().Hash(state);
                }
            }
        }

        public static byte[] VariantOneInit(CNState state, byte[] input)
        {
            byte[] tweak = state.GetTweak();

            XOR64(tweak, input, 0, 35);

            return tweak;
        }

        public unsafe static void VariantOneStepOne(byte* scratchpad)
        {
            byte tmp = scratchpad[11];

            uint table = 0x75310;

            byte index = (byte)((((tmp >> 3) & 6) | (tmp & 1)) << 1);

            scratchpad[11] = (byte)(tmp ^ ((table >> index) & 0x30));
        }

        public unsafe static void VariantOneStepTwo(ulong *a, ulong *b)
        {
            *a ^= *b;
        }

        private static void XOR64(byte[] a, byte[] b, int offsetA = 0,
                                  int offsetB = 0)
        {
            for (int i = 0; i < 8; i++)
            {
                a[i + offsetA] ^= b[i + offsetB];
            }
        }

        public static unsafe void InitMixingState(ulong *a, ulong *b, ulong *k)
        {
            a[0] = k[0] ^ k[4];
            a[1] = k[1] ^ k[5];
            b[0] = k[2] ^ k[6];
            b[1] = k[3] ^ k[7];
        }

        public static int StateIndex(ulong j, int modulus)
        {
            return (int)(j >> 4 & (ulong)modulus) << 4;
        }
    }
}
