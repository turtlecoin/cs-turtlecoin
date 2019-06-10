//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Cryptography;
using Canti.Cryptography.Native.CryptoNight;
using System;

namespace CryptoTests
{
    class KeyTests
    {
        public static void RunTests(KeyPair Keys, ICryptography Crypto)
        {
            Console.WriteLine("Running key tests...");

            Console.WriteLine();

            /*KeyPair Keys = Crypto.GenerateKeys();
            Console.WriteLine($"Generated Private Key: {Keys.PrivateKey}");
            Console.WriteLine($"Generated Public Key: {Keys.PublicKey}");

            bool Check_Key = Crypto.CheckKey(Keys.PublicKey);
            Console.WriteLine($"Check Key: {Check_Key}");

            string KeyImage = Crypto.GenerateKeyImage(Keys.PublicKey, Keys.PrivateKey);
            Console.WriteLine($"Key Image: {KeyImage}");

            KeyPair ViewKeys = Crypto.GenerateViewKeysFromPrivateSpendKey(Keys.PrivateKey);
            Console.WriteLine($"Generated Private View Key: {ViewKeys.PrivateKey}");
            Console.WriteLine($"Generated Public View Key: {ViewKeys.PublicKey}");

            string PrivateViewKey = Crypto.GeneratePrivateViewKeyFromPrivateSpendKey(Keys.PrivateKey);
            Console.WriteLine($"Generated Private View Key: {PrivateViewKey}");

            string Signature = Crypto.GenerateSignature(Keys.PublicKey, Keys.PublicKey, Keys.PrivateKey);
            Console.WriteLine($"Generated Signature: {Signature}");

            bool Check_Signature = Crypto.CheckSignature(Keys.PublicKey, Keys.PublicKey, Signature);
            Console.WriteLine($"Check Signature: {Check_Signature}");

            string Reduced = Crypto.ScReduce32(SEED1);
            Console.WriteLine($"Sc Reduce 32: {Reduced}");

            string Scalar = Crypto.HashToScalar(SEED1);
            Console.WriteLine($"Hash To Scalar: {Scalar}");*/

            Console.WriteLine();
        }
    }
}
