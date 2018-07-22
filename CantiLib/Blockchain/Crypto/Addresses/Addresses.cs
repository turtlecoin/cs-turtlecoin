//
// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Linq;
using System.Collections.Generic;

using Canti.Data;
using Canti.Blockchain;
using Canti.Blockchain.Crypto;

using static Canti.Blockchain.Crypto.Keccak.Keccak;

namespace Canti.Blockchain.Crypto.Addresses
{
    internal static class Addresses
    {
        public static string AddressFromPrivateKeys(PrivateKey privateSpendKey,
                                                    PrivateKey privateViewKey)
        {
            PublicKey publicSpendKey = KeyOps.PrivateKeyToPublicKey(privateSpendKey);
            PublicKey publicViewKey = KeyOps.PrivateKeyToPublicKey(privateViewKey);

            return AddressFromPublicKeys(publicSpendKey, publicViewKey);
        }

        public static string AddressFromPublicKeys(PublicKey publicSpendKey,
                                                   PublicKey publicViewKey)
        {
            List<byte> combined = new List<byte>();

            byte[] prefix = Encoding.IntegerToByteArray<ulong>(Globals.addressPrefix);

            /* Concat the prefix, public spend key and private spend key */
            combined.AddRange(prefix);
            combined.AddRange(publicSpendKey.data);
            combined.AddRange(publicViewKey.data);

            /* Hash the combined data with keccak and take the first 4 bytes */
            byte[] checksum = keccak(combined.ToArray(), 4);

            /* Take the first 4 bytes of this hash and add it to the end of
               the hash as a checksum */
            combined.AddRange(checksum);

            Base58 b = new Base58();
            return b.Encode(combined.ToArray());
        }
    }
}
