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

        /* Gets whether we should use intrinsics functions to speed up hashing */
        bool Intrinsics();
    }

    public class CNV0 : IHashProvider
    {
        public CNV0(): this(true)
        {
        }

        public CNV0(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNV0Params(UseIntrinsics));
        }

        private sealed class CNV0Params : ICryptoNight
        {
            public CNV0Params(): this(true)
            {
            }

            public CNV0Params(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

            private bool UseIntrinsics;

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

            public bool Intrinsics()
            {
                return UseIntrinsics;
            }
        }
    }

    public class CNV1 : IHashProvider
    {
        public CNV1(): this(true)
        {
        }

        public CNV1(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNV1Params(UseIntrinsics));
        }

        private sealed class CNV1Params : ICryptoNight
        {
            public CNV1Params(): this(true)
            {
            }

            public CNV1Params(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

            private bool UseIntrinsics;

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

            public bool Intrinsics()
            {
                return UseIntrinsics;
            }
        }
    }

    public class CNLiteV0 : IHashProvider
    {
        public CNLiteV0(): this(true)
        {
        }

        public CNLiteV0(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNLiteV0Params(UseIntrinsics));
        }

        private sealed class CNLiteV0Params : ICryptoNight
        {
            public CNLiteV0Params(): this(true)
            {
            }

            public CNLiteV0Params(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

            private bool UseIntrinsics;

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

            public bool Intrinsics()
            {
                return UseIntrinsics;
            }
        }
    }

    
    public class CNLiteV1 : IHashProvider
    {
        public CNLiteV1(): this(true)
        {
        }

        public CNLiteV1(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNLiteV1Params(UseIntrinsics));
        }

        private sealed class CNLiteV1Params : ICryptoNight
        {
            public CNLiteV1Params(): this(true)
            {
            }

            public CNLiteV1Params(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

            private bool UseIntrinsics;

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

            public bool Intrinsics()
            {
                return UseIntrinsics;
            }
        }
    }
}
