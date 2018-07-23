//
// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;

using Canti.Utilities;

using static Canti.Blockchain.Crypto.Keccak.Keccak;

namespace Canti.Blockchain.Crypto
{
    internal static class Addresses
    {
        public static string AddressFromKeys(PrivateKey privateSpendKey,
                                             PrivateKey privateViewKey)
        {
            PublicKey publicSpendKey = KeyOps.PrivateKeyToPublicKey(privateSpendKey);
            PublicKey publicViewKey = KeyOps.PrivateKeyToPublicKey(privateViewKey);

            return AddressFromKeys(publicSpendKey, publicViewKey);
        }

        public static string AddressFromKeys(PublicKey publicSpendKey,
                                             PublicKey publicViewKey)
        {
            /* Specify the address prefix in the input for easier testing */
            return AddressFromKeys(publicSpendKey, publicViewKey,
                                   Globals.addressPrefix);
        }

        public static string AddressFromKeys(PublicKey publicSpendKey,
                                             PublicKey publicViewKey,
                                             ulong addressPrefix)
        {
            List<byte> combined = new List<byte>();

            /* Concat the prefix, public spend key and private spend key */
            combined.AddRange(PackPrefixAsByteList(addressPrefix));
            combined.AddRange(publicSpendKey.data);
            combined.AddRange(publicViewKey.data);

            /* Hash the combined data with keccak and take the first 4 bytes */
            byte[] checksum = keccak(combined.ToArray(), 4);

            /* Take the first 4 bytes of this hash and add it to the end of
               the hash as a checksum */
            combined.AddRange(checksum);

            /* Now we have to encode our address in base58 - but we do it in
               a bit of an odd way, in blocks. We take 8 bytes at a time,
               and convert it to base58. Then, if the block converts to less
               than 11 base58 chars, we pad it with ones (1 is 0 in Base58)
               
               The final block however is not padded to 11 chars, rather it
               is padded to the length of the hex string modulo 8.
               
               This ensures addresses are always the same length. */

            /* Split into separate chunks of 8 */
            var chunks = combined.ChunkBy(8);

            /* Where the address result will go */
            string address = "";

            /* The length to pad the last block to */
            int lastBlockSize = combined.Count % 8;

            Base58 b = new Base58();

            for (int i = 0; i < chunks.Count; i++)
            {
                string tmp = b.Encode(chunks[i].ToArray());

                if (i < chunks.Count - 1)
                {
                    /* Pad to 11 chars with ones, i.e. 0 in base58 */
                    tmp = tmp.PadRight(11, '1');
                }
                /* Last iteration, we do different padding */
                else
                {
                    tmp = tmp.PadRight(lastBlockSize, '1');
                }

                address += tmp;
            }

            return address;
        }

        /* Pack our prefix into as few bytes as possible */
        private static List<byte> PackPrefixAsByteList(ulong prefix)
        {
            List<byte> output = new List<byte>();

            while (prefix >= 0x80)
            {
                output.Add((byte)(prefix & 0x7f | 0x80));
                prefix >>= 7;
            }

            output.Add((byte)prefix);

            return output;
        }
    }
}
