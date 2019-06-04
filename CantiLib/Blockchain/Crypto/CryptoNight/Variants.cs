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
            get => Memory / Constants.InitSizeByte;
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
            get => (Memory / AES.Constants.BlockSize / Light) - 1;
        }
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

            public CNV1Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 21 */
            public override int Memory { get; } = 2097152;

            /* 2 ^ 20 */
            public override int Iterations { get; } = 1048576;

            public override int Variant { get; } = 1;

            public override bool Intrinsics { get; }
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

            public CNLiteV1Params(bool useIntrinsics) => Intrinsics = useIntrinsics;

            /* 2 ^ 20 */
            public override int Memory { get; } = 1048576;

            /* 2 ^ 19 */
            public override int Iterations { get; } = 524288;

            public override int Variant { get; } = 1;

            public override bool Intrinsics { get; }
        }
    }

    public class CNV2 : IHashProvider
    {
        public CNV2(): this(true)
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
            public CNV2Params(): this(true)
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
            
            public override int Light { get; } = 2;
        }
    }
}
