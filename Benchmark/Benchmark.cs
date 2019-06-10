using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

using static Canti.Utils;

using Canti;
using Canti.Cryptography;
using Canti.Cryptography.Native;
using Canti.Cryptography.Native.CryptoNight;

namespace Benchmark
{
    class CppWrapper : IHashProvider
    {
        public delegate string HashFunc(string data);

        public CppWrapper(HashFunc func)
        {
            this.Func = func;
        }

        public byte[] Hash(byte[] input)
        {
            return HexStringToByteArray(Func(ByteArrayToHexString(input)));
        }

        private HashFunc Func;
    }

    class Benchmark
    {
        static void Main(string[] args)
        {
            var cpp = new TurtleCoinCrypto();

            var hashes = new List<HashFunction>
            {
                new HashFunction("Blake",                               new Blake(),                                2_000_000),

                new HashFunction("CNV0 (C++ Pinvoke)",                  new CppWrapper(cpp.CN_SlowHashV0),          500),
                new HashFunction("CNV0 (C# Platform Specific)",         new CNV0(),                                 500),
                new HashFunction("CNV0 (C# Platform Independent)",      new CNV0(false),                            60),

                new HashFunction("CNLiteV1 (C++ Pinvoke)",              new CppWrapper(cpp.CN_LiteSlowHashV1),      1000),
                new HashFunction("CNLiteV1 (C# Platform Specific)",     new CNLiteV1(),                             1000),
                new HashFunction("CNLiteV1 (C# Platform Independent)",  new CNLiteV1(false),                        150),

                new HashFunction("CNTurtle (C++ Pinvoke)",              new CppWrapper(cpp.CN_TurtleLiteSlowHashV2),3000),
                new HashFunction("CNTurtle (C# Platform Specific)",     new CNTurtleLiteV2(),                       3000),
                new HashFunction("CNTurtle (C# Platform Independent)",  new CNTurtleLiteV2(false),                  60),

                new HashFunction("Groestl",                             new Groestl(),                              200_000),
                new HashFunction("JH",                                  new JH(),                                   50000),

                new HashFunction("Keccak (C++ Pinvoke)",                new CppWrapper(cpp.CN_FastHash),            2_000_000),
                new HashFunction("Keccak (C#)",                         new Keccak(),                               2_000_000),

                new HashFunction("Skein",                               new Skein(),                                3_000_000)
            };

            while (true)
            {
                // Write command menu
                Console.WriteLine("Menu:");

                int index = 1;

                foreach (var hashFunction in hashes)
                {
                    Console.WriteLine($"{index}. Benchmark the {hashFunction.Name} hash function");
                    index++;
                }

                Console.WriteLine($"{index}. Benchmark ALL");

                index++;

                Console.WriteLine($"{index}. Exit");

                #if DEBUG
                Console.WriteLine("Warning: You are testing in DEBUG mode - Reported hash speeds will be inaccurate.");
                #endif

                if (!Aes.IsSupported || !Bmi2.X64.IsSupported || !Sse2.IsSupported)
                {
                    Console.WriteLine("Your platform does not support intrinsics - Platform Specific falls back to Platform Independent");
                }

                Console.Write("Enter your selection: ");

                // Get selection
                var selection = Console.ReadLine();
                Console.WriteLine();

                if (int.TryParse(selection, out int selectionNum))
                {
                    if (selectionNum > 0 && selectionNum <= hashes.Count)
                    {
                        HashBenchmark(hashes[selectionNum - 1]);
                    }
                    /* Test all */
                    else if (selectionNum == hashes.Count + 1)
                    {
                        foreach (var hashFunction in hashes)
                        {
                            HashBenchmark(hashFunction);
                            Console.WriteLine();
                        }
                    }
                    /* Exit */
                    else if (selectionNum == hashes.Count + 2)
                    {
                        Console.WriteLine("Bye.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input...");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input...");
                }

                Console.WriteLine();
            }
        }

        public static void HashBenchmark(HashFunction hash)
        { 
            var Result = Canti.Benchmark.Run(() => {
                byte[] input = SecureRandom.Bytes(64);
                hash.Function.Hash(input);
            }, hash.Iterations);

            string elapsedTime = Result.ToString(@"hh\:mm\:ss\.ff");

            Console.WriteLine($"Ran {hash.Iterations} iterations of {hash.Name}:");
            Console.WriteLine($"Runtime: {elapsedTime}");
            Console.WriteLine($"Hashes per second: {hash.Iterations / Result.TotalSeconds:F2}");
        }
    }

    public class HashFunction
    {
        public HashFunction(string name, IHashProvider hashFunction, int iterations)
        {
            this.Name = name;
            this.Function = hashFunction;
            this.Iterations = iterations;
        }

        public string Name { get; }

        public IHashProvider Function { get; }

        public int Iterations { get; }
    }
}
