//
// Copyright 2012-2018 The CryptoNote Developers, The ByteCoin Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using static Canti.Cryptography.Native.CryptoNight.CryptoNight;

namespace Canti.Cryptography.Native.CryptoNight
{
    public abstract class ICryptoNight
    {
        /* Gets the scratchpad size */
        public abstract int Memory { get; }

        /* Gets the amount of iterations */
        public abstract int Iterations { get; }

        /* Gets the variant of CN to use - currently supported, 0 and 1 */
        public abstract int Variant { get; }

        /* Gets whether we should use intrinsics functions to speed up hashing */
        public abstract bool Intrinsics { get; }

        /* Just to avoid computing in loops */
        public int InitRounds
        {
            get => Memory / Constants.INIT_SIZE_BYTE;
        }

        /* Just to avoid computing in loops */
        public int AesRounds
        {
            get => Iterations / 2;
        }

        /* Determines how we index the scratchpad 'memory' address. If this is
           set to 2, then we will iterate less of the scratchpad. */
        public virtual int Light { get; } = 1;

        /* Determines what we '&' the 'memory' index with - to constrain where
           it falls in the scratchpad. */
        public int ScratchModulus
        {
            get => (Memory / AES.BLOCK_SIZE / Light) - 1;
        }
    }

    public class CNV0 : IHashProvider
    {
        public CNV0() : this(true)
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
            public CNV0Params() : this(true)
            {
            }

            public CNV0Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 21 */
            public override int Memory { get; } = 2097152;

            /* 2 ^ 20 */
            public override int Iterations { get; } = 1048576;

            public override int Variant { get; } = 0;

