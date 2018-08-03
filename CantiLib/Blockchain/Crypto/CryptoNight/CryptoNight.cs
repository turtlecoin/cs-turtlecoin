// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using Canti.Data;
using Canti.Blockchain.Crypto.AES;
using static Canti.Blockchain.Crypto.Keccak.Keccak;

namespace Canti.Blockchain.Crypto.CryptoNight
{
    public static class CryptoNight
    {
        /* Takes a byte[] input, and return a byte[] output, of length 256 */
        /* TODO: Take memory, iterations, etc as input struct, or possibly
           use an interface */
        public static byte[] CryptoNightVersionZero(byte[] data)
        {
            /* CryptoNight Step 1:  Use Keccak1600 to initialize the 'state'
             *                      'text', 'k', and 'aesKey' buffers
             */


            /* Hash the inputted data with keccak into a 200 byte hash */
            byte[] state = keccak1600(data);

            /* State returned is 200 bytes, k takes the first 64 bytes, and
               text takes the latter 128 bytes */
            byte[] k = new byte[64];
            byte[] text = new byte[Constants.InitSizeByte];

            /* Copy 64 bytes from the state array to the k array */
            Buffer.BlockCopy(state, 0, k, 0, k.Length);

            /* Copy InitSizeByte bytes from the state array, at an offset of
               k.Length, to the text array */
            Buffer.BlockCopy(state, k.Length, text, 0, text.Length);

            byte[] aesKey = new byte[AES.Constants.KeySize];

            /* Copy AESKeySize bytes from the state array to the aesKey array */
            Buffer.BlockCopy(state, 0, aesKey, 0, aesKey.Length);


            /* CryptoNight Step 2:  Iteratively encrypt the results from Keccak
             *                      to fill the 2MB large random access buffer.
             */

            /* Expand our initial key into many for each round of pseudo aes */
            byte[] expandedKeys = AES.AES.ExpandKey(aesKey);

            /* Our large scratchpad, 2MB in default CN */
            byte[] scratchpad = new byte[Constants.Memory];

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

            for (int i = 0; i < AES.Constants.BlockSize; i++)
            {
                a[i] = (byte)(k[i] ^ k[i+32]);
                b[i] = (byte)(k[i+16] ^ k[i+48]);
            }

            for (int i = 0; i < Constants.Iterations / 2; i++)
            {
                /* ITERATION ONE */

                int j = e2i(a);

                /* Load c from the scratchpad */
                CopyBlockFromScratchpad(scratchpad, c, j);

                AES.AES.EncryptionRound(a, c);

                XORBlocks(b, c);

                SwapBlocks(b, c);

                /* Write c back to the scratchpad */
                CopyBlockToScratchpad(scratchpad, c, j);

                SwapBlocks(a, b);


                /* ITERATION TWO */ 
            }

            /* Just for testing purposes for now */
            return scratchpad;
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


        /* XOR a and b */
        private static void XORBlocks(byte[] a, byte[] b)
        {
            for (int i = 0; i < AES.Constants.BlockSize; i++)
            {
                a[i] ^= b[i];
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
    }
}
