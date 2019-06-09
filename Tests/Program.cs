//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Cryptography;
using System;

namespace CryptoTests
{
    class Program
    {
        static void Main()
        {
            KeyPair Keys = new TurtleCoinCrypto().GenerateKeys();
            Console.WriteLine($"Generated Private Key: {Keys.PrivateKey}");
            Console.WriteLine($"Generated Public Key: {Keys.PublicKey}");

            KeyTests.RunTests(Keys, new TurtleCoinCrypto());
            KeyTests.RunTests(Keys, new NativeCrypto());

            // Run pinvoke tests
            //Console.WriteLine("Pinvoked Crypto Tests:");
            //HashTests.RunTests(new TurtleCoinCrypto());

            // Run native tests
            //Console.WriteLine("Native Crypto Tests:");
            //HashTests.RunTests(new NativeCrypto());

            //KeyTests.RunTests();

            //Benchmarking.RunTests();

            Console.ReadLine();
        }
    }
}
