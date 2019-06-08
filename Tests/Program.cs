//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace CryptoTests
{
    class Program
    {
        static void Main()
        {
            KeyTests.RunTests();

            HashTests.RunTests();

            Benchmarking.RunTests();

            Console.ReadLine();
        }
    }
}