            public override bool Intrinsics { get; }
        }
    }

    public class CNV1 : IHashProvider
    {
        public CNV1() : this(true)
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
            public CNV1Params() : this(true)
            {
            }

            public CNV1Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 21 */
            public override int Memory { get; } = 2097152;

            /* 2 ^ 20 */
            public override int Iterations { get; } = 1048576;

            public override int Variant { get; } = 1;

            public override bool Intrinsics { get; }
        }
    }

    public class CNV2 : IHashProvider
    {
        public CNV2() : this(true)
        {
        }

        public CNV2(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNV2Params(UseIntrinsics));
        }

        private sealed class CNV2Params : ICryptoNight
        {
            public CNV2Params() : this(true)
            {
            }

            public CNV2Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 21 */
            public override int Memory { get; } = 2097152;

            /* 2 ^ 20 */
            public override int Iterations { get; } = 1048576;

            public override int Variant { get; } = 2;

            public override bool Intrinsics { get; }
        }
    }

    public class CNLiteV0 : IHashProvider
    {
        public CNLiteV0() : this(true)
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
            public CNLiteV0Params() : this(true)
            {
            }

            public CNLiteV0Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 20 */
            public override int Memory { get; } = 1048576;

            /* 2 ^ 19 */
            public override int Iterations { get; } = 524288;

            public override int Variant { get; } = 0;

            public override bool Intrinsics { get; }
        }
    }

    public class CNLiteV1 : IHashProvider
    {
        public CNLiteV1() : this(true)
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
            public CNLiteV1Params() : this(true)
            {
            }

            public CNLiteV1Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 20 */
            public override int Memory { get; } = 1048576;

            /* 2 ^ 19 */
            public override int Iterations { get; } = 524288;

            public override int Variant { get; } = 1;

            public override bool Intrinsics { get; }
        }
    }

    public class CNLiteV2 : IHashProvider
    {
        public CNLiteV2() : this(true)
        {
        }

        public CNLiteV2(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNLiteV2Params(UseIntrinsics));
        }

        private sealed class CNLiteV2Params : ICryptoNight
        {
            public CNLiteV2Params() : this(true)
            {
            }

            public CNLiteV2Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 20 */
            public override int Memory { get; } = 1048576;

            /* 2 ^ 19 */
            public override int Iterations { get; } = 524288;

            public override int Variant { get; } = 2;

            public override bool Intrinsics { get; }
        }
    }

    public class CNDarkV0 : IHashProvider
    {
        public CNDarkV0() : this(true)
        {
        }

        public CNDarkV0(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNDarkV0Params(UseIntrinsics));
        }

        private sealed class CNDarkV0Params : ICryptoNight
        {
            public CNDarkV0Params() : this(true)
            {
            }

            public CNDarkV0Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 21 */
            public override int Memory { get; } = 524288;

            /* 2 ^ 20 */
            public override int Iterations { get; } = 262144;

            public override int Variant { get; } = 0;

            public override bool Intrinsics { get; }
        }
    }

    public class CNDarkV1 : IHashProvider
    {
        public CNDarkV1() : this(true)
        {
        }

        public CNDarkV1(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNDarkV1Params(UseIntrinsics));
        }

        private sealed class CNDarkV1Params : ICryptoNight
        {
            public CNDarkV1Params() : this(true)
            {
            }

            public CNDarkV1Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 21 */
            public override int Memory { get; } = 524288;

            /* 2 ^ 20 */
            public override int Iterations { get; } = 262144;

            public override int Variant { get; } = 1;

            public override bool Intrinsics { get; }
        }
    }

    public class CNDarkV2 : IHashProvider
    {
        public CNDarkV2() : this(true)
        {
        }

        public CNDarkV2(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNDarkV2Params(UseIntrinsics));
        }

        private sealed class CNDarkV2Params : ICryptoNight
        {
            public CNDarkV2Params() : this(true)
            {
            }

            public CNDarkV2Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 21 */
            public override int Memory { get; } = 524288;

            /* 2 ^ 20 */
            public override int Iterations { get; } = 262144;

            public override int Variant { get; } = 2;

            public override bool Intrinsics { get; }
        }
    }

    public class CNDarkLiteV0 : IHashProvider
    {
        public CNDarkLiteV0() : this(true)
        {
        }

        public CNDarkLiteV0(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNDarkLiteV0Params(UseIntrinsics));
        }

        private sealed class CNDarkLiteV0Params : ICryptoNight
        {
            public CNDarkLiteV0Params() : this(true)
            {
            }

            public CNDarkLiteV0Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 20 */
            public override int Memory { get; } = 524288;

            /* 2 ^ 19 */
            public override int Iterations { get; } = 262144;

            public override int Variant { get; } = 0;

            public override bool Intrinsics { get; }

            public override int Light { get; } = 2;
        }
    }

    public class CNDarkLiteV1 : IHashProvider
    {
        public CNDarkLiteV1() : this(true)
        {
        }

        public CNDarkLiteV1(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNDarkLiteV1Params(UseIntrinsics));
        }

        private sealed class CNDarkLiteV1Params : ICryptoNight
        {
            public CNDarkLiteV1Params() : this(true)
            {
            }

            public CNDarkLiteV1Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 20 */
            public override int Memory { get; } = 524288;

            /* 2 ^ 19 */
            public override int Iterations { get; } = 262144;

            public override int Variant { get; } = 1;

            public override bool Intrinsics { get; }

            public override int Light { get; } = 2;
        }
    }

    public class CNDarkLiteV2 : IHashProvider
    {
        public CNDarkLiteV2() : this(true)
        {
        }

        public CNDarkLiteV2(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNDarkLiteV2Params(UseIntrinsics));
        }

        private sealed class CNDarkLiteV2Params : ICryptoNight
        {
            public CNDarkLiteV2Params() : this(true)
            {
            }

            public CNDarkLiteV2Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 20 */
            public override int Memory { get; } = 524288;

            /* 2 ^ 19 */
            public override int Iterations { get; } = 262144;

            public override int Variant { get; } = 2;

            public override bool Intrinsics { get; }

            public override int Light { get; } = 2;
        }
    }

    public class CNTurtleV0 : IHashProvider
    {
        public CNTurtleV0() : this(true)
        {
        }

        public CNTurtleV0(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNTurtleV0Params(UseIntrinsics));
        }

        private sealed class CNTurtleV0Params : ICryptoNight
        {
            public CNTurtleV0Params() : this(true)
            {
            }

            public CNTurtleV0Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 18 */
            public override int Memory { get; } = 262144;

            /* 2 ^ 17 */
            public override int Iterations { get; } = 131072;

            public override int Variant { get; } = 0;

            public override bool Intrinsics { get; }
        }
    }

    public class CNTurtleV1 : IHashProvider
    {
        public CNTurtleV1() : this(true)
        {
        }

        public CNTurtleV1(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNTurtleV1Params(UseIntrinsics));
        }

        private sealed class CNTurtleV1Params : ICryptoNight
        {
            public CNTurtleV1Params() : this(true)
            {
            }

            public CNTurtleV1Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 18 */
            public override int Memory { get; } = 262144;

            /* 2 ^ 17 */
            public override int Iterations { get; } = 131072;

            public override int Variant { get; } = 1;

            public override bool Intrinsics { get; }
        }
    }

    public class CNTurtleV2 : IHashProvider
    {
        public CNTurtleV2(): this(true)
        {
        }

        public CNTurtleV2(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNTurtleV2Params(UseIntrinsics));
        }

        private sealed class CNTurtleV2Params : ICryptoNight
        {
            public CNTurtleV2Params(): this(true)
            {
            }

            public CNTurtleV2Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 18 */
            public override int Memory { get; } = 262144;

            /* 2 ^ 17 */
            public override int Iterations { get; } = 131072;

            public override int Variant { get; } = 2;

            public override bool Intrinsics { get; }
        }
    }

    public class CNTurtleLiteV0 : IHashProvider
    {
        public CNTurtleLiteV0() : this(true)
        {
        }

        public CNTurtleLiteV0(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNTurtleLiteV0Params(UseIntrinsics));
        }

        private sealed class CNTurtleLiteV0Params : ICryptoNight
        {
            public CNTurtleLiteV0Params() : this(true)
            {
            }

            public CNTurtleLiteV0Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 18 */
            public override int Memory { get; } = 262144;

            /* 2 ^ 17 */
            public override int Iterations { get; } = 131072;

            public override int Variant { get; } = 0;

            public override bool Intrinsics { get; }

            public override int Light { get; } = 2;
        }
    }

    public class CNTurtleLiteV1 : IHashProvider
    {
        public CNTurtleLiteV1() : this(true)
        {
        }

        public CNTurtleLiteV1(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNTurtleLiteV1Params(UseIntrinsics));
        }

        private sealed class CNTurtleLiteV1Params : ICryptoNight
        {
            public CNTurtleLiteV1Params() : this(true)
            {
            }

            public CNTurtleLiteV1Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 18 */
            public override int Memory { get; } = 262144;

            /* 2 ^ 17 */
            public override int Iterations { get; } = 131072;

            public override int Variant { get; } = 1;

            public override bool Intrinsics { get; }

            public override int Light { get; } = 2;
        }
    }

    public class CNTurtleLiteV2 : IHashProvider
    {
        public CNTurtleLiteV2() : this(true)
        {
        }

        public CNTurtleLiteV2(bool useIntrinsics) => UseIntrinsics = useIntrinsics;

        private bool UseIntrinsics;

        public byte[] Hash(byte[] input)
        {
            return SlowHash(input, new CNTurtleLiteV2Params(UseIntrinsics));
        }

        private sealed class CNTurtleLiteV2Params : ICryptoNight
        {
            public CNTurtleLiteV2Params() : this(true)
            {
            }

            public CNTurtleLiteV2Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 18 */
            public override int Memory { get; } = 262144;

            /* 2 ^ 17 */
            public override int Iterations { get; } = 131072;

            public override int Variant { get; } = 2;

            public override bool Intrinsics { get; }

            public override int Light { get; } = 2;
        }
    }
}
