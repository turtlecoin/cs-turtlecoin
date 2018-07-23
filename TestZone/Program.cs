using System;

using Canti.Blockchain.Crypto;

namespace TestZone
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Add some code here to mess around with the codebase!");
            Console.WriteLine();
            Console.WriteLine("To get you started, here's a set of private keys and their corresponding address!");

            WalletKeys keys = KeyOps.GenerateWalletKeys();

            string address = Addresses.AddressFromKeys(keys.spendKeys.publicKey,
                                                       keys.viewKeys.publicKey);

            Console.WriteLine();

            Console.WriteLine($"Private spend key: {keys.spendKeys.privateKey.ToString()}");
            Console.WriteLine($"Private view key: {keys.viewKeys.privateKey.ToString()}");

            Console.WriteLine();

            Console.WriteLine($"Public address: {address}");
        }
    }
}
