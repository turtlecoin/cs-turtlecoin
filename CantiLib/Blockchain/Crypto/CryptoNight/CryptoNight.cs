// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using Canti.Blockchain.Crypto.AES;
using static Canti.Blockchain.Crypto.Keccak.Keccak;

namespace Canti.Blockchain.Crypto.CryptoNight
{
    public static class CryptoNight
    {
        /* Takes a byte[] input, and return a byte[] output, of length 256 */
        public static byte[] CryptoNightVersionZero(byte[] data)
        {
            /* CryptoNight Step 1:  Use Keccak1600 to initialize the 'state'
             *                      (and 'text') buffers from the data.
             */


            /* Hash the inputted data with keccak into a 200 byte hash */
            byte[] state = keccak1600(data);

            byte[] text = new byte[Constants.InitSizeByte];

            byte[] aesKey = new byte[Constants.AesKeySize];

            byte[] expandedKeys = AES.AES.ExpandKey(aesKey);

            byte[] scratchpad = new byte[Constants.Memory];

            /* Copy InitSizeByte bytes from the state array, at an offset of
               64, to the text array */
            Buffer.BlockCopy(state, 64, text, 0, text.Length);

            /* Copy AesKeySize bytes from the state array to the aesKey array */
            Buffer.BlockCopy(state, 0, aesKey, 0, aesKey.Length);


            /* CryptoNight Step 2:  Iteratively encrypt the results from Keccak
             *                      to fill the 2MB large random access buffer.
             */

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

            /* Just for testing purposes for now */
            return scratchpad;
        }
    }
}
