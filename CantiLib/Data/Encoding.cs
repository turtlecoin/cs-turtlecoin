//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Canti.Data
{
    public static class Encoding
    {
        #region Utilities
        // Gets the byte size of an object
        public static int GetSizeOfObject(object Input)
        {
            try { return System.Runtime.InteropServices.Marshal.SizeOf(Input); }
            catch { return -1;  }
        }

        // Gets the smallest integer type an integer will fit in
        public static object GetSmallestIntegerType(ulong Value)
        {
            if (Value < 64) return (byte)Value;
            else if (Value < 16384) return (ushort)Value;
            else if (Value < 1073741824) return (uint)Value;
            else return Value;
        }
        #endregion

        #region Objects
        // Encodes an object to a hex string
        internal static string ObjectToHexString(object Input)
        {
            byte[] Bytes = ObjectToByteArray(Input);
            string Hex = ByteArrayToHexString(Bytes);
            return Hex;
        }

        // Decodes an object from a hex string
        public static T HexStringToObject<T>(string Input)
        {
            byte[] Bytes = HexStringToByteArray(Input);
            return DecodeObject<T>(Bytes);
        }

        // Encodes an object to a byte array
        public static byte[] ObjectToByteArray(object Input)
        {
            BinaryFormatter Binary = new BinaryFormatter();
            using (MemoryStream Stream = new MemoryStream())
            {
                Binary.Serialize(Stream, Input);
                return Stream.ToArray();
            }
        }

        // Decodes an object from a byte array
        public static T DecodeObject<T>(byte[] Input)
        {
            using (var Stream = new MemoryStream())
            {
                BinaryFormatter Binary = new BinaryFormatter();
                Stream.Write(Input, 0, Input.Length);
                Stream.Seek(0, SeekOrigin.Begin);
                return (T)Binary.Deserialize(Stream);
            }
        }
        #endregion

        #region Strings
        // Encodes a string to a byte array
        public static byte[] StringToByteArray(string Input)
        {
            byte[] Output = new byte[0];
            foreach (char c in Input.ToCharArray()) Output = AppendToByteArray((byte)c, Output);
            return Output;
        }

        // Decodes a string from a byte array
        public static string ByteArrayToString(byte[] Input)
        {
            string Output = "";
            foreach (byte b in Input) Output += (char)b;
            return Output;
        }
        public static string ByteArrayToString(byte[] Input, int Offset, int Length)
        {
            byte[] Buffer = SplitByteArray(Input, Offset, Length);
            return ByteArrayToString(Buffer);
        }

        // Encodes a string to a hex string
        public static string StringToHexString(string Input)
        {
            return ByteArrayToHexString(StringToByteArray(Input));
        }

        // Decodes a string from a hex string
        public static string HexStringToString(string Input)
        {
            return ByteArrayToString(HexStringToByteArray(Input));
        }
        #endregion

        #region Integers
        // Encodes an integer type to a hex string
        public static string IntegerToHexString<T>(T Input) where T : IConvertible
        {
            byte[] Bytes = IntegerToByteArray(Input);
            return ByteArrayToHexString(Bytes);
        }

        // Decodes a hex string to an integer type
        public static T HexStringToInteger<T>(string Input) where T : IConvertible
        {
            byte[] Bytes = HexStringToByteArray(Input);
            return ByteArrayToInteger<T>(Bytes);
        }

        // Encodes an integer type to a byte array
        public static byte[] IntegerToByteArray<T>(T Input) where T : IConvertible
        {
            byte[] Output = new byte[GetSizeOfObject(default(T))];
            for (int i = 0; i < Output.Length; i++)
                Output[i] = (byte)(((Convert.ToUInt64(Input) >> i * 8) & 0xFF));
            return Output;
        }

        // Decodes an integer type from a byte array
        public static T ByteArrayToInteger<T>(byte[] Input) where T : IConvertible
        {
            ulong Output = ((ulong)Input[Input.Length - 1] << (Input.Length - 1) * 8);
            for (int i = Input.Length - 2; i >= 0; i--)
                Output |= ((ulong)Input[i] << i * 8);
            return (T)Convert.ChangeType(Output, typeof(T));
        }
        public static T ByteArrayToInteger<T>(byte[] Input, int Offset) where T : IConvertible
        {
            byte[] Bytes = SplitByteArray(Input, Offset, GetSizeOfObject(default(T)));
            ulong Output = ((ulong)Bytes[Bytes.Length - 1] << (Bytes.Length - 1) * 8);
            for (int i = Bytes.Length - 2; i >= 0; i--)
                Output |= ((ulong)Bytes[i] << i * 8);
            return (T)Convert.ChangeType(Output, typeof(T));
        }
        public static T ByteArrayToInteger<T>(byte[] Input, int Offset, int Length) where T : IConvertible
        {
            byte[] Bytes = SplitByteArray(Input, Offset, Length);
            ulong Output = ((ulong)Bytes[Bytes.Length - 1] << (Bytes.Length - 1) * 8);
            for (int i = Bytes.Length - 2; i >= 0; i--)
                Output |= ((ulong)Bytes[i] << i * 8);
            return (T)Convert.ChangeType(Output, typeof(T));
        }
        #endregion

        #region Conversion
        // Converts a byte array to a hex string
        public static string ByteArrayToHexString(byte[] Input)
        {
            StringBuilder Hex = new StringBuilder(Input.Length * 2);
            foreach (byte Byte in Input) Hex.AppendFormat("{0:x2}", Byte);
            return Hex.ToString();
        }

        // Converts a hex string to a byte array
        public static byte[] HexStringToByteArray(String Input)
        {
            byte[] Output = new byte[Input.Length / 2];
            for (int i = 0; i < Input.Length; i += 2)
                Output[i / 2] = Convert.ToByte(Input.Substring(i, 2), 16);
            return Output;
        }
        #endregion

        #region Compression
        // Compresses a byte array using gzip compression
        public static byte[] CompressByteArray(byte[] Input)
        {
            using (MemoryStream In = new MemoryStream(Input))
            using (MemoryStream Out = new MemoryStream())
            {
                using (GZipStream Comp = new GZipStream(Out, CompressionMode.Compress))
                    In.CopyTo(Comp);
                return Out.ToArray();
            }
        }

        // Decomresses a byte array using gzip compressiion
        public static byte[] DecompressByteArray(byte[] Input)
        {
            using (MemoryStream In = new MemoryStream(Input))
            using (MemoryStream Out = new MemoryStream())
            {
                using (GZipStream Comp = new GZipStream(In, CompressionMode.Decompress))
                    Comp.CopyTo(Out);
                return Out.ToArray();
            }
        }
        #endregion

        #region Byte Arrays
        // Splits a byte array into a smaller array
        public static byte[] SplitByteArray(byte[] Source, int Offset, int Length)
        {
            byte[] Output = new byte[Length];
            Buffer.BlockCopy(Source, Offset, Output, 0, Length);
            return Output;
        }

        // Appends one byte array to the end of another one
        public static byte[] AppendToByteArray(byte[] Source, byte[] Destination)
        {
            byte[] NewBytes = new byte[Source.LongLength + Destination.LongLength];
            Buffer.BlockCopy(Destination, 0, NewBytes, 0, Destination.Length);
            Buffer.BlockCopy(Source, 0, NewBytes, Destination.Length, Source.Length);
            return NewBytes;
        }

        public static byte[] AppendToByteArray(byte Source, byte[] Destination)
        {
            byte[] NewBytes = new byte[1 + Destination.LongLength];
            Buffer.BlockCopy(Destination, 0, NewBytes, 0, Destination.Length);
            Buffer.BlockCopy(new byte[] { Source }, 0, NewBytes, Destination.Length, 1);
            return NewBytes;
        }
        #endregion

        #region Miscellaneous
        // Converts a guid to a hex string
        public static string GuidToHexString(Guid Input)
        {
            byte[] bytes = Input.ToByteArray();
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            Console.WriteLine(StringToHexString(new string(chars)));
            return new string(chars);
        }

        // Converts a hex string to a guid
        public static Guid HexStringToGuid(string Input)
        {
            return new Guid(HexStringToByteArray(Input));
        }
        #endregion

        // This doesn't belong here lol
        public static int BlockSizeLimit()
        {
            // Currently hard set to 115kb
            return 115000;
        }
    }
}
