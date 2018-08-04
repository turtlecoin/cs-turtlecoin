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
*/

using System.Security.Cryptography;

namespace Canti.Blockchain.Crypto.Skein.SkeinFish
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
        ChainedConfig
    }

    public class Skein : HashAlgorithm
    {
        private readonly ThreefishCipher _cipher;
        
        private readonly int _cipherStateBits;
        private readonly int _cipherStateBytes;
        private readonly int _cipherStateWords;

        private readonly int _outputBytes;

        private readonly byte[] _inputBuffer;
        private int _bytesFilled;

        private readonly ulong[] _cipherInput;
        private readonly ulong[] _state;

        public SkeinConfig Configuration { get; private set; }
        public UbiTweak UbiParameters { get; private set; }

        public int StateSize
        {
            get { return _cipherStateBits; }
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
            // Make sure the output bit size > 0
            if (outputSize <= 0)
                throw new CryptographicException("Output bit size must be greater than zero.");

            // Make sure output size is divisible by 8
            if (outputSize % 8 != 0)
                throw new CryptographicException("Output bit size must be divisible by 8.");

            _cipherStateBits = stateSize;
            _cipherStateBytes = stateSize / 8;
            _cipherStateWords = stateSize / 64;

            base.HashSizeValue = outputSize;
            _outputBytes = (outputSize + 7) / 8;

            // Figure out which cipher we need based on
            // the state size
            _cipher = ThreefishCipher.CreateCipher(stateSize);
            if (_cipher == null) throw new CryptographicException("Unsupported state size.");
            
            // Allocate buffers
            _inputBuffer = new byte[_cipherStateBytes];
            _cipherInput = new ulong[_cipherStateWords];
            _state = new ulong[_cipherStateWords];

            // Allocate tweak
            UbiParameters = new UbiTweak();

            // Generate the configuration string
            Configuration = new SkeinConfig(this);
            Configuration.SetSchema(83, 72, 65, 51); // "SHA3"
            Configuration.SetVersion(1);
            Configuration.GenerateConfiguration();
        }

       
        void ProcessBlock(int bytes)
        {
            // Set the key to the current state
            _cipher.SetKey(_state);

            // Update tweak
            UbiParameters.BitsProcessed += (ulong) bytes;
            _cipher.SetTweak(UbiParameters.Tweak);

            // Encrypt block
            _cipher.Encrypt(_cipherInput, _state);

            // Feed-forward input with state
            for (int i = 0; i < _cipherInput.Length; i++)
                _state[i] ^= _cipherInput[i];
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int bytesDone = 0;
            int offset = ibStart;

            // Fill input buffer
            while (bytesDone < cbSize && offset < array.Length)
            {
                // Do a transform if the input buffer is filled
                if (_bytesFilled == _cipherStateBytes)
                {
                    // Copy input buffer to cipher input buffer
                    InputBufferToCipherInput();
                    
                    // Process the block
                    ProcessBlock(_cipherStateBytes);

                    // Clear first flag, which will be set
                    // by Initialize() if this is the first transform
                    UbiParameters.IsFirstBlock = false;

                    // Reset buffer fill count
                    _bytesFilled = 0;
                }

                _inputBuffer[_bytesFilled++] = array[offset++];
                bytesDone++;
            }
        }

        protected override byte[] HashFinal()
        {
            int i;

            // Pad left over space in input buffer with zeros
            // and copy to cipher input buffer
            for (i = _bytesFilled; i < _inputBuffer.Length; i++)
                _inputBuffer[i] = 0;

            InputBufferToCipherInput();
            
            // Do final message block
            UbiParameters.IsFinalBlock = true;
            ProcessBlock(_bytesFilled);

            // Clear cipher input
            for (i = 0; i < _cipherInput.Length; i++)
                _cipherInput[i] = 0;

            // Do output block counter mode output
            int j;

            var hash = new byte[_outputBytes];
            var oldState = new ulong[_cipherStateWords];

            // Save old state
            for (j = 0; j < _state.Length; j++)
                oldState[j] = _state[j];

            for (i = 0; i < _outputBytes; i += _cipherStateBytes)
            {
                UbiParameters.StartNewBlockType(UbiType.Out);
                UbiParameters.IsFinalBlock = true;
                ProcessBlock(8);

                // Output a chunk of the hash
                int outputSize = _outputBytes - i;
                if (outputSize > _cipherStateBytes)
                    outputSize = _cipherStateBytes;

                PutBytes(_state, hash, i, outputSize);

                // Restore old state
                for (j = 0; j < _state.Length; j++)
                    _state[j] = oldState[j];

                // Increment counter
                _cipherInput[0]++;
            }
                                    
            return hash;
        }

        /// <summary>
        /// Creates the initial state with zeros instead of the configuration block, then initializes the hash.
        /// This does not start a new UBI block type, and must be done manually.
        /// </summary>
        public void Initialize(SkeinInitializationType initializationType)
        {
            switch(initializationType)
            {
                case SkeinInitializationType.Normal:
                    // Normal initialization
                    Initialize();
                    return;

                case SkeinInitializationType.ZeroedState:
                    // Copy the configuration value to the state
                    for (int i = 0; i < _state.Length; i++)
                        _state[i] = 0;
                    break;

                case SkeinInitializationType.ChainedState:
                    // Keep the state as it is and do nothing
                    break;

                case SkeinInitializationType.ChainedConfig:
                    // Generate a chained configuration
                    Configuration.GenerateConfiguration(_state);
                    // Continue initialization
                    Initialize();
                    return;
            }

            // Reset bytes filled
            _bytesFilled = 0;
        }

        public sealed override void Initialize()
        {
            // Copy the configuration value to the state
            for (int i = 0; i < _state.Length; i++)
                _state[i] = Configuration.ConfigValue[i];

            // Set up tweak for message block
            UbiParameters.StartNewBlockType(UbiType.Message);

            // Reset bytes filled
            _bytesFilled = 0;
        }

        // Moves the byte input buffer to the ulong cipher input
        void InputBufferToCipherInput()
        {
            for (int i = 0; i < _cipherStateWords; i++)
                _cipherInput[i] = GetUInt64(_inputBuffer, i * 8);
        }

        #region Utils
        static ulong GetUInt64(byte[] buf, int offset)
        {
            ulong v = buf[offset];
            v |= (ulong)buf[offset + 1] << 8;
            v |= (ulong)buf[offset + 2] << 16;
            v |= (ulong)buf[offset + 3] << 24;
            v |= (ulong)buf[offset + 4] << 32;
            v |= (ulong)buf[offset + 5] << 40;
            v |= (ulong)buf[offset + 6] << 48;
            v |= (ulong)buf[offset + 7] << 56;
            return v;
        }

        static void PutBytes(ulong[] input, byte[] output, int offset, int byteCount)
        {
            int j = 0;
            for (int i = 0; i < byteCount; i++)
            {
                //PutUInt64(output, i + offset, input[i / 8]);
                output[offset + i] = (byte) ((input[i / 8] >> j) & 0xff);
                j = (j + 8) % 64;
            }
        }

        #endregion
    }
}
