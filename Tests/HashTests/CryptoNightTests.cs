using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.CryptoNight;
using Canti.Data;

/* https://cryptonote.org/cns/cns008.txt */
namespace Tests
{
    [TestClass]
    public class CryptoNightTests
    {
        [TestMethod]
        public void TestCryptoNight()
        {
            IHashProvider p = new CNV0();

            /* Uses groestl as the final hashing function */
            byte[] input1 = Encoding.StringToByteArray("");
            string actualOutput1 = Encoding.ByteArrayToHexString(p.Hash(input1));
            string expectedOutput1 = "eb14e8a833fac6fe9a43b57b336789c46ffe93f2868452240720607b14387e11";

            Assert.AreEqual<string>(expectedOutput1, actualOutput1);

            /* Uses JH as the final hashing function */
            byte[] input2 = Encoding.StringToByteArray("The quick brown fox jumps over the lazy dog");
            string actualOutput2 = Encoding.ByteArrayToHexString(p.Hash(input2));
            string expectedOutput2 = "3ebb7f9f7d273d7c318d869477550cc800cfb11b0cadb7ffbdf6f89f3a471c59";

            Assert.AreEqual<string>(expectedOutput2, actualOutput2);

            /* Uses skein as the final hashing function */
            byte[] input3 = Encoding.StringToByteArray("The quick brown fox jumps over the lazy dog.");
            string actualOutput3 = Encoding.ByteArrayToHexString(p.Hash(input3));
            string expectedOutput3 = "e37cc1b6fabcd3652b6d2879ac806e39f591f9d1c20be0c6b99cf6bef31158a2";

            Assert.AreEqual<string>(expectedOutput3, actualOutput3);

            /* Uses JH as the final hashing function */
            byte[] input4 = Encoding.StringToByteArray("I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself");
            string actualOutput4 = Encoding.ByteArrayToHexString(p.Hash(input4));
            string expectedOutput4 = "d986f765ad299c605eba4712ffe11918ed9f39c4358949fd11a2cfd3f04fab35";

            Assert.AreEqual<string>(expectedOutput4, actualOutput4);
            
            /* Uses Blake as the final hashing function */
            byte[] input5 = Encoding.StringToByteArray("Blake I love you please be the final hashing function :(");
            string actualOutput5 = Encoding.ByteArrayToHexString(p.Hash(input5));
            string expectedOutput5 = "551c37241ee7a6730a1a65cc7257504e41e6c73e6320cfbf1295ed439756647d";

            Assert.AreEqual<string>(expectedOutput5, actualOutput5);
        }
    }
}
