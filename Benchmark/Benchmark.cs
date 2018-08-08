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
                new HashFunction("Blake", new Blake()),
                new HashFunction("CNV0", new CNV0()),
                new HashFunction("CNV1", new CNV1()),
                new HashFunction("CNLiteV0", new CNLiteV0()),
                new HashFunction("CNLiteV1", new CNLiteV1()),
                new HashFunction("Groestl", new Groestl()),
                new HashFunction("JH", new JH()),
                new HashFunction("Keccak", new Keccak()),
                new HashFunction("Skein", new Skein())
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
                    // Exit
                    else if (selectionNum == hashes.Count + 1)
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
            }
        }

        public static void HashBenchmark(HashFunction hash, int iterations = 100)
        { 
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
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

            Console.WriteLine($"Ran {iterations} iterations of {hash.Name}:");
            Console.WriteLine($"Runtime: {elapsedTime}");
            Console.WriteLine($"Hashes per second: {iterations / ts.TotalSeconds}");
        }
    }

    public class HashFunction
    {
        public HashFunction(string name, IHashProvider hashFunction)
        {
            this.Name = name;
            this.Function = hashFunction;
        }

        public string Name { get; }

        public IHashProvider Function { get; }
    }
}
