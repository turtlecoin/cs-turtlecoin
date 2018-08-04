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

using System;
using System.Security.Cryptography;

namespace Canti.Blockchain.Crypto.Skein.SkeinFish
{
    public enum ThreefishTransformType
    {
        Encrypt, Decrypt
    }

    public class ThreefishTransform : ICryptoTransform
    {
        delegate int TransformFunc(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset);

        private readonly ThreefishCipher _cipher;
        private readonly TransformFunc _transformFunc;

        private readonly CipherMode  _cipherMode;
        private readonly PaddingMode _paddingMode;

        private readonly int _cipherBytes;
        private readonly int _cipherWords;

        private readonly ulong[] _block;
        private readonly ulong[] _tempBlock;
        private readonly ulong[] _iv;
        
        // Used when in a stream ciphering mode
        private readonly byte[] _streamBytes;
        private int _usedStreamBytes;

        public ThreefishTransform(
            byte[] key, byte[] iv, ThreefishTransformType type, CipherMode mode, PaddingMode padding
        )
        {
            _cipherMode    = mode;
            _paddingMode   = padding;

            _cipherBytes = key.Length;
            _cipherWords = key.Length / 8;
            OutputBlockSize  = key.Length * 8;

            // Allocate working blocks now so that we don't
            // have to allocate them each time 
            // Transform(Final)Block is called
            _block = new ulong[_cipherWords];
            _tempBlock = new ulong[_cipherWords];
            _streamBytes = new byte[_cipherBytes];

            // Allocate IV and set value
            _iv = new ulong[_cipherWords];
            GetBytes(iv, 0, _iv, _cipherBytes);

            // Figure out which cipher we need based on
            // the cipher bit size
            switch (OutputBlockSize)
            {
                case 256:
                    _cipher = new Threefish256();
                    break;
                case 512:
                    _cipher = new Threefish512();
                    break;
                case 1024:
                    _cipher = new Threefish1024();
                    break;

                default:
                    throw new CryptographicException("Unsupported key/block size.");
            }

            bool e = (type == ThreefishTransformType.Encrypt);

            switch(_cipherMode)
            {
                case CipherMode.ECB:
                    _transformFunc = e ? new TransformFunc(EcbEncrypt) : new TransformFunc(EcbDecrypt);
                    break;
                case CipherMode.CBC:
                    _transformFunc = e ? new TransformFunc(CbcEncrypt) : new TransformFunc(CbcDecrypt);
                    break;
                case CipherMode.OFB:
                    _transformFunc = new TransformFunc(OfbApplyStream);
                    break;
                case CipherMode.CFB:
                    _transformFunc = e ? new TransformFunc(CfbEncrypt) : new TransformFunc(CfbDecrypt);
                    break;
                case CipherMode.CTS:
                    throw new CryptographicException("CTS mode not supported.");
            }

            // Set the key
            var keyWords = new ulong[_cipherWords];
            GetBytes(key, 0, keyWords, _cipherBytes);
            _cipher.SetKey(keyWords);

            InitializeBlocks();
        }

