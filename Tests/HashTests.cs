//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti;
using System;
using static Canti.Cryptography.Crypto;

namespace CryptoTests
{
    class HashTests
    {
        public static void RunTests()
        {
            Console.WriteLine("Starting hashing tests...");

            Console.WriteLine();

            string Seed = SecureRandom.String(128);
            Console.WriteLine($"Generated seed: {Seed}");

            string FastHash = CN_FastHash(Seed);
            Console.WriteLine($"Fast Hash: {FastHash}");

            string SlowHashV0 = CN_SlowHashV0(Seed);
            Console.WriteLine($"Slow Hash V0: {SlowHashV0}");

            string SlowHashV1 = CN_SlowHashV1(Seed);
            Console.WriteLine($"Slow Hash V1: {SlowHashV1}");

            string SlowHashV2 = CN_SlowHashV2(Seed);
            Console.WriteLine($"Slow Hash V2: {SlowHashV2}");

            string LiteSlowHashV0 = CN_LiteSlowHashV0(Seed);
            Console.WriteLine($"Lite Slow Hash V0: {LiteSlowHashV0}");

            string LiteSlowHashV1 = CN_LiteSlowHashV1(Seed);
            Console.WriteLine($"Lite Slow Hash V1: {LiteSlowHashV1}");

            string LiteSlowHashV2 = CN_LiteSlowHashV2(Seed);
            Console.WriteLine($"Lite Slow Hash V2: {LiteSlowHashV2}");

            string DarkSlowHashV0 = CN_DarkSlowHashV0(Seed);
            Console.WriteLine($"Dark Slow Hash V0: {DarkSlowHashV0}");

            string DarkSlowHashV1 = CN_DarkSlowHashV1(Seed);
            Console.WriteLine($"Dark Slow Hash V1: {DarkSlowHashV1}");

            string DarkSlowHashV2 = CN_DarkSlowHashV2(Seed);
            Console.WriteLine($"Dark Slow Hash V2: {DarkSlowHashV2}");

            string DarkLiteSlowHashV0 = CN_DarkLiteSlowHashV0(Seed);
            Console.WriteLine($"Dark Lite Slow Hash V0: {DarkLiteSlowHashV0}");

            string DarkLiteSlowHashV1 = CN_DarkLiteSlowHashV1(Seed);
            Console.WriteLine($"Dark Lite Slow Hash V1: {DarkLiteSlowHashV1}");

            string DarkLiteSlowHashV2 = CN_DarkLiteSlowHashV2(Seed);
            Console.WriteLine($"Dark Lite Slow Hash V2: {DarkLiteSlowHashV2}");

            string TurtleSlowHashV0 = CN_TurtleSlowHashV0(Seed);
            Console.WriteLine($"Turtle Slow Hash V0: {TurtleSlowHashV0}");

            string TurtleSlowHashV1 = CN_TurtleSlowHashV1(Seed);
            Console.WriteLine($"Turtle Slow Hash V1: {TurtleSlowHashV1}");

            string TurtleSlowHashV2 = CN_TurtleSlowHashV2(Seed);
            Console.WriteLine($"Turtle Slow Hash V2: {TurtleSlowHashV2}");

            string TurtleLiteSlowHashV0 = CN_TurtleLiteSlowHashV0(Seed);
            Console.WriteLine($"Turtle Lite Slow Hash V0: {TurtleLiteSlowHashV0}");

            string TurtleLiteSlowHashV1 = CN_TurtleLiteSlowHashV1(Seed);
            Console.WriteLine($"Turtle Lite Slow Hash V1: {TurtleLiteSlowHashV1}");

            string TurtleLiteSlowHashV2 = CN_TurtleLiteSlowHashV2(Seed);
            Console.WriteLine($"Turtle Lite Slow Hash V2: {TurtleLiteSlowHashV2}");

            string SoftShellSlowHashV0 = CN_SoftShellSlowHashV0(Seed, 100_000);
            Console.WriteLine($"Soft Shell Slow Hash V0 at height 100,000: {SoftShellSlowHashV0}");

            string SoftShellSlowHashV1 = CN_SoftShellSlowHashV1(Seed, 100_000);
            Console.WriteLine($"Soft Shell Slow Hash V1 at height 100,000: {SoftShellSlowHashV1}");

            string SoftShellSlowHashV2 = CN_SoftShellSlowHashV2(Seed, 100_000);
            Console.WriteLine($"Soft Shell Slow Hash V2 at height 100,000: {SoftShellSlowHashV2}");

            string Chukwa_SlowHash = ChukwaSlowHash(Seed);
            Console.WriteLine($"Chuka Slow Hash: {Chukwa_SlowHash}");

            Console.WriteLine();
        }
    }
}
