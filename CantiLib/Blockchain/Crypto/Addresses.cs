//
// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Linq;
using System.Collections.Generic;

using Canti.Errors;
using Canti.Utilities;

using static Canti.Blockchain.Crypto.Keccak.Keccak;

namespace Canti.Blockchain.Crypto
{
    public static class Addresses
    {
        public static string AddressFromKeys(PrivateKey privateSpendKey,
                                             PrivateKey privateViewKey)
        {
            PublicKey publicSpendKey = KeyOps.PrivateKeyToPublicKey(
                privateSpendKey
            );

            PublicKey publicViewKey = KeyOps.PrivateKeyToPublicKey(
                privateViewKey
            );

            return AddressFromKeys(publicSpendKey, publicViewKey);
        }

        public static string AddressFromKeys(PublicKey publicSpendKey,
                                             PublicKey publicViewKey)
        {
            /* Specify the address prefix in the input for easier testing */
            return AddressFromKeys(publicSpendKey, publicViewKey,
                                   Globals.addressPrefix);
        }

        public static string AddressFromKeys(PrivateKey privateSpendKey,
                                             PrivateKey privateViewKey,
                                             ulong addressPrefix)
        {
            PublicKey publicSpendKey = KeyOps.PrivateKeyToPublicKey(
                privateSpendKey
            );

            PublicKey publicViewKey = KeyOps.PrivateKeyToPublicKey(
                privateViewKey
            );

            return AddressFromKeys(publicSpendKey, publicViewKey,
                                   addressPrefix);
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

            /* Get the checksum of the previous data */
            byte[] checksum = GetAddressChecksum(combined);

            /* Append it to the address bytes */ 
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

            for (int i = 0; i < chunks.Count; i++)
            {
                string tmp = Base58.Encode(chunks[i].ToArray());

                if (i < chunks.Count - 1)
                {
                    /* Pad to 11 chars with ones, i.e. 0 in base58 - we pad
                       at the beginning */
                    tmp = tmp.PadLeft(11, '1');
                }
                /* Last iteration, we do different padding */
                else
                {
                    tmp = tmp.PadLeft(lastBlockSize, '1');
                }

                address += tmp;
            }

            return address;
        }

        public static IEither<Error, PublicKeys>
                      KeysFromAddress(string address)
        {
            return KeysFromAddress(address, Globals.addressPrefix);
        }

        public static IEither<Error, PublicKeys>
                      KeysFromAddress(string address, ulong prefix)
        {
            /* Split into chunks of 11 */
            List<List<char>> chunks = address.ToList().ChunkBy(11);

            List<byte> decoded = new List<byte>();

            foreach (List<char> chunk in chunks)
            {
                List<byte> decodedChunk;

                try
                {
                    /* Convert char list to string, and decode from base58 */
                    decodedChunk = new List<byte>(
                        Base58.Decode(string.Concat(chunk))
                    );
                }
                catch (FormatException)
                {
                    return Either.Left<Error, PublicKeys>(
                        Error.AddressNotBase58()
                    );
                }

                /* Only take last 8 bytes, any more in a chunk are padding
                   from the base58 convert. Remember we pad the beginning
                   of a chunk, so we need to take the last 8 bytes, not the
                   first 8 bytes. */
                decoded.AddRange(decodedChunk.TakeLast(8));
            }

            byte[] expectedPrefix = PackPrefixAsByteList(prefix).ToArray();

            /* Prefix length + spend key length + view key length 
                             + checksum length */
            int expectedLength = expectedPrefix.Length + 32 + 32 + 4;

            if (decoded.Count != expectedLength)
            {
                return Either.Left<Error, PublicKeys>(
                    Error.AddressWrongLength()
                );
            }

            byte[] actualPrefix = new byte[expectedPrefix.Length];
            byte[] spendKeyData = new byte[32];
            byte[] viewKeyData = new byte[32];
            byte[] actualChecksum = new byte[4];

            int i = 0;

            for (int j = 0; j < expectedPrefix.Length; j++)
            {
                actualPrefix[j] = decoded[i++];
            }

            for (int j = 0; j < spendKeyData.Length; j++)
            {
                spendKeyData[j] = decoded[i++];
            }

            for (int j = 0; j < viewKeyData.Length; j++)
            {
                viewKeyData[j] = decoded[i++];
            }

            for (int j = 0; j < actualChecksum.Length; j++)
            {
                actualChecksum[j] = decoded[i++];
            }

            /* Sanity checking */
            for (int j = 0; j < expectedPrefix.Length; j++)
            {
                /* We already know the lengths are the same so won't go out
                   of bounds */
                if (actualPrefix[j] != expectedPrefix[j])
                {
                    return Either.Left<Error, PublicKeys>(
                        Error.AddressWrongPrefix()
                    );
                }
            }

            List<byte> addressNoChecksum = new List<byte>();

            addressNoChecksum.AddRange(actualPrefix);
            addressNoChecksum.AddRange(spendKeyData);
            addressNoChecksum.AddRange(viewKeyData);

            byte[] expectedChecksum = GetAddressChecksum(addressNoChecksum);

            for (int j = 0; j < expectedChecksum.Length; j++)
            {
                if (actualChecksum[j] != expectedChecksum[j])
                {
                    return Either.Left<Error, PublicKeys>(
                        Error.AddressWrongChecksum()
                    );
                }
            }

            var spendKey = new PublicKey(spendKeyData);
            var viewKey = new PublicKey(viewKeyData);

            if (!KeyOps.IsValidKey(spendKey) || !KeyOps.IsValidKey(viewKey))
            {
                return Either.Left<Error, PublicKeys>(
                    Error.InvalidPublicKey()
                );
            }

            return Either.Right<Error, PublicKeys>(
                new PublicKeys(spendKey, viewKey)
            );
        }

        private static byte[] GetAddressChecksum(List<byte> addressInBytes)
        {
            return keccak(addressInBytes.ToArray(), 4);
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
