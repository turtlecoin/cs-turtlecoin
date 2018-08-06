//
// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using static Canti.Blockchain.Crypto.CryptoNight.CryptoNight;

namespace Canti.Blockchain.Crypto.CryptoNight
{
    public interface ICryptoNight
    {
        /* Gets the scratchpad size */
        int Memory();

        /* Gets the amount of iterations */
        int Iterations();

        /* Gets the variant of CN to use - currently supported, 0 and 1 */
        int Variant();
    }

    public class CNV0 : IHashProvider
    {
        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNV0Params());
        }

        private sealed class CNV0Params : ICryptoNight
        {
            /* 2 ^ 21 */
            public int Memory()
            {
                return 2097152;
            }

            /* 2 ^ 20 */
            public int Iterations()
            {
                return 1048576;
            }

            public int Variant()
            {
                return 0;
            }
        }
    }

    public class CNV1 : IHashProvider
    {
        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNV1Params());
        }

        private sealed class CNV1Params : ICryptoNight
        {
            /* 2 ^ 21 */
            public int Memory()
            {
                return 2097152;
            }

            /* 2 ^ 20 */
            public int Iterations()
            {
                return 1048576;
            }

            public int Variant()
            {
                return 1;
            }
        }
    }

    public class CNLiteV0 : IHashProvider
    {
        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNLiteV0Params());
        }

        private sealed class CNLiteV0Params : ICryptoNight
        {
            /* 2 ^ 20 */
            public int Memory()
            {
                return 1048576;
            }

            /* 2 ^ 19 */
            public int Iterations()
            {
                return 524288;
            }

            public int Variant()
            {
                return 0;
            }
        }
    }

    
    public class CNLiteV1 : IHashProvider
    {
        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNLiteV1Params());
        }

        private sealed class CNLiteV1Params : ICryptoNight
        {
            /* 2 ^ 20 */
            public int Memory()
            {
                return 1048576;
            }

            /* 2 ^ 19 */
            public int Iterations()
            {
                return 524288;
            }

            public int Variant()
            {
                return 1;
            }
        }
    }
}
