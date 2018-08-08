using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Blockchain.Crypto;
using Canti.Data;
using Canti.Utilities;

namespace Tests
{
    public class HashTests
    {
        public static void Test(Dictionary<string, string> testVectors,
                                IHashProvider hashFunction)
        {
            foreach (var(input, expectedOutput) in testVectors.Tuples())
            {
                byte[] inputArray = Encoding.StringToByteArray(input);

                string actualOutput = Encoding.ByteArrayToHexString(
                    hashFunction.Hash(inputArray)
                );
                
                Assert.AreEqual<string>(expectedOutput, actualOutput);
            }
        }
    }
}
