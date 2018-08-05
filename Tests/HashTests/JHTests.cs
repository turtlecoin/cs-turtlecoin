using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.JH;
using Canti.Data;

namespace Tests
{
    [TestClass]
    public class JHTests
    {
        [TestMethod]
        public void TestJH()
        {
            byte[] input1 = Encoding.StringToByteArray("");
            string actualOutput1 = Encoding.ByteArrayToHexString(JH.jh(input1));
            string expectedOutput1 = "46e64619c18bb0a92a5e87185a47eef83ca747b8fcc8e1412921357e326df434";

            Assert.AreEqual<string>(expectedOutput1, actualOutput1);

            byte[] input2 = Encoding.StringToByteArray("The quick brown fox jumps over the lazy dog");
            string actualOutput2 = Encoding.ByteArrayToHexString(JH.jh(input2));
            string expectedOutput2 = "6a049fed5fc6874acfdc4a08b568a4f8cbac27de933496f031015b38961608a0";

            Assert.AreEqual<string>(expectedOutput2, actualOutput2);

            byte[] input3 = Encoding.StringToByteArray("The quick brown fox jumps over the lazy dog.");
            string actualOutput3 = Encoding.ByteArrayToHexString(JH.jh(input3));
            string expectedOutput3 = "d001ae2315421c5d3272bac4f4aa524bddd207530d5d26bbf51794f0da18fafc";

            Assert.AreEqual<string>(expectedOutput3, actualOutput3);

            byte[] input4 = Encoding.StringToByteArray("I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself");
            string actualOutput4 = Encoding.ByteArrayToHexString(JH.jh(input4));
            string expectedOutput4 = "072ae5d7355bc133eb39983d74068a7b926f198bd27d2c7770a5272b15c84c1e";

            Assert.AreEqual<string>(expectedOutput4, actualOutput4);
        }
    }
}
