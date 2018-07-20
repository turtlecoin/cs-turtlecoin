namespace Canti.Blockchain.Crypto
{
    internal class KeyPair
    {
        public KeyPair(PublicKey publicKey, PrivateKey privateKey)
        {
            this.privateKey = privateKey;
            this.publicKey = publicKey;
        }

        public PrivateKey privateKey;
        public PublicKey publicKey;
    }

    internal class ThirtyTwoByteKey
    {
        public ThirtyTwoByteKey(byte[] data)
        {
            /* We just copy the first 32 bytes. This allows us to take in a
               byte[] of any length, without having to resize first. A loop
               copy is apparently faster than Array.Copy() or
               Buffer.BlockCopy() when the length is of this size */
            for (int i = 0; i < 32; i++)
            {
                this.data[i] = data[i];
            }
        }

        public byte[] data = new byte[32];
    }

    internal class EllipticCurvePoint : ThirtyTwoByteKey
    {
        public EllipticCurvePoint(byte[] data) : base(data) {}
    }

    internal class EllipticCurveScalar : ThirtyTwoByteKey
    {
        public EllipticCurveScalar(byte[] data) : base(data) {}
    }

    internal class PrivateKey : ThirtyTwoByteKey
    {
        public PrivateKey(byte[] data) : base(data) {}
    }

    internal class PublicKey : ThirtyTwoByteKey
    {
        public PublicKey(byte[] data) : base(data) {}
    }
}
