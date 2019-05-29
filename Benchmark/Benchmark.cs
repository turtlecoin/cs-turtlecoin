using System;
using System.Diagnostics;
using System.Collections.Generic;

using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.Blake;
using Canti.Blockchain.Crypto.CryptoNight;
using Canti.Blockchain.Crypto.Groestl;
using Canti.Blockchain.Crypto.JH;
using Canti.Blockchain.Crypto.Keccak;
using Canti.Blockchain.Crypto.Skein;

namespace Benchmark
{
    class Benchmark
    {
        static void Main(string[] args)
        {
            var hashes = new List<HashFunction>
            {
                new HashFunction("Blake",                           new Blake(),            500000),
                new HashFunction("CNV0",                            new CNV0(),             10),
                new HashFunction("CNV0 (Platform Independent)",     new CNV0(false),        10),
                new HashFunction("CNV1",                            new CNV1(),             10),
                new HashFunction("CNV1 (Platform Independent)",     new CNV1(false),        10),
                new HashFunction("CNLiteV0",                        new CNLiteV0(),         20),
                new HashFunction("CNLiteV0 (Platform Independent)", new CNLiteV0(false),    20),
                new HashFunction("CNLiteV1",                        new CNLiteV1(),         20),
                new HashFunction("CNLiteV1 (Platform Independent)", new CNLiteV1(false),    20),
                new HashFunction("Groestl",                         new Groestl(),          50000),
                new HashFunction("JH",                              new JH(),               10000),
                new HashFunction("Keccak",                          new Keccak(),           300000),
                new HashFunction("Skein",                           new Skein(),            500000)
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < hash.Iterations; i++)
            {
                byte[] input = SecureRandom.Bytes(64);
                hash.Function.Hash(input);
            }

            stopwatch.Stop();

            TimeSpan ts = stopwatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            Console.WriteLine($"Ran {hash.Iterations} iterations of {hash.Name}:");
            Console.WriteLine($"Runtime: {elapsedTime}");
            Console.WriteLine($"Hashes per second: {hash.Iterations / ts.TotalSeconds:F2}");
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
