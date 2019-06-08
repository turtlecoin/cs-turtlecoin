//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Cryptography;
using System;
using static Canti.Cryptography.Crypto;

namespace CryptoTests
{
    class KeyTests
    {
        public static void RunTests()
        {
            Console.WriteLine("Running key tests...");

            Console.WriteLine();

            KeyPair Keys = GenerateKeys();
            Console.WriteLine($"Generated Private Key: {Keys.PrivateKey}");
            Console.WriteLine($"Generated Public Key: {Keys.PublicKey}");

            bool Check_Key = CheckKey(Keys.PublicKey);
            Console.WriteLine($"Check Key: {Check_Key}");

            string KeyImage = GenerateKeyImage(Keys.PublicKey, Keys.PrivateKey);
            Console.WriteLine($"Key Image: {KeyImage}");

            KeyPair ViewKeys = GenerateViewKeysFromPrivateSpendKey(Keys.PrivateKey);
            Console.WriteLine($"Generated Private View Key: {ViewKeys.PrivateKey}");
            Console.WriteLine($"Generated Public View Key: {ViewKeys.PublicKey}");

            string PrivateViewKey = GeneratePrivateViewKeyFromPrivateSpendKey(Keys.PrivateKey);
            Console.WriteLine($"Generated Private View Key: {PrivateViewKey}");

            string Signature = GenerateSignature(Keys.PublicKey, Keys.PublicKey, Keys.PrivateKey);
            Console.WriteLine($"Generated Signature: {Signature}");


            bool Check_Signature = CheckSignature(Keys.PublicKey, Keys.PublicKey, Signature);
            Console.WriteLine($"Check Signature: {Check_Signature}");

            Console.WriteLine();
        }
    }
}
