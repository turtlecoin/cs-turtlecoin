//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti;
using System;

using static Canti.Cryptography.Crypto;

namespace CryptoTests
{
    class Benchmarking
    {
        public static void RunTests()
        {
            Console.WriteLine("Starting hashing benchmarks...");

            Console.WriteLine();

            string Seed = SecureRandom.String(128);
            Console.WriteLine($"Generated seed: {Seed}");

            Console.WriteLine("Benchmarking 1,000 iterations of Fast Hash...");
            string Output = Seed;
            var Time = Benchmark.Run(() => { Output = CN_FastHash(Output); }, 1000);
            Console.WriteLine($"Benchmark took {Time.ToString(@"hh\:mm\:ss\.ffffff")} to finish");
            Console.WriteLine($"Final hash: {Output}");

            Console.WriteLine();

            Console.WriteLine("Benchmarking 1,000 iterations of Chukwa Slow Hash...");
            Output = Seed;
            Time = Benchmark.Run(() => { Output = ChukwaSlowHash(Output); }, 1000);
            Console.WriteLine($"Benchmark took {Time.ToString(@"hh\:mm\:ss\.ffffff")} to finish");
            Console.WriteLine($"Final hash: {Output}");

            Console.WriteLine();
        }
    }
}
