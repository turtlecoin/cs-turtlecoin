//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using Canti.Cryptography.Native;
using System;
using static Canti.Utils;

namespace Canti.Cryptography
{
    public sealed partial class NativeCrypto : ICryptography
    {
        public string CN_FastHash(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            return CN_FastHash(HexStringToByteArray(Data));
        }

        public string CN_FastHash(byte[] Data)
        {
            byte[] Output = Keccak.Hash(Data);

            return ByteArrayToHexString(Output);
        }

        public string CN_SlowHashV0(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_SlowHashV1(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_SlowHashV2(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_LiteSlowHashV0(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_LiteSlowHashV1(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_LiteSlowHashV2(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_DarkSlowHashV0(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_DarkSlowHashV1(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_DarkSlowHashV2(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_DarkLiteSlowHashV0(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_DarkLiteSlowHashV1(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_DarkLiteSlowHashV2(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_TurtleSlowHashV0(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_TurtleSlowHashV1(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_TurtleSlowHashV2(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_TurtleLiteSlowHashV0(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_TurtleLiteSlowHashV1(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_TurtleLiteSlowHashV2(string Data)
        {
            throw new NotImplementedException();
        }

        public string CN_SoftShellSlowHashV0(string Data, uint height)
        {
            throw new NotImplementedException();
        }

        public string CN_SoftShellSlowHashV1(string Data, uint height)
        {
            throw new NotImplementedException();
        }

        public string CN_SoftShellSlowHashV2(string Data, uint height)
        {
            throw new NotImplementedException();
        }

        public string ChukwaSlowHash(string Data)
        {
            throw new NotImplementedException();
        }
    }
}
