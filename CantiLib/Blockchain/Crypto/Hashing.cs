//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Data;
using System;
using System.Collections.Generic;

namespace Canti.Blockchain.Crypto
{
    class Hashing
    {
        // Serializes a string array into a hash array string
        public static string SerializeHashArray(string[] Value)
        {
            string Output = "";
            foreach (string Hash in Value)Output += Hash;
            return Output;
        }

        // Deserializes a hash array into a string array
        public static string[] DeserializeHashArray(string Value)
        {
            byte[] Data = new byte[0];
            try { Data = Encoding.HexStringToByteArray(Value); }
            catch { Data = Encoding.StringToByteArray(Value); }
            List<string> Output = new List<string>();
            for (int i = 0; i < Data.Length; i += 32)
                Output.Add(Encoding.ByteArrayToHexString(Encoding.SplitByteArray(Data, i, 32)));
            return Output.ToArray();
        }


    }
}