        // (Re)initializes the blocks for encryption
        void InitializeBlocks()
        {
            switch (_cipherMode)
            {
                case CipherMode.ECB:
                case CipherMode.CBC:
                    // Clear the working block
                    for (int i = 0; i < _cipherWords; i++)
                        _block[i] = 0;
                    break;

                case CipherMode.OFB:
                    // Copy the IV to the working block
                    for (int i = 0; i < _cipherWords; i++)
                        _block[i] = _iv[i];

                    break;

                case CipherMode.CFB:
                    // Copy IV to cipher stream bytes
                    PutBytes(_iv, _streamBytes, 0, _cipherBytes);
                    break;
            }

            _usedStreamBytes = _cipherBytes;
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

        static void PutUInt64(byte[] buf, int offset, ulong v)
        {
            buf[offset] = (byte)(v & 0xff);
            buf[offset + 1] = (byte)((v >> 8) & 0xff);
            buf[offset + 2] = (byte)((v >> 16) & 0xff);
            buf[offset + 3] = (byte)((v >> 24) & 0xff);
            buf[offset + 4] = (byte)((v >> 32) & 0xff);
            buf[offset + 5] = (byte)((v >> 40) & 0xff);
            buf[offset + 6] = (byte)((v >> 48) & 0xff);
            buf[offset + 7] = (byte)(v >> 56);
        }

        static void GetBytes(byte[] input, int offset, ulong[] output, int byteCount)
        {
            for (int i = 0; i < byteCount; i += 8)
            {
                output[i / 8] = GetUInt64(input, i + offset);
            }
        }

        static void PutBytes(ulong[] input, byte[] output, int offset, int byteCount)
        {
            for (int i = 0; i < byteCount; i += 8)
            {
                PutUInt64(output, i + offset, input[i / 8]);
            }
        }

        #endregion

        #region ModeTransformFunctions

        // ECB mode encryption
        int EcbEncrypt(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            if (inputCount >= _cipherBytes)
            {
                GetBytes(input, inputOffset, _block, _cipherBytes);
                _cipher.Encrypt(_block, _block);
                PutBytes(_block, output, outputOffset, _cipherBytes);

                return _cipherBytes;
            }

            return 0;
        }

        // ECB mode decryption
        int EcbDecrypt(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            if (inputCount >= _cipherBytes)
            {
                GetBytes(input, inputOffset, _block, _cipherBytes);
                _cipher.Decrypt(_block, _block);
                PutBytes(_block, output, outputOffset, _cipherBytes);

                return _cipherBytes;
            }

            return 0;
        }

        // CBC mode encryption
        int CbcEncrypt(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            if (inputCount >= _cipherBytes)
            {
                int i;

                GetBytes(input, inputOffset, _block, _cipherBytes);

                // Apply the IV
                for (i = 0; i < _cipherWords; i++)
                    _block[i] ^= _iv[i];

                _cipher.Encrypt(_block, _block);

                // Copy the output to the IV
                for (i = 0; i < _cipherWords; i++)
                    _iv[i] = _block[i];

                PutBytes(_block, output, outputOffset, _cipherBytes);

                return _cipherBytes;
            }

            return 0;
        }

        // CBC mode encryption
        int CbcDecrypt(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            if (inputCount >= _cipherBytes)
            {
                int i;

                GetBytes(input, inputOffset, _block, _cipherBytes);
                
                // Copy the block to the temp block for later (wink wink)
                for (i = 0; i < _cipherWords; i++)
                    _tempBlock[i] = _block[i];

                _cipher.Decrypt(_block, _block);

                // Apply the IV and copy temp block
                // to IV
                for (i = 0; i < _cipherWords; i++)
                {
                    _block[i] ^= _iv[i];
                    _iv[i] = _tempBlock[i];
                }

                PutBytes(_block, output, outputOffset, _cipherBytes);

                return _cipherBytes;
            }

            return 0;
        }

        // OFB mode encryption/decryption
        int OfbApplyStream(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            int i;

            // Input length doesn't matter in OFB, just encrypt
            // as much as we can
            for (i = 0; i < inputCount; i++)
            {
                // Generate new stream bytes if we've used
                // them all up
                if (_usedStreamBytes >= _cipherBytes)
                {
                    _cipher.Encrypt(_block, _block);
                    PutBytes(_block, _streamBytes, 0, _cipherBytes);
                    _usedStreamBytes = 0;
                }

                // XOR input byte with stream byte, output it
                output[outputOffset + i] = (byte)(input[inputOffset + i] ^
                                             _streamBytes[_usedStreamBytes]);
                _usedStreamBytes++;
            }

            // Return bytes done
            return i;
        }

        // CFB mode encryption
        int CfbEncrypt(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            int i;

            for (i = 0; i < inputCount; i++)
            {
                // Generate new stream bytes if we've used
                // them all up
                if (_usedStreamBytes >= _cipherBytes)
                {
                    // Copy cipher stream bytes to working block
                    // (this is the feedback)
                    GetBytes(_streamBytes, 0, _block, _cipherBytes);
                    // Process
                    _cipher.Encrypt(_block, _block);
                    // Put back
                    PutBytes(_block, _streamBytes, 0, _cipherBytes);
                    // Reset for next time
                    _usedStreamBytes = 0;
                }

                // XOR input byte with stream byte
                var b = (byte)(input[inputOffset + i] ^ _streamBytes[_usedStreamBytes]);
                // Output cipher byte
                output[outputOffset + i] = b;
                // Put cipher byte into stream bytes for the feedback
                _streamBytes[_usedStreamBytes] = b;

                _usedStreamBytes++;
            }

            // Return bytes done
            return i;
        }

        // CFB mode decryption
        int CfbDecrypt(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            int i;

            for (i = 0; i < inputCount; i++)
            {
                // Generate new stream bytes if we've used
                // them all up
                if (_usedStreamBytes >= _cipherBytes)
                {
                    // Copy cipher stream bytes to working block
                    // (this is the feedback)
                    GetBytes(_streamBytes, 0, _block, _cipherBytes);
                    // Process
                    _cipher.Encrypt(_block, _block);
                    // Put back
                    PutBytes(_block, _streamBytes, 0, _cipherBytes);
                    // Reset for next time
                    _usedStreamBytes = 0;
                }

                // Get ciphertext byte
                byte b = input[inputOffset + i];
                // XOR input byte with stream byte, output plaintext
                output[outputOffset + i] = (byte)(b ^ _streamBytes[_usedStreamBytes]);
                // Put ciphertext byte into stream bytes for the feedback
                _streamBytes[_usedStreamBytes] = b;

                _usedStreamBytes++;
            }

            // Return bytes done
            return i;
        }


        #endregion


        #region ICryptoTransform Members

        public bool CanReuseTransform
        {
            get { return true; }
        }

        public bool CanTransformMultipleBlocks
        {
            get { return true; }
        }

        public int InputBlockSize
        {
            get { return OutputBlockSize; }
        }

        public int OutputBlockSize { get; private set; }


        private void PadBlock(byte[] input, int inputOffset, int alreadyFilled)
        {
            // Apply the type of padding we're using
            switch (_paddingMode)
            {
                case PaddingMode.None: break;
                case PaddingMode.Zeros:
                    // Fill with zeros
                    for (int i = alreadyFilled; i < _cipherBytes; i++)
                        input[i + inputOffset] = 0;

                    break;

                case PaddingMode.PKCS7:
                    // Fill each byte value with the number of
                    // bytes padded
                    for (int i = alreadyFilled; i < _cipherBytes; i++)
                        input[i + inputOffset] = (byte) (_cipherBytes - alreadyFilled);

                    break;

                case PaddingMode.ANSIX923:
                    // fill with zeros, set last byte
                    // to number of bytes padded
                    for (int i = alreadyFilled; i < _cipherBytes; i++)
                    {
                        input[i + inputOffset] = 0;
                        // If its the last byte, set to number of bytes padded
                        if (i == _cipherBytes - 1)
                            input[i + inputOffset] = (byte)(_cipherBytes - alreadyFilled);
                    }

                    break;

                case PaddingMode.ISO10126:
                    // Fill remaining bytes with random values
                    if (alreadyFilled < _cipherBytes)
                    {
                        var randBytes = new byte[_cipherBytes - alreadyFilled];
                        new RNGCryptoServiceProvider().GetBytes(randBytes);

                        for (int i = alreadyFilled; i < _cipherBytes; i++)
                            input[i + inputOffset] = randBytes[i - alreadyFilled];
                    }

                    break;
            }
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            // Make sure the input count is evenly
            // divisible by the block size
            if ((inputCount & (_cipherBytes - 1)) != 0)
                throw new CryptographicException("inputCount must be divisible by the block size.");

            int totalDone = 0;
            int done;
            // Apply as much of the transform as we can
            do
            {
                done = _transformFunc(
                    inputBuffer,
                    inputOffset + totalDone,
                    inputCount - totalDone,
                    outputBuffer,
                    outputOffset + totalDone
                    );

                totalDone += done;

            } while (done == _cipherBytes);
           
            return totalDone;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var output = new byte[inputCount];

            int totalDone = 0;
            int done;
            // Apply as much of the transform as we can
            do
            {
                done = _transformFunc(
                    inputBuffer,
                    inputOffset + totalDone,
                    inputCount - totalDone,
                    output,
                    totalDone
                    );

                totalDone += done;

            } while (done == _cipherBytes);

            int remaining = inputCount - totalDone;

            // Do the padding and the final transform if
            // there's any data left
            if (totalDone < inputCount)
            {
                // Resize output buffer to be evenly
                // divisible by the block size
                if (inputCount % _cipherBytes != 0)
                {
                    int outputSize = inputCount + (_cipherBytes - (inputCount % _cipherBytes));
                    Array.Resize(ref output, outputSize);
                }
                                
                // Copy remaining bytes over to the output
                for (int i = 0; i < remaining; i++)
                    output[i + totalDone] = inputBuffer[inputOffset + totalDone + i];

                // Pad the block
                PadBlock(output, totalDone, remaining);

                // Encrypt the block
                _transformFunc(output, totalDone, _cipherBytes, output, totalDone);
            }

            // Reinitialize the cipher
            InitializeBlocks();

            return output;

        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // nothing to dispose
        }

        #endregion
    }
}
