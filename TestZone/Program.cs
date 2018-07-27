using System;

using Canti.Blockchain.Crypto;

namespace TestZone
{
    partial class Program
    {
        static void Main(string[] args)
        {
            // Begin testing!
            Console.WriteLine("Add some code here to mess around with the codebase!");

            // Get menu choice
            while (true)
            {
                /* 
                 * Add your test code here!
                 * There are a few tests added as an example,
                 * but feel free to add your own to play with
                 * what the codebase offers!
                 * :)
                 */

                // Write command menu
                Console.WriteLine("Menu:");
                Console.WriteLine(" 1. Test wallet key generation");
                Console.WriteLine(" 2. Test encryption");
                Console.WriteLine(" 3. Exit");
                Console.Write("Enter your selection: ");

                // Get selection
                var Selection = Console.ReadKey();
                Console.WriteLine();

                // Read selection
                switch (Selection.KeyChar)
                {
                    // Test key generation
                    case '1':
                        TestKeyGeneration();
                        continue;

                    // Test cryptography
                    case '2':
                        TestCryptography();
                        continue;

                    // Exit
                    case '3':
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
