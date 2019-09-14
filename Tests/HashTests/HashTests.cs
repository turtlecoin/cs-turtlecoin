using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static Canti.Utils;
using Canti.Cryptography;

namespace Tests
{
    public class HashTests
    {
        public static void Test(Dictionary<string, string> testVectors,
                                IHashProvider hashFunction)
        {
            foreach (var entry in testVectors)
            {
                byte[] inputArray = StringToByteArray(entry.Key);

                string actualOutput = ByteArrayToHexString(
                    hashFunction.Hash(inputArray)
                );
                
                Assert.AreEqual<string>(entry.Value, actualOutput);
            }
        }
    }
}
