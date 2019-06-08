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

namespace Canti.Cryptography.Native.SkeinFish
{
    public class SkeinConfig
    {
        private readonly int stateSize;

        public SkeinConfig(Skein sourceHash)
        {
            stateSize = sourceHash.StateSize;
            ConfigValue = new ulong[sourceHash.StateSize/8];
            // Set the state size for the configuration
            ConfigString = new ulong[ConfigValue.Length];
            ConfigString[1] = (ulong)sourceHash.HashSize;
        }

        public void GenerateConfiguration()
        {
            var cipher = ThreefishCipher.CreateCipher(stateSize);
            var tweak = new UbiTweak();
            tweak.StartNewBlockType(UbiType.Config);
            tweak.IsFinalBlock = true;
            tweak.BitsProcessed = 32;
            cipher.SetTweak(tweak.Tweak);
            cipher.Encrypt(ConfigString, ConfigValue);
            ConfigValue[0] ^= ConfigString[0];
            ConfigValue[1] ^= ConfigString[1];
            ConfigValue[2] ^= ConfigString[2];
        }

        public void GenerateConfiguration(ulong[] initialState)
        {
            var cipher = ThreefishCipher.CreateCipher(stateSize);
            var tweak = new UbiTweak();
            tweak.StartNewBlockType(UbiType.Config);
            tweak.IsFinalBlock = true;
            tweak.BitsProcessed = 32;
            cipher.SetKey(initialState);
            cipher.SetTweak(tweak.Tweak);
            cipher.Encrypt(ConfigString, ConfigValue);
            ConfigValue[0] ^= ConfigString[0];
            ConfigValue[1] ^= ConfigString[1];
            ConfigValue[2] ^= ConfigString[2];
        }

        public void SetSchema(params byte[] schema)
        {
            if (schema.Length!=4)
                throw new Exception("Schema must be 4 bytes.");
            ulong n = ConfigString[0];
            // Clear the schema bytes
            n &= ~0xfffffffful;
            // Set schema bytes
            n |= (ulong)schema[3]<<24;
            n |= (ulong)schema[2]<<16;
            n |= (ulong)schema[1]<<8;
            n |= (ulong)schema[0];
            ConfigString[0] = n;
        }

        public void SetVersion(int version)
        {
            if (version<0 || version>3)
                throw new Exception("Version must be between 0 and 3, inclusive.");
            ConfigString[0] &= ~((ulong)0x03<<32);
            ConfigString[0] |= (ulong)version<<32;
        }

        public void SetTreeLeafSize(byte size)
        {
            ConfigString[2] &= ~(ulong)0xff;
            ConfigString[2] |= size;
        }

        public void SetTreeFanOutSize(byte size)
        {
            ConfigString[2] &= ~((ulong)0xff<<8);
            ConfigString[2] |= (ulong)size<<8;
        }

        public void SetMaxTreeHeight(byte height)
        {
            if (height==1)
                throw new Exception("Tree height must be zero or greater than 1.");
            ConfigString[2] &= ~((ulong)0xff<<16);
            ConfigString[2] |= (ulong)height<<16;
        }

        public ulong[] ConfigValue { get; private set; }

        public ulong[] ConfigString { get; private set; }
    }
}
