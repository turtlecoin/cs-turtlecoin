using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.Blake256;
using Canti.Data;

namespace Tests
{
    [TestClass]
    public class Blake256Tests
    {
        [TestMethod]
        public void TestBlake256()
        {
            byte[] input1 = Encoding.StringToByteArray("");

            byte[] actualOutput1 = Blake256.blake256(input1);

            byte[] expectedOutput1 = Encoding.HexStringToByteArray("716f6e863f744b9ac22c97ec7b76ea5f5908bc5b2f67c61510bfc4751384ea7a");

            CollectionAssert.AreEqual(expectedOutput1, actualOutput1);

            byte[] input2 = Encoding.StringToByteArray("The quick brown fox jumped over the lazy dog");

            byte[] actualOutput2 = Blake256.blake256(input2);

            byte[] expectedOutput2 = Encoding.HexStringToByteArray("46212ed47e6fe8e624285146f7f31ee6e14a192109fbd1ae09bc53ef89e1a35a");

            CollectionAssert.AreEqual(expectedOutput2, actualOutput2);

            byte[] input3 = Encoding.HexStringToByteArray("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeafb0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7");

            byte[] actualOutput3 = Blake256.blake256(input3);

            byte[] expectedOutput3 = Encoding.HexStringToByteArray("c4d944c2b1c00a8ee627726b35d4cd7fe018de090bc637553cc782e25f974cba");

            CollectionAssert.AreEqual(expectedOutput3, actualOutput3);
        }
    }
}
