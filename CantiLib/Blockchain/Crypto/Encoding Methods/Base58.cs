//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Data;
using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace Canti.Blockchain.Crypto
{
    public static class Base58
    {
        // Private variables
        private const string Characters = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private const char LeadingCharacter = '1';
        private const int CheckSumSizeInBytes = 4;

        // Encodes a value or set of values
        public static string Encode(byte[] Data)
        {
            // Decode input to a big integer value
            BigInteger Integer = 0;
            for (int i = 0; i < Data.Length; i++) Integer = Integer * 256 + Data[i];

            // Start populating output string
            string Output = "";
            while (Integer > 0)
            {
                // Get divisible remainder
                int Remainder = (int)(Integer % 58);
                Integer /= 58;

                // Add a base58 character based off the position of the remainder
                Output = Characters[Remainder] + Output;
            }

            // Add leading digits
            for (int i = 0; i < Data.Length && Data[i] == 0; i++) Output = '1' + Output;

            // Return output
            return Output;
        }
        public static string EncodeWithCheckSum(byte[] Data)
        {
            // Encode with checksum input
            return Encode(AddCheckSum(Data));
        }

        // Decodes a value
        public static byte[] Decode(string Data)
        {
            // Decode base58 string to a big integer value
            BigInteger Integer = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                // Check character validity
                int Digit = Characters.IndexOf(Data[i]);
                if (Digit < 0) throw new FormatException("Invalid Base58 input string");

                // Add base58 derived value to the big integer
                Integer = Integer * 58 + Digit;
            }

            // Get 
            int LeadingZeroCount = Data.TakeWhile(c => c == '1').Count();
            var LeadingZeros = Enumerable.Repeat((byte)0, LeadingZeroCount);
            byte[] BytesWithoutLeadingZeros = new byte[0];

            // Little endian systems
            if (BitConverter.IsLittleEndian) BytesWithoutLeadingZeros = Integer.ToByteArray().Reverse().SkipWhile(b => b == 0).ToArray();

            // Big endian systems
            else BytesWithoutLeadingZeros = Integer.ToByteArray().SkipWhile(b => b == 0).ToArray();

            // Return output
            return LeadingZeros.Concat(BytesWithoutLeadingZeros).ToArray();
        }
        public static byte[] DecodeWithCheckSum(string Data)
        {
            // Decode base58 string to get the data with checksum
            byte[] OutputWithCheckSum = Decode(Data);

            // Verify checksum value
            byte[] Output = VerifyCheckSum(OutputWithCheckSum);
            if (Output == null) throw new FormatException("Base58 checksum is invalid");

            // Return output
            return Output;
        }

        // Gets checksum
        public static byte[] GetCheckSum(byte[] Data)
        {
            // Create a managed SHA256 provider
            SHA256 Sha256 = new SHA256Managed();

            // Compute a hash based on input data
            byte[] HashA = Sha256.ComputeHash(Data);

            // Re-hash the first hash
            byte[] HashB = Sha256.ComputeHash(HashA);

            // Create an output array
            byte[] Output = new byte[CheckSumSizeInBytes];

            // Copy hash b into the output with checksum length
            Buffer.BlockCopy(HashB, 0, Output, 0, Output.Length);

            // Return output
            return Output;
        }

        // Adds checksum value
        public static byte[] AddCheckSum(byte[] Data)
        {
            // Get checksum of data
            byte[] CheckSum = GetCheckSum(Data);

            // Output input data with checksum appended to it
            return Encoding.AppendToByteArray(CheckSum, Data);
        }

        // Verify checksum value
        public static byte[] VerifyCheckSum(byte[] Data)
        {
            // Remove checksum from input
            byte[] Output = Encoding.SplitByteArray(Data, 0, Data.Length - CheckSumSizeInBytes);

            // Get supplied checksum
            byte[] SuppliedCheckSum = Encoding.SplitByteArray(Data, Data.Length - CheckSumSizeInBytes, CheckSumSizeInBytes);

            // Check if checksum is valid
            byte[] ValidCheckSum = GetCheckSum(Output);

            // Valid
            if (SuppliedCheckSum.SequenceEqual(ValidCheckSum)) return Output;

            // Invalid
            else return null;
        }
    }
}
