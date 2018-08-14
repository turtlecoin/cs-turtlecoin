//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Blockchain.Crypto;
using System;

namespace TestZone
{
    partial class Program
    {
        // Test key generation
        static void TestKeyGeneration()
        {
            // Generate a set of wallet keys
            WalletKeys Keys = KeyOps.GenerateWalletKeys();

            // Get an address string from the generated wallet keys
            string Address = Addresses.AddressFromKeys(Keys.publicSpendKey, Keys.publicViewKey);

            // Output generated keys to console
            Console.WriteLine($"Private spend key: {Keys.privateSpendKey.ToString()}");
            Console.WriteLine($"Private view key: {Keys.privateViewKey.ToString()}");

            // Output address to console
            Console.WriteLine($"Public address: {Address}");
            Console.WriteLine();
        }
    }
}
