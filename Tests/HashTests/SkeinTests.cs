using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.Skein;
using Canti.Data;

namespace Tests
{
    [TestClass]
    public class SkeinTests
    {
        /* https://en.wikipedia.org/wiki/Skein_(hash_function)#Examples_of_Skein_hashes */
        [TestMethod]
        public void TestSkein()
        {
            byte[] input1 = Encoding.StringToByteArray("");
            string actualOutput1 = Encoding.ByteArrayToHexString(Skein.skein(input1));
            string expectedOutput1 = "39ccc4554a8b31853b9de7a1fe638a24cce6b35a55f2431009e18780335d2621";

            Assert.AreEqual<string>(expectedOutput1, actualOutput1);

            byte[] input2 = Encoding.StringToByteArray("The quick brown fox jumps over the lazy dog");
            string actualOutput2 = Encoding.ByteArrayToHexString(Skein.skein(input2));
            string expectedOutput2 = "b3250457e05d3060b1a4bbc1428bc75a3f525ca389aeab96cfa34638d96e492a";

            Assert.AreEqual<string>(expectedOutput2, actualOutput2);

            byte[] input3 = Encoding.StringToByteArray("The quick brown fox jumps over the lazy dog.");
            string actualOutput3 = Encoding.ByteArrayToHexString(Skein.skein(input3));
            string expectedOutput3 = "41e829d7fca71c7d7154ed8fc8a069f274dd664ae0ed29d365d919f4e575eebb";

            Assert.AreEqual<string>(expectedOutput3, actualOutput3);

            byte[] input4 = Encoding.HexStringToByteArray("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeafb0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7");
            string actualOutput4 = Encoding.ByteArrayToHexString(Skein.skein(input4));
            string expectedOutput4 = "4469617682c766627aa08384cb41502a0288c711a6cc15c1a5f8016310e5b552";

            Assert.AreEqual<string>(expectedOutput4, actualOutput4);
        }
    }
}
