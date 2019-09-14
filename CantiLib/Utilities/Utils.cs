//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Canti
{
    /// <summary>
    /// General purpose utility functions
    /// </summary>
    public static class Utils
    {
        #region Encoding

        #region Integers

        /// <summary>
        /// Encodes an integer value type into a byte array
        /// </summary>
        /// <param name="Input">The value to encode</param>
        /// <returns>A byte array representation of the integer input</returns>
        public static byte[] IntegerToByteArray(dynamic Input)
        {
            byte[] Output = new byte[0];
            Type ValueType = Input.GetType();
            if (ValueType == typeof(byte) || ValueType == typeof(sbyte)) Output = new byte[1];
            else if (ValueType == typeof(ushort) || ValueType == typeof(short)) Output = new byte[2];
            else if (ValueType == typeof(uint) || ValueType == typeof(int)) Output = new byte[4];
            else if (ValueType == typeof(ulong) || ValueType == typeof(long)) Output = new byte[8];

            for (int i = 0; i < Output.Length; i++)
                Output[i] = (byte)((Convert.ToUInt64(Input) >> i * 8) & 0xFF);
            return Output;
        }

        /// <summary>
        /// Decodes an integer value from a byte array
        /// </summary>
        /// <typeparam name="T">The type of integer decode</typeparam>
        /// <param name="Input">The byte array to be decoded</param>
        /// <returns>A decoded integer value</returns>
        public static T ByteArrayToInteger<T>(byte[] Input) where T : IComparable<T>
        {
            int Offset = Marshal.SizeOf(default(T)) - 1;
            ulong Output = ((ulong)Input[Offset] << Offset * 8);
            for (; Offset >= 0; Offset--)
                Output |= ((ulong)Input[Offset] << Offset * 8);
            return (T)Convert.ChangeType(Output, typeof(T));
        }

        /// <summary>
        /// Decodes an integer value from a byte array
        /// </summary>
        /// <typeparam name="T">The type of integer to decode</typeparam>
        /// <param name="Input">The byte array to be decoded</param>
        /// <param name="Offset">The position within the byte array to start decoding at</param>
        /// <returns>A decoded integer value</returns>
        public static T ByteArrayToInteger<T>(byte[] Input, int Offset) where T : IComparable<T>
        {
            byte[] Bytes = new byte[Marshal.SizeOf(default(T))];
            Buffer.BlockCopy(Input, Offset, Bytes, 0, Bytes.Length);
            return ByteArrayToInteger<T>(Bytes);
        }

        #endregion

        #region VarInts

        /// <summary>
        /// Packs an integer value into a varint byte array
        /// </summary>
        /// <param name="Input">The integer value to be packed</param>
        /// <returns>An encoded varint byte array</returns>
        public static byte[] PackP2pVarInt(dynamic Input)
        {
            if (Input <= byte.MaxValue) return IntegerToByteArray((byte)(Input << 2 | 0));
            else if (Input <= ushort.MaxValue) return IntegerToByteArray((ushort)(Input << 2 | 1));
            else if (Input <= uint.MaxValue) return IntegerToByteArray((uint)(Input << 2 | 3));
            else if (Input <= ulong.MaxValue) return IntegerToByteArray((ulong)(Input << 2 | 7));
            else return new byte[0];
        }

        /// <summary>
        /// Unpacks an integer value from a varint byte array
        /// </summary>
        /// <typeparam name="T">The type of integer to unpack</typeparam>
        /// <param name="Data">A byte array buffer to unpack from</param>
        /// <param name="Offset">The position to begin unpacking at within the buffer</param>
        /// <param name="NewOffset">The position offset of where unpacking ended</param>
        /// <returns>An unpacked/decoded integer value</returns>
        public static T UnpackP2pVarInt<T>(byte[] Data, int Offset, out int NewOffset) where T : IComparable<T>
        {
            // Get integer size from mask
            int SizeMask = Data[Offset] & 0x03;
            int BytesLeft = 0;

            // Set number of bit shift iterations
            if (SizeMask == 0) BytesLeft = 0;
            else if (SizeMask == 1) BytesLeft = 1;
            else if (SizeMask == 2) BytesLeft = 3;
            else if (SizeMask == 3) BytesLeft = 7;

            // Calculate output number
            ulong Output = Data[Offset];
            for (NewOffset = Offset; NewOffset <= Offset + BytesLeft; NewOffset++)
                Output |= (ulong)Data[NewOffset] << ((NewOffset - Offset) * 8);
            Output >>= 2;

            // Return result
            return (T)Convert.ChangeType(Output, typeof(T));
        }

        public static T UnpackVarInt<T>(byte[] Data, int Offset, out int NewOffset) where T : IComparable<T>
        {
            NewOffset = Offset;
            ulong Output = 0;
            int i;
            for (i = 0; i < Data.Length - Offset; i++)
            {
                byte Temp = Data[i + Offset];
                if (!(i > 0 && Temp == 0)) NewOffset++;
                Output |= (ulong)(Temp & 0x7f) << (i * 7);
                if ((Temp & 0x80) == 0) break;
            }
            return (T)Convert.ChangeType(Output, typeof(T));
        }

        #endregion

        #region Objects

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
        public static T ByteArrayToObject<T>(byte[] Input)
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

        #endregion

        #region Conversion

        /// <summary>
        /// Converts a hex string into a byte array
        /// </summary>
        /// <param name="Input">The hex string to be converted</param>
        /// <returns>A byte array equivalent of the input hex string</returns>
        public static byte[] HexStringToByteArray(string Input)
        {
            byte[] Output = new byte[Input.Length / 2];
            for (int i = 0; i < Input.Length; i += 2)
                Output[i / 2] = Convert.ToByte(Input.Substring(i, 2), 16);
            return Output;
        }

        /// <summary>
        /// Converts a byte array into a hex string
        /// </summary>
        /// <param name="Input">The byte array to be converted</param>
        /// <returns>A hex string equivalent of the input byte array</returns>
        public static string ByteArrayToHexString(byte[] Input)
        {
            StringBuilder Hex = new StringBuilder(Input.Length * 2);
            foreach (byte Byte in Input) Hex.AppendFormat("{0:x2}", Byte);
            return Hex.ToString();
        }

        /// <summary>
        /// Converts a string into a byte array
        /// </summary>
        /// <param name="Input">The string to be converted</param>
        /// <returns>A byte array equivalent of the input string</returns>
        public static byte[] StringToByteArray(string Input)
        {
            return Encoding.UTF8.GetBytes(Input);
        }

        /// <summary>
        /// Converts a byte array into a string
        /// </summary>
        /// <param name="Input">The byte array to be converted</param>
        /// <returns>A string equivalent of the input byte array</returns>
        public static string ByteArrayToString(byte[] Input)
        {
            string Output = "";
            foreach (byte b in Input) Output += (char)b;
            return Output;
        }

        #endregion

        #region String Extensions

        /// <summary>
        /// Gets a byte array representing this string
        /// </summary>
        /// <param name="Input">The string to convert</param>
        /// <param name="EncodeMethod">The method of encoding to use</param>
        /// <returns>A byte array representing this string</returns>
        public static byte[] GetBytes(this string Input, Encoding EncodeMethod = null)
        {
            if (EncodeMethod == null) EncodeMethod = Encoding.Default;
            return EncodeMethod.GetBytes(Input);
        }

        #endregion

        #region Byte Array Extensions

        /// <summary>
        /// Appends a byte array to the end of another byte array
        /// </summary>
        /// <param name="Destination">The byte array to be appended to</param>
        /// <param name="Input">The byte array to be appended</param>
        /// <returns>A new byte array with the given byte array appended to it</returns>
        public static byte[] AppendBytes(this byte[] Destination, byte[] Input)
        {
            byte[] Output = new byte[Input.LongLength + Destination.LongLength];
            Buffer.BlockCopy(Destination, 0, Output, 0, Destination.Length);
            Buffer.BlockCopy(Input, 0, Output, Destination.Length, Input.Length);
            return Output;
        }

        /// <summary>
        /// Appends a single byte to the end of another byte array
        /// </summary>
        /// <param name="Destination">The byte array to be appended to</param>
        /// <param name="Input">The byte to be appended</param>
        /// <returns>A new byte array with the given byte appended to it</returns>
        public static byte[] AppendBytes(this byte[] Destination, byte Input)
        {
            byte[] Output = new byte[1 + Destination.LongLength];
            Buffer.BlockCopy(Destination, 0, Output, 0, Destination.Length);
            Buffer.SetByte(Output, Output.Length - 1, Input);
            return Output;
        }

        /// <summary>
        /// Appends an encoded byte array representation of an integer to the end of another byte array
        /// </summary>
        /// <param name="Destination">The byte array to be appended to</param>
        /// <param name="Input">The integer value to be encoded and appended</param>
        /// <param name="Shrink">Whether or not to shrink the integer to its smallest byte footprint (niche)</param>
        /// <returns>A new byte array with the given integer value encoded and appended to it</returns>
        public static byte[] AppendInteger<T>(this byte[] Destination, T Input, bool Shrink = false) where T : IComparable<T>
        {
            if (!Shrink)
            {
                byte[] tmp = IntegerToByteArray(Input);
                return Destination.AppendBytes(tmp);
            }
            else
            {
                dynamic val = Input;
                if (val < byte.MaxValue) val = (byte)val;
                else if (val < ushort.MaxValue) val = (ushort)val;
                else if (val < uint.MaxValue) val = (uint)val;
                else val = (ulong)val;
                return AppendInteger(Destination, val);
            }
        }

        /// <summary>
        /// Appends an encoded byte array representation of a string to the end of another byte array
        /// </summary>
        /// <param name="Destination">The byte array to be appended to</param>
        /// <param name="Input">The string value to be encoded and appended</param>
        /// <returns>A new byte array with the given string value encoded and appended to it</returns>
        public static byte[] AppendString(this byte[] Destination, string Input)
        {
            byte[] tmp = Encoding.UTF8.GetBytes(Input);
            return Destination.AppendBytes(tmp);
        }

        /// <summary>
        /// Appends a string to the end of another byte array
        /// </summary>
        /// <param name="Destination">The byte array to be appended to</param>
        /// <param name="Input">The hex string value to be encoded and appended</param>
        /// <returns>A new byte array with the given string value encoded and appended to it</returns>
        public static byte[] AppendHexString(this byte[] Destination, string Input)
        {
            if (Input.Length % 2 != 0) throw new ArgumentException("Invalid hex string given");
            byte[] tmp = HexStringToByteArray(Input);
            return Destination.AppendBytes(tmp);
        }

        /// <summary>
        /// Appends an encoded protobuf style varint byte array representation of an integer to the end of another byte array
        /// </summary>
        /// <param name="Destination">The byte array to be appended to</param>
        /// <param name="Input">The integer value to be encoded and tappended</param>
        /// <returns>A new byte array with the given integer value encoded and appended to it</returns>
        public static byte[] AppendVarInt<T>(this byte[] Destination, T Input) where T : IComparable<T>
        {
            ulong Converted = Convert.ToUInt64(Input);
            while (Converted >= 0x80)
            {
                Destination = Destination.AppendInteger((byte)(Converted & 0x7f | 0x80));
                Converted >>= 7;
            }

            // Lazy workarounds
            if (Converted == 0x7f) return Destination.AppendHexString("FF");

            // Append final byte value
            return Destination.AppendInteger((byte)Converted);
        }

        /// <summary>
        /// Retrieves a section from a larger byte array
        /// </summary>
        /// <param name="Source">The byte array to be cut from</param>
        /// <param name="Offset">The position to start the new byte array</param>
        /// <param name="Length">How long the new byte array will be</param>
        /// <returns>A new byte array with contents copied from the source byte array</returns>
        public static byte[] SubBytes(this byte[] Source, int Offset, int Length = 0)
        {
            if (Length < 1) Length = Source.Length - Offset;
            byte[] Output = new byte[Length];
            Buffer.BlockCopy(Source, Offset, Output, 0, Length);
            return Output;
        }

        /// <summary>
        /// Retrieves a section from a larger byte array
        /// </summary>
        /// <param name="Source">The byte array to be cut from</param>
        /// <param name="Offset">The position to start the hex string</param>
        /// <param name="Length">How long the next hex string should be (in bytes)</param>
        /// <returns>A new hex string with contents copied from the source byte array</returns>
        public static string SubHexString(this byte[] Source, int Offset, int Length = 0)
        {
            if (Length < 1) Length = Source.Length - Offset;
            byte[] Output = new byte[Length];
            Buffer.BlockCopy(Source, Offset, Output, 0, Length);
            return ByteArrayToHexString(Output);
        }

        /// <summary>
        /// Determines whether two byte arrays match
        /// </summary>
        /// <param name="Self">The byte array to be compared from</param>
        /// <param name="Value">The byte array to be compared to</param>
        /// <returns>True if these two byte arrays match values</returns>
        public static bool Matches(this byte[] Self, ReadOnlySpan<byte> Value)
        {
            ReadOnlySpan<byte> Tmp = Self;
            return Tmp.SequenceEqual(Value);
        }

        /// <summary>
        /// Converts this byte array to a string
        /// </summary>
        /// <param name="Self">The byte array to convert</param>
        /// <param name="EncodeMethod">The encoding method to use</param>
        /// <returns>A string representing this byte array</returns>
        public static string ToString(this byte[] Self, Encoding EncodeMethod = null)
        {
            if (EncodeMethod == null) EncodeMethod = Encoding.Default;
            return EncodeMethod.GetString(Self);
        }

        /// <summary>
        /// Separates a byte array into chunks of a specified size
        /// </summary>
        /// <param name="Source">The byte array to split</param>
        /// <param name="ChunkSize">The amount of bytes in each chunk</param>
        /// <returns>A list of byte arrays of up to a specified length</returns>
        public static List<byte[]> ChunkBy(this byte[] Source, int ChunkSize)
        {
            return Source.Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / ChunkSize)
                .Select(x => x.Select(v => v.Value).ToArray())
                .ToList();
        }

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Retrieves a unix timestamp of the current time
        /// </summary>
        /// <returns>A unix timestamp representing the current time</returns>
        public static ulong GetTimestamp()
        {
            return (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Gets the time delta between the current time and a given timestamp
        /// </summary>
        /// <param name="Timestamp">A timestamp to compare against the current time</param>
        /// <returns>The difference between the current time and the given timestamp</returns>
        public static ulong GetTimeDelta(ulong Timestamp)
        {
            return (ulong)Math.Abs((long)GetTimestamp() - (long)Timestamp);
        }

        /// <summary>
        /// Combines a directory path and file name into a single string
        /// </summary>
        /// <param name="DirectoryPath">The directory the specified file name belongs in, 
        /// which defaults to the current directory if null or empty</param>
        /// <param name="FileName">The name of the file to combine</param>
        /// <returns>A string with both the directory path and file name</returns>
        public static string CombinePath(string DirectoryPath, string FileName)
        {
            // Check if directory path is not specified
            if (string.IsNullOrEmpty(DirectoryPath))
            {
                // Set to current directory
                DirectoryPath = Environment.CurrentDirectory;
            }

            // Combine the two paths
            return Path.Combine(DirectoryPath, FileName);
        }

        /// <summary>
        /// Gets the system's application data directory path
        /// </summary>
        /// <param name="SubFolder">(OPTIONAL) An optional sub-folder that will be appended to </param>
        /// <returns>A string location of the application data folder with optional sub-folder</returns>
        public static string GetAppDataPath(string SubFolder = "")
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrEmpty(SubFolder)) return AppData;
            else return Path.Combine(AppData, SubFolder);
        }

        /// <summary>
        /// Converts an unsigned integer representation of an IP address into a human-readable format
        /// </summary>
        /// <param name="Ip">An unsigned integer representation of an IP address</param>
        /// <returns>A human-readable IP address string</returns>
        public static string IpAddressFromUint(uint Ip)
        {
            return new IPAddress(new byte[] {
                (byte)((Ip >> 24) & 0xFF),
                (byte)((Ip >> 16) & 0xFF),
                (byte)((Ip >> 8)  & 0xFF),
                (byte)(Ip & 0xFF)
            }).ToString();
        }

        /// <summary>
        /// Converts a human readable IP address to an unsigned integer representation of that address
        /// </summary>
        /// <param name="Ip">A human readable IP address string</param>
        /// <returns>An unsigned integer representation of the given IP address</returns>
        public static uint IpAddressToUint(string Address)
        {
            var Buffer = IPAddress.Parse(Address).GetAddressBytes();
            var Output = (uint)Buffer[3] << 24;
            Output += (uint)Buffer[2] << 16;
            Output += (uint)Buffer[1] << 8;
            Output += Buffer[0];
            return Output;
        }

        /// <summary>
        /// Checks whether or not a string is a valid 32 byte hex string
        /// </summary>
        /// <param name="Value">The string to verify</param>
        /// <returns>True if the string respresents a 32 byte hex string</returns>
        public static bool IsKey(string Value)
        {
            if (Value.Length % 2 == 0 && Value.Length == 64) return true;
            return false;
        }

        #endregion
    }
}
