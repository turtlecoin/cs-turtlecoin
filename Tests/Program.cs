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
