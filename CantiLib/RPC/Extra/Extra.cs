//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using Canti.Data;

namespace Canti.RPC
{
    public class Extra
    {
        // public encoded hex string
        public string Hex { get; set; }

        // Holds a key-value collection of objects
        public List<object> Children { get; set; }

        // Overrides the to string function to return the encoded hex string
        public override string ToString()
        {
            return Hex;
        }

        // Decodes a string
        public static Extra FromString(string Hex)
        {
            // Create an extra object
            Extra Extra = new Extra
            {
                Children = new List<object>()
            };

            // Crop hex string to only the included data
            if (Hex.StartsWith("01")) Hex = Hex.Substring(66);

            // Check if there is data to be read
            while (Hex.StartsWith("02"))
            {
                
                // Crop to data position
                Hex = Hex.Substring(2);

                // Read size
                byte[] DataSizeBytes = Encoding.HexStringToByteArray(Hex.Substring(0, 8));
                int DataSize = Encoding.ByteArrayToInteger<int>(DataSizeBytes);

                // Get data string
                string DataString = Hex.Substring(8, DataSize * 2);

                // Decompress data
                byte[] DataBytes = Encoding.HexStringToByteArray(DataString);
                try { DataBytes = Encoding.DecompressByteArray(DataBytes); }
                catch { }

                // Decode hex string to dictionary
                try
                {
                    Extra.Children.Add(Encoding.DecodeObject<object>(DataBytes));
                }
                catch
                {
                    Extra.Children.Add(DataString);
                }

                // Crop to end of data position
                Hex = Hex.Substring(8 + DataSize * 2);
            }

            // Return extra object
            return Extra;
        }
    }
}
