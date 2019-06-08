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

Improvements and tweaks:
Copyright (c) 2015 Pavel Kovalenko
Same licence, etc. applies.
*/

using System;
using System.Security.Cryptography;

namespace Canti.Cryptography.Native.SkeinFish
{
    public class Threefish : SymmetricAlgorithm
    {
        private const int DefaultCipherSize = 256;
        private ulong[] tweak;

        public Threefish()
        {
            // Set up supported key and block sizes for Threefish
            KeySizes[] supportedSizes =
            {
                new KeySizes(256, 512, 256),
                new KeySizes(1024, 1024, 0)
            };
            LegalBlockSizesValue = supportedSizes;
            LegalKeySizesValue = supportedSizes;
            // Set up default sizes
            KeySizeValue = DefaultCipherSize;
            BlockSizeValue = DefaultCipherSize;
            FeedbackSizeValue = DefaultCipherSize/2;
            // CBC is the default for the other symmetric ciphers in the standard library.
            ModeValue = CipherMode.CBC;
        }

        public void SetTweak(ulong[] newTweak)
        {
            if (newTweak.Length!=2)
                throw new ArgumentException("Tweak must be an array of two unsigned 64-bit integers.");
            tweak = newTweak;
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            var tsm = new ThreefishTransform(rgbKey, rgbIV, FeedbackSize,
                ThreefishTransformMode.Decrypt, ModeValue, PaddingValue);
            if (tweak!=null)
                tsm.InternalSetTweak(tweak);
            return tsm;
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            var tsm = new ThreefishTransform(rgbKey, rgbIV, FeedbackSize,
                ThreefishTransformMode.Encrypt, ModeValue, PaddingValue);
            if (tweak!=null)
                tsm.InternalSetTweak(tweak);
            return tsm;
        }

        public override void GenerateIV()
        { IVValue = GenerateRandomBytes(BlockSizeValue/8); }

        public override void GenerateKey()
        { KeyValue = GenerateRandomBytes(KeySizeValue/8); }

        private static byte[] GenerateRandomBytes(int amount)
        {
            var rngCrypto = new RNGCryptoServiceProvider();
            var bytes = new byte[amount];
            rngCrypto.GetBytes(bytes);
            return bytes;
        }
    }
}
