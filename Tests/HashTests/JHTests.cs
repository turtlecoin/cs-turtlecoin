using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Cryptography.Native;

namespace Tests
{
    [TestClass]
    public class JHTests
    {
        [TestMethod]
        public void TestJH()
        {
            var testVectors = new Dictionary<string, string>
            {
                { "", "46e64619c18bb0a92a5e87185a47eef83ca747b8fcc8e1412921357e326df434" },
                { "The quick brown fox jumps over the lazy dog", "6a049fed5fc6874acfdc4a08b568a4f8cbac27de933496f031015b38961608a0" },
                { "The quick brown fox jumps over the lazy dog.", "d001ae2315421c5d3272bac4f4aa524bddd207530d5d26bbf51794f0da18fafc" },
                { "I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself", "072ae5d7355bc133eb39983d74068a7b926f198bd27d2c7770a5272b15c84c1e" }
            };

            HashTests.Test(testVectors, new JH());
        }
    }
}
