//
// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using Canti.Data;

namespace Canti.Blockchain.Crypto
{
    public class WalletKeys
    {
        public WalletKeys(KeyPair spendKeys, KeyPair viewKeys)
        {
            this.spendKeys = spendKeys;
            this.viewKeys = viewKeys;
        }

        public WalletKeys(PublicKey publicSpendKey, PrivateKey privateSpendKey,
                          PublicKey publicViewKey, PrivateKey privateViewKey)
        {
            this.spendKeys = new KeyPair(publicSpendKey, privateSpendKey);
            this.viewKeys = new KeyPair(publicViewKey, privateViewKey);
        }

        public KeyPair spendKeys;
        public KeyPair viewKeys;
    }

    public class KeyPair
    {
        public KeyPair(PublicKey publicKey, PrivateKey privateKey)
        {
            this.privateKey = privateKey;
            this.publicKey = publicKey;
        }

        public PrivateKey privateKey;
        public PublicKey publicKey;
    }

    public class ThirtyTwoByteKey
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

        public override string ToString()
        {
            return Encoding.ByteArrayToHexString(data);
        }

        public byte[] data = new byte[32];
    }

    public class EllipticCurvePoint : ThirtyTwoByteKey
    {
        public EllipticCurvePoint(byte[] data) : base(data) {}
    }

    public class EllipticCurveScalar : ThirtyTwoByteKey
    {
        public EllipticCurveScalar(byte[] data) : base(data) {}
    }

    public class PrivateKey : ThirtyTwoByteKey
    {
        public PrivateKey(byte[] data) : base(data) {}
    }

    public class PublicKey : ThirtyTwoByteKey
    {
        public PublicKey(byte[] data) : base(data) {}
    }
}
