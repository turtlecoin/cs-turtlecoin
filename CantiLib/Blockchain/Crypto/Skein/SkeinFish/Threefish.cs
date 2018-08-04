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
    public class Threefish : SymmetricAlgorithm
    {
        const int DefaultCipherSize = 256;

        public Threefish()
        {
            // Set up supported key and block sizes for Threefish
            KeySizes[] supportedSizes = 
            {
                new KeySizes(256, 512, 256),
                new KeySizes(1024, 1024, 0)
            };

            base.LegalBlockSizesValue = supportedSizes;
            base.LegalKeySizesValue   = supportedSizes;

            // Set up default sizes
            base.KeySizeValue   = DefaultCipherSize;
            base.BlockSizeValue = DefaultCipherSize;

            // ECB is the default for the other ciphers in
            // the standard library I think
            base.ModeValue = CipherMode.ECB;
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new ThreefishTransform(rgbKey, rgbIV, ThreefishTransformType.Decrypt, ModeValue, PaddingValue);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new ThreefishTransform(rgbKey, rgbIV, ThreefishTransformType.Encrypt, ModeValue, PaddingValue);
        }

        public override void GenerateIV()
        {
            base.IVValue = GenerateRandomBytes(base.BlockSizeValue / 8);
        }

        public override void GenerateKey()
        {
            base.KeyValue = GenerateRandomBytes(base.KeySizeValue / 8);
        }

        static byte[] GenerateRandomBytes(int amount)
        {
            var rngCrypto = new RNGCryptoServiceProvider();

            var bytes = new byte[amount];
            rngCrypto.GetBytes(bytes);

            return bytes;
        }
    }
}
