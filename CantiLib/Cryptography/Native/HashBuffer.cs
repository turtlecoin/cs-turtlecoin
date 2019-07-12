//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Cryptography.Native
{
    internal class HashBuffer
    {
        internal byte[] A;
        internal byte[] B;
        internal byte[] Output
        {
            get
            {
                return A.AppendBytes(B);
            }
        }
        internal HashBuffer()
        {
            A = new byte[32];
            B = new byte[32];
        }

        internal HashBuffer(byte[] Signature)
        {
            A = Signature.SubBytes(0, 32);
            B = Signature.SubBytes(32, 32);
        }
    };
}
