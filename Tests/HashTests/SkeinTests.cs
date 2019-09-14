using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Cryptography.Native;

/* https://en.wikipedia.org/wiki/Skein_(hash_function)#Examples_of_Skein_hashes */
/* Useful for testing: https://asecuritysite.com/encryption/sk */
namespace Tests
{
    [TestClass]
    public class SkeinTests
    {
        [TestMethod]
        public void TestSkein()
        {
            var testVectors = new Dictionary<string, string>
            {
                { "", "39ccc4554a8b31853b9de7a1fe638a24cce6b35a55f2431009e18780335d2621" },
                { "The quick brown fox jumps over the lazy dog", "b3250457e05d3060b1a4bbc1428bc75a3f525ca389aeab96cfa34638d96e492a" },
                { "The quick brown fox jumps over the lazy dog.", "41e829d7fca71c7d7154ed8fc8a069f274dd664ae0ed29d365d919f4e575eebb" },
                { "I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself", "690ef219f8c4e792c6c22c365728b15459a8937d5b92287127f550d5157c70a2" }
            };

            HashTests.Test(testVectors, new Skein());
        }
    }
}
