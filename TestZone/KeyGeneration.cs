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
            string Address = Addresses.AddressFromKeys(Keys.spendKeys.publicKey, Keys.viewKeys.publicKey);

            // Output generated keys to console
            Console.WriteLine($"Private spend key: {Keys.spendKeys.privateKey.ToString()}");
            Console.WriteLine($"Private view key: {Keys.viewKeys.privateKey.ToString()}");

            // Output address to console
            Console.WriteLine($"Public address: {Address}");
            Console.WriteLine();
        }
    }
}
