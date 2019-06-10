//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Cryptography;
using System;
using System.Diagnostics;

namespace CryptoTests
{
    class HashTests
    {
        public static void RunTests(ICryptography Crypto)
        {
            Stopwatch s = new Stopwatch();
            s.Restart();

            string Seed = "0100fb8e8ac805899323371bb790db19218afd8db8e3755d8b90f39b3d5506a9abce4fa912244500000000ee8146d49fa93ee724deb57d12cbc6c6f3b924d946127c7a97418f9348828f0f02";
            Console.WriteLine($"Seed: {Seed}");

            string FastHash = Crypto.CN_FastHash(Seed);
            Console.WriteLine($"Fast Hash: {FastHash}");

            string SlowHashV0 = Crypto.CN_SlowHashV0(Seed);
            Console.WriteLine($"Slow Hash V0: {SlowHashV0}");

            string SlowHashV1 = Crypto.CN_SlowHashV1(Seed);
            Console.WriteLine($"Slow Hash V1: {SlowHashV1}");

            string SlowHashV2 = Crypto.CN_SlowHashV2(Seed);
            Console.WriteLine($"Slow Hash V2: {SlowHashV2}");

            string LiteSlowHashV0 = Crypto.CN_LiteSlowHashV0(Seed);
            Console.WriteLine($"Lite Slow Hash V0: {LiteSlowHashV0}");

            string LiteSlowHashV1 = Crypto.CN_LiteSlowHashV1(Seed);
            Console.WriteLine($"Lite Slow Hash V1: {LiteSlowHashV1}");

            string LiteSlowHashV2 = Crypto.CN_LiteSlowHashV2(Seed);
            Console.WriteLine($"Lite Slow Hash V2: {LiteSlowHashV2}");

            string DarkSlowHashV0 = Crypto.CN_DarkSlowHashV0(Seed);
            Console.WriteLine($"Dark Slow Hash V0: {DarkSlowHashV0}");

            string DarkSlowHashV1 = Crypto.CN_DarkSlowHashV1(Seed);
            Console.WriteLine($"Dark Slow Hash V1: {DarkSlowHashV1}");

            string DarkSlowHashV2 = Crypto.CN_DarkSlowHashV2(Seed);
            Console.WriteLine($"Dark Slow Hash V2: {DarkSlowHashV2}");

            string DarkLiteSlowHashV0 = Crypto.CN_DarkLiteSlowHashV0(Seed);
            Console.WriteLine($"Dark Lite Slow Hash V0: {DarkLiteSlowHashV0}");

            string DarkLiteSlowHashV1 = Crypto.CN_DarkLiteSlowHashV1(Seed);
            Console.WriteLine($"Dark Lite Slow Hash V1: {DarkLiteSlowHashV1}");

            string DarkLiteSlowHashV2 = Crypto.CN_DarkLiteSlowHashV2(Seed);
            Console.WriteLine($"Dark Lite Slow Hash V2: {DarkLiteSlowHashV2}");

            string TurtleSlowHashV0 = Crypto.CN_TurtleSlowHashV0(Seed);
            Console.WriteLine($"Turtle Slow Hash V0: {TurtleSlowHashV0}");

            string TurtleSlowHashV1 = Crypto.CN_TurtleSlowHashV1(Seed);
            Console.WriteLine($"Turtle Slow Hash V1: {TurtleSlowHashV1}");

            string TurtleSlowHashV2 = "";
            try { TurtleSlowHashV2 = Crypto.CN_TurtleSlowHashV2(Seed); } catch { }
            Console.WriteLine($"Turtle Slow Hash V2: {TurtleSlowHashV2}");

            string TurtleLiteSlowHashV0 = Crypto.CN_TurtleLiteSlowHashV0(Seed);
            Console.WriteLine($"Turtle Lite Slow Hash V0: {TurtleLiteSlowHashV0}");

            string TurtleLiteSlowHashV1 = Crypto.CN_TurtleLiteSlowHashV1(Seed);
            Console.WriteLine($"Turtle Lite Slow Hash V1: {TurtleLiteSlowHashV1}");

            string TurtleLiteSlowHashV2 = Crypto.CN_TurtleLiteSlowHashV2(Seed);
            Console.WriteLine($"Turtle Lite Slow Hash V2: {TurtleLiteSlowHashV2}");

            /*string SoftShellSlowHashV0 = Crypto.CN_SoftShellSlowHashV0(Seed, 100_000);
            Console.WriteLine($"Soft Shell Slow Hash V0 at height 100,000: {SoftShellSlowHashV0}");

            string SoftShellSlowHashV1 = Crypto.CN_SoftShellSlowHashV1(Seed, 100_000);
            Console.WriteLine($"Soft Shell Slow Hash V1 at height 100,000: {SoftShellSlowHashV1}");

            string SoftShellSlowHashV2 = Crypto.CN_SoftShellSlowHashV2(Seed, 100_000);
            Console.WriteLine($"Soft Shell Slow Hash V2 at height 100,000: {SoftShellSlowHashV2}");

            string Chukwa_SlowHash = ChukwaSlowHash(Seed);
            Console.WriteLine($"Chuka Slow Hash: {Chukwa_SlowHash}");*/

            s.Stop();
            Console.WriteLine($"Time taken: {s.Elapsed.ToString(@"hh\:mm\:ss\.ffffff")}");

            Console.WriteLine();
        }
    }
}
