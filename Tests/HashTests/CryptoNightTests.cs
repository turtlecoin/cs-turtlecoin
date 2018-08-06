using System;
using System.Collections.Generic;
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
        public void TestCNV0()
        {
            var testVectors = new Dictionary<string, string>();

            testVectors.Add("", "eb14e8a833fac6fe9a43b57b336789c46ffe93f2868452240720607b14387e11");
            testVectors.Add("The quick brown fox jumps over the lazy dog", "3ebb7f9f7d273d7c318d869477550cc800cfb11b0cadb7ffbdf6f89f3a471c59");
            testVectors.Add("The quick brown fox jumps over the lazy dog.", "e37cc1b6fabcd3652b6d2879ac806e39f591f9d1c20be0c6b99cf6bef31158a2");
            testVectors.Add("I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself", "d986f765ad299c605eba4712ffe11918ed9f39c4358949fd11a2cfd3f04fab35");

            HashTests.Test(testVectors, new CNV0());
        }

        [TestMethod]
        public void TestCNV1()
        {
            var testVectors = new Dictionary<string, string>();

            testVectors.Add("The quick brown fox jumps over the lazy dog", "94f5dec524fad6d32004c55c035e5ea223e7315be20e2dc5b8a0ac7464ffeb1f");
            testVectors.Add("The quick brown fox jumps over the lazy dog.", "86d34efc73e709dcc0f862725be692d1f8c5b407b4d730cd309acf80cc8f7c73");
            testVectors.Add("I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself", "246e1f4e7a61e0b29d4fefe33bb48b175468d28c0e44e84cb0cf244be8af9a12");

            HashTests.Test(testVectors, new CNV1());
        }


        [TestMethod]
        public void TestCNLiteV0()
        {
            var testVectors = new Dictionary<string, string>();

            testVectors.Add("", "4cec4a947f670ffdd591f89cdb56ba066c31cd093d1d4d7ce15d33704c090611");
            testVectors.Add("The quick brown fox jumps over the lazy dog", "fbbbc024c37acff2e7302275458447f888a8d6361ce407c391be72ed34c16fee");
            testVectors.Add("The quick brown fox jumps over the lazy dog.", "6d0e743fca6358bd0e9b365a68887fa9abf9f2940fe50682be28759b776d0fb0");
            testVectors.Add("I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself", "38c887c08c2c9398d411c4bbf9b3eb707087906a5326ebeb3b238e7867fb1a9b");

            HashTests.Test(testVectors, new CNLiteV0());
        }

        [TestMethod]
        public void TestCNLiteV1()
        {
            var testVectors = new Dictionary<string, string>();

            testVectors.Add("The quick brown fox jumps over the lazy dog", "973a324237703e0f2ebe678a0000a00afb14c2c394c1859c84bcaa7b90bc56db");
            testVectors.Add("The quick brown fox jumps over the lazy dog.", "b860f59d6aef32c7cacac02c4ac794066402dbd30c64c7fb733600a91441326d");
            testVectors.Add("I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself", "f7a5217873b802940a629573f6a100deb25764af254f8f4fef1a8b9d51ef3cc5");

            HashTests.Test(testVectors, new CNLiteV1());
        }
    }
}
