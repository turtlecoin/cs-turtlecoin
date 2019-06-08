/*
Copyright (c) 2010 Alberto Fajardo

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
 
Bug fixes:
Copyright (c) 2015 Pavel Kovalenko
Same licence, etc. applies.
*/

using System.Security.Cryptography;

namespace Canti.Cryptography.Native.SkeinFish
{
    /// <summary>
    /// Specifies the Skein initialization type.
    /// </summary>
    public enum SkeinInitializationType
    {
        /// <summary>
        /// Identical to the standard Skein initialization.
        /// </summary>
        Normal,
        /// <summary>
        /// Creates the initial state with zeros instead of the configuration block, then initializes the hash.
        /// This does not start a new UBI block type, and must be done manually.
        /// </summary>
        ZeroedState,
        /// <summary>
        /// Leaves the initial state set to its previous value, which is then chained with subsequent block transforms.
        /// This does not start a new UBI block type, and must be done manually.
        /// </summary>
        ChainedState,
        /// <summary>
        /// Creates the initial state by chaining the previous state value with the config block, then initializes the hash.
        /// This starts a new UBI block type with the standard Payload type.
        /// </summary>
        ChainedConfig,
    }

    public class Skein : HashAlgorithm
    {
        private readonly ThreefishCipher cipher;
        private readonly int cipherStateBits;
        private readonly int cipherStateBytes;
        private readonly int cipherStateWords;
        private readonly int outputBytes;
        private readonly byte[] inputBuffer;
        private int bytesFilled;
        private readonly ulong[] cipherInput;
        private readonly ulong[] state;
        public SkeinConfig Configuration { get; private set; }
        public UbiTweak UbiParameters { get; private set; }

        public int StateSize
        {
            get { return cipherStateBits; }
        }

        /// <summary>
        /// Initializes the Skein hash instance.
        /// </summary>
        /// <param name="stateSize">The internal state size of the hash in bits.
        /// Supported values are 256, 512, and 1024.</param>
        /// <param name="outputSize">The output size of the hash in bits.
        /// Output size must be divisible by 8 and greater than zero.</param>
        public Skein(int stateSize, int outputSize)
        {
            if (outputSize<=0)
                throw new CryptographicException("Output bit size must be greater than zero.");
            if (outputSize%8!=0)
                throw new CryptographicException("Output bit size must be divisible by 8.");
            cipherStateBits = stateSize;
            cipherStateBytes = stateSize/8;
            cipherStateWords = stateSize/64;
            HashSizeValue = outputSize;
            outputBytes = (outputSize+7)/8;
            cipher = ThreefishCipher.CreateCipher(stateSize);
            if (cipher==null)
                throw new CryptographicException("Unsupported state size.");
            inputBuffer = new byte[cipherStateBytes];
            cipherInput = new ulong[cipherStateWords];
            state = new ulong[cipherStateWords];
            UbiParameters = new UbiTweak();
            Configuration = new SkeinConfig(this);
            Configuration.SetSchema(83, 72, 65, 51); // "SHA3"
            Configuration.SetVersion(1);
            Configuration.GenerateConfiguration();
            Initialize();
        }

        private void ProcessBlock(int bytes)
        {
            // Set the key to the current state
            cipher.SetKey(state);
            // Update tweak
            UbiParameters.BitsProcessed += (ulong)bytes;
            cipher.SetTweak(UbiParameters.Tweak);
            // Encrypt block
            cipher.Encrypt(cipherInput, state);
            // Feed-forward input with state
            for (int i = 0; i<cipherInput.Length; i++)
                state[i] ^= cipherInput[i];
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int bytesDone = 0;
            int offset = ibStart;
            while (bytesDone<cbSize && offset<array.Length)
            {
                if (bytesFilled==cipherStateBytes)
                {
                    InputBufferToCipherInput();
                    ProcessBlock(cipherStateBytes);
                    UbiParameters.IsFirstBlock = false;
                    bytesFilled = 0;
                }
                inputBuffer[bytesFilled++] = array[offset++];
                bytesDone++;
            }
        }

        protected override byte[] HashFinal()
        {
            // special case for empty MAC key
            if (UbiParameters.BlockType==UbiType.Key && bytesFilled==0)
                return null;
            // Pad left over space in input buffer with zeros and copy to cipher input buffer
            for (int i = bytesFilled; i<inputBuffer.Length; i++)
                inputBuffer[i] = 0;
            InputBufferToCipherInput();
            UbiParameters.IsFinalBlock = true;
            ProcessBlock(bytesFilled);
            for (int i = 0; i<cipherInput.Length; i++)
                cipherInput[i] = 0;
            // Do output block counter mode output
            var hash = new byte[outputBytes];
            var oldState = new ulong[cipherStateWords];
            for (int j = 0; j<state.Length; j++)
                oldState[j] = state[j];
            for (int i = 0; i<outputBytes; i += cipherStateBytes)
            {
                UbiParameters.StartNewBlockType(UbiType.Out);
                UbiParameters.IsFinalBlock = true;
                ProcessBlock(8);
                // Output a chunk of the hash
                int outputSize = outputBytes-i;
                if (outputSize>cipherStateBytes)
                    outputSize = cipherStateBytes;
                PutBytes(state, hash, i, outputSize);
                for (int j = 0; j<state.Length; j++)
                    state[j] = oldState[j];
                cipherInput[0]++;
            }
            return hash;
        }

        /// <summary>
        /// Creates the initial state with zeros instead of the configuration block, then initializes the hash.
        /// This does not start a new UBI block type, and must be done manually.
        /// </summary>
        public void Initialize(SkeinInitializationType initializationType)
        {
            switch (initializationType)
            {
            case SkeinInitializationType.Normal:
                Initialize();
                return;
            case SkeinInitializationType.ZeroedState:
                for (int i = 0; i<state.Length; i++)
                    state[i] = 0;
                break;
            case SkeinInitializationType.ChainedState:
                break;
            case SkeinInitializationType.ChainedConfig:
                Configuration.GenerateConfiguration(state);
                Initialize();
                return;
            }
            bytesFilled = 0;
        }

        public override sealed void Initialize()
        {
            for (int i = 0; i<state.Length; i++)
                state[i] = Configuration.ConfigValue[i];
            UbiParameters.StartNewBlockType(UbiType.Message);
            bytesFilled = 0;
        }

        // Moves the byte input buffer to the ulong cipher input
        private void InputBufferToCipherInput()
        {
            for (int i = 0; i<cipherStateWords; i++)
                cipherInput[i] = GetUInt64(inputBuffer, i*8);
        }

        #region Utils

        private static ulong GetUInt64(byte[] buf, int offset)
        {
            ulong v = buf[offset];
            v |= (ulong)buf[offset+1]<<8;
            v |= (ulong)buf[offset+2]<<16;
            v |= (ulong)buf[offset+3]<<24;
            v |= (ulong)buf[offset+4]<<32;
            v |= (ulong)buf[offset+5]<<40;
            v |= (ulong)buf[offset+6]<<48;
            v |= (ulong)buf[offset+7]<<56;
            return v;
        }

        private static void PutBytes(ulong[] input, byte[] output, int offset, int byteCount)
        {
            int j = 0;
            for (int i = 0; i<byteCount; i++)
            {
                output[offset+i] = (byte)((input[i/8]>>j) & 0xff);
                j = (j+8)%64;
            }
        }

        #endregion
    }
}
