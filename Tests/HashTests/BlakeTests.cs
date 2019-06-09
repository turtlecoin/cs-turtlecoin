using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static Canti.Utils;
using Canti.Cryptography.Native;

/* Helpful for testing: https://asecuritysite.com/encryption/blake */
namespace Tests
{
    [TestClass]
    public class BlakeTests
    {
        [TestMethod]
        public void TestBlake()
        {
            var testVectors = new Dictionary<string, string>
            {
                { "", "716f6e863f744b9ac22c97ec7b76ea5f5908bc5b2f67c61510bfc4751384ea7a" },
                { "The quick brown fox jumps over the lazy dog", "7576698ee9cad30173080678e5965916adbb11cb5245d386bf1ffda1cb26c9d7" },
                { "The quick brown fox jumps over the lazy dog.", "13af722eafeab6bb2ed498129044e6782c84a7604bba9988b135d98158fbe816" },
                { "I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself", "4c1c8d8e21f59dab627b7a98da6d7d6faf60a31a2dfe3ddf50905e1c07e066c6" }
            };

            HashTests.Test(testVectors, new Blake());
        }
    }
}
