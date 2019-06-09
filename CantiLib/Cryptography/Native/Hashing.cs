//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using Canti.Cryptography.Native;
using Canti.Cryptography.Native.CryptoNight;
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
            byte[] Output = new Keccak().Hash(Data);

            return ByteArrayToHexString(Output);
        }

        public string CN_SlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_SlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_SlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_LiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNLiteV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_LiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNLiteV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_LiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNLiteV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkLiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkLiteV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkLiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkLiteV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_DarkLiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNDarkLiteV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleLiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleLiteV0().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleLiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleLiteV1().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
        }

        public string CN_TurtleLiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            byte[] Output = new CNTurtleLiteV2().Hash(HexStringToByteArray(Data));

            return ByteArrayToHexString(Output);
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
