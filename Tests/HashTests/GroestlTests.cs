using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Cryptography.Native;

/* Helpful for testing: https://asecuritysite.com/encryption/gro */
namespace Tests
{
    [TestClass]
    public class GroestlTests
    {
        [TestMethod]
        public void TestGroestl()
        {
            var testVectors = new Dictionary<string, string>
            {
                { "", "1a52d11d550039be16107f9c58db9ebcc417f16f736adb2502567119f0083467" },
                { "The quick brown fox jumps over the lazy dog", "8c7ad62eb26a21297bc39c2d7293b4bd4d3399fa8afab29e970471739e28b301" },
                { "The quick brown fox jumps over the lazy dog.", "f48290b1bcacee406a0429b993adb8fb3d065f4b09cbcdb464a631d4a0080aaf" },
                { "I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself", "c0a2d0128630376ffa594518d8a59585abe88a86e5be34701e5e10a641e5bea6" }
            };

            HashTests.Test(testVectors, new Groestl());
        }
    }
}
