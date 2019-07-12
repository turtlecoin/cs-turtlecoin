//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Cryptography.Native;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    public class Addresses
    {
        private const char LeadingChar = '1';

        // Converts a set of public keys to a readable address
        public static string AddressFromKeys(string PublicSpendKey, string PublicViewKey, ulong Prefix)
        {
            // First we create a byte buffer
            byte[] Buffer = new byte[0];

            // Pack address prefix into buffer
            Buffer = Buffer.AppendVarInt(Prefix);

            // Add keys to buffer
            Buffer = Buffer.AppendBytes(HexStringToByteArray(PublicSpendKey));
            Buffer = Buffer.AppendBytes(HexStringToByteArray(PublicViewKey));

            // Append buffer checksum to buffer
            Buffer = Buffer.AppendBytes(Keccak.KeccakHash(Buffer, 4));

            /* Now we have to encode our address in base58 - but we do it in
               a bit of an odd way, in blocks. We take 8 bytes at a time,
               and convert it to base58. Then, if the block converts to less
               than 11 base58 chars, we pad it with ones (1 is 0 in Base58)
               
               The final block however is not padded to 11 chars, rather it
               is padded to the length of the hex string modulo 8.
               
               This ensures addresses are always the same length. */

            // Split buffer into chunks of 8
            var Chunks = Buffer.ChunkBy(8);

            // Create an output string for our address
            string Output = "";

            // The size of the remainder after chunking into groups of 8
            int LastChunkSize = Buffer.Length % 8;

            // Loop through all chunks
            for (int i = 0; i < Chunks.Count; i++)
            {
                // Encode the chunk into a base58 string
                string AddrChunk = Base58.Encode(Chunks[i]);

                // Pad to 11 chars with ones, i.e. 0 in base58 - we pad at the beginning
                if (i < Chunks.Count - 1)
                {
                    AddrChunk = AddrChunk.PadLeft(11, LeadingChar);
                }

                // Last iteration, we do different padding
                else
                {
                    AddrChunk = AddrChunk.PadLeft(LastChunkSize, LeadingChar);
                }

                // Add address chunk to output string
                Output += AddrChunk;
            }

            // Return resulting address string
            return Output;
        }
    }
}
