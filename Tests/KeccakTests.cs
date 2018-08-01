using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.Keccak;
using Canti.Data;

namespace Tests
{
    [TestClass]
    public class KeccakTests
    {
        [TestMethod]
        public void TestKeccak()
        {
            byte[] input1 = Encoding.HexStringToByteArray("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f");

            byte[] actualOutput1 = Keccak.keccak(input1);

            byte[] expectedOutput1 = Encoding.HexStringToByteArray("8ae1aa597fa146ebd3aa2ceddf360668dea5e526567e92b0321816a4e895bd2d");

            CollectionAssert.AreEqual(expectedOutput1, actualOutput1);

            byte[] input2 = Encoding.HexStringToByteArray("67c6697351ff4aec29cdbaabf2fbe3467cc254f81be8e78d765a2e63339fc99a");

            byte[] actualOutput2 = Keccak.keccak(input2);

            byte[] expectedOutput2 = Encoding.HexStringToByteArray("1968ed1e4a2f6c11cbe2a41b81da9f35fdd3cf5aca60613b7c4b50d0a1071889");

            CollectionAssert.AreEqual(expectedOutput2, actualOutput2);

            byte[] input3 = Encoding.HexStringToByteArray("f71c12de1b8b97205570247a2156a02159654a0fd86caa1661aea266b6de70ad");

            byte[] actualOutput3 = Keccak.keccak(input3);

            byte[] expectedOutput3 = Encoding.HexStringToByteArray("d8b50138c7d6d5c7305b4bbddebe19a9eca5702276bb2427f465a82ee2a40467");

            CollectionAssert.AreEqual(expectedOutput3, actualOutput3);
        }
    }
}
