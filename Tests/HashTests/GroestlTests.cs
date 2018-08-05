using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.Groestl;
using Canti.Data;

/* Helpful for testing: https://asecuritysite.com/encryption/gro */
namespace Tests
{
    [TestClass]
    public class GroestlTests
    {
        [TestMethod]
        public void TestGroestl()
        {
            byte[] input1 = Encoding.StringToByteArray("");
            string actualOutput1 = Encoding.ByteArrayToHexString(Groestl.groestl(input1));
            string expectedOutput1 = "1a52d11d550039be16107f9c58db9ebcc417f16f736adb2502567119f0083467";

            Assert.AreEqual<string>(expectedOutput1, actualOutput1);

            byte[] input2 = Encoding.StringToByteArray("The quick brown fox jumps over the lazy dog");
            string actualOutput2 = Encoding.ByteArrayToHexString(Groestl.groestl(input2));
            string expectedOutput2 = "8c7ad62eb26a21297bc39c2d7293b4bd4d3399fa8afab29e970471739e28b301";

            Assert.AreEqual<string>(expectedOutput2, actualOutput2);

            byte[] input3 = Encoding.StringToByteArray("The quick brown fox jumps over the lazy dog.");
            string actualOutput3 = Encoding.ByteArrayToHexString(Groestl.groestl(input3));
            string expectedOutput3 = "f48290b1bcacee406a0429b993adb8fb3d065f4b09cbcdb464a631d4a0080aaf";

            Assert.AreEqual<string>(expectedOutput3, actualOutput3);

            byte[] input4 = Encoding.StringToByteArray("I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself");
            string actualOutput4 = Encoding.ByteArrayToHexString(Groestl.groestl(input4));
            string expectedOutput4 = "c0a2d0128630376ffa594518d8a59585abe88a86e5be34701e5e10a641e5bea6";

            Assert.AreEqual<string>(expectedOutput4, actualOutput4);
        }
    }
}
