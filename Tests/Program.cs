//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Cryptography;
using System;

namespace CryptoTests
{
    class Program
    {
        static void TestCrypto(ICryptography Crypto)
        {
            // Test key generation
            KeyPair Keys = Crypto.GenerateKeys();
            Console.WriteLine($"Generated Private Key: {Keys.PrivateKey}");
            Console.WriteLine($"Generated Public Key: {Keys.PublicKey}");

            // Test key checking
            bool CheckKey = Crypto.CheckKey(Keys.PublicKey);
            Console.WriteLine($"Check Key: {CheckKey}");
        }

        static void Main()
        {
            // Test native crypto
            ICryptography Native = new NativeCrypto();
            TestCrypto(Native);

            // Test external crypto
            ICryptography External = new TurtleCoinCrypto();
            TestCrypto(External);


            // Currently fucking around with ring sigs :t_shrug:

            KeyPair Keys = new TurtleCoinCrypto().GenerateKeys();

            // Currently banging my head on the wall

            string SEED1 = "00112233445566778899aabbccddeeff00112233445566778899aabbccddeeff";

            // Currently questioning my life choices

            string KeyImage = new TurtleCoinCrypto().GenerateKeyImage(Keys.PublicKey, Keys.PrivateKey);
            Console.WriteLine($"Key Image: {KeyImage}");

            // Currently seeking out the hardest alcohol I can find

            ulong Mixin = 2;
            ulong RealIndex = 0;
            string[] PublicKeys = new string[Mixin];
            for (int i = 0; i < PublicKeys.Length; i++)
            {
                PublicKeys[i] = new TurtleCoinCrypto().GenerateKeys().PublicKey;
            }
            PublicKeys[RealIndex] = Keys.PublicKey;

            // Currentku thinjig of iother thngs

            string[] RingSigs = new NativeCrypto().GenerateRingSignatures(SEED1, KeyImage, PublicKeys, Keys.PrivateKey, RealIndex);
            Console.WriteLine($"Ring Signatures:");
            foreach (var Sig in RingSigs)
            {
                Console.WriteLine($"  {Sig}");
            }

            // Curntnh klhl aaaa sdkd

            bool Check_RingSigs1 = new NativeCrypto().CheckRingSignatures(SEED1, KeyImage, PublicKeys, RingSigs);
            Console.WriteLine($"Check Ring Signatures: {Check_RingSigs1}");

            // dl;f vopd $#@#$L: T@3

            Console.ReadLine();
        }
    }
}
