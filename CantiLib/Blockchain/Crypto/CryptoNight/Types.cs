//
// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using Canti.Blockchain.Crypto.AES;

namespace Canti.Blockchain.Crypto.CryptoNight
{
    public class MixScratchpadState
    {
        public static MixScratchpadState GetScratchpadState(CNState cnState)
        {
            if (cachedState == null)
            {
                cachedState = new MixScratchpadState(cnState);
            }

            return cachedState;
        }

        public unsafe MixScratchpadState(CNState cnState)
        {
            k = cnState.GetK();

            fixed (byte* aPtr = a, bPtr = b, kPtr = k)
            {
                ulong *_a = (ulong *)aPtr;
                ulong *_b = (ulong *)bPtr;
                ulong *_k = (ulong *)kPtr;

                _a[0] = (_k + 0)[0] ^ (_k + 4)[0];
                _a[1] = (_k + 0)[1] ^ (_k + 4)[1];
                _b[0] = (_k + 2)[0] ^ (_k + 6)[0];
                _b[1] = (_k + 2)[1] ^ (_k + 6)[1];
            }
        }

        public byte[] a = new byte[AES.Constants.BlockSize];
        public byte[] b = new byte[AES.Constants.BlockSize * 2];
        public byte[] c = new byte[AES.Constants.BlockSize];
        public byte[] d = new byte[AES.Constants.BlockSize];

        public byte[] k;

        private static MixScratchpadState cachedState = null;
    }

    /* This class encapsulates the different ways we index the 200 byte
       state buffer. There are different bits we use each section for,
       of different lengths and offsets. */
    public class CNState
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

        /* Tweak is the last 8 bytes of the state, e.g. state[191:199] */
        public byte[] GetTweak()
        {
            byte[] tweak = new byte[8];

            /* Copy 8 bytes from the state array to the tweak array, at an
               offset of 192 in the state array */
            Buffer.BlockCopy(state, 192, tweak, 0, tweak.Length);

            return tweak;
        }

        /* The underlying 200 byte array */
        private byte[] state;
    }
}
