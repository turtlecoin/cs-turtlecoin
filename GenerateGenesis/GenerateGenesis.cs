//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

// For generating a genesis transaction blob
namespace GenerateGenesis
{
    class Program
    {
        // Generate a genesis transaction hex string
        static string GenerateGenesisTransaction(string Address, ulong Amount)
        {
            // TODO - this *fun stuff*
            return "";
        }

        // Main entry point
        static void Main()
        {
            Console.WriteLine("Enter an address:");
            // TODO - verify address validity
            string Address = Console.ReadLine();

            Console.WriteLine();

            Console.WriteLine("Enter an amount:");
            ulong Amount = 0;
            while (!ulong.TryParse(Console.ReadLine(), out Amount))
            {
                Console.WriteLine("Enter an amount:");
            }

            Console.WriteLine();

            Console.WriteLine("Transaction hex (copy this into your currency config):");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(GenerateGenesisTransaction(Address, Amount));
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
