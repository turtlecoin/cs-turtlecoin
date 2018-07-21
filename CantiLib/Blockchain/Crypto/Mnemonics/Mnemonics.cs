//
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Linq;

namespace Canti.Blockchain.Crypto.Mnemonics
{
    internal static class Mnemonics
    {
        /* Get the checksum word given the 24 input words */
        public static string GetChecksumWord(string[] words)
        {
            string trimmed = "";

            /* Take the first 3 chars from each of the 24 words */
            foreach (string word in words)
            {
                trimmed += word.Substring(0, 3);
            }

            uint checksum = CRC32.crc32(trimmed);

            return words[checksum % words.Length];
        }

        private static bool HasValidChecksum(string[] words)
        {
            var wordsNoChecksum = words.Take(words.Length - 1);

            return words.Last() == GetChecksumWord(wordsNoChecksum.ToArray());
        }
    }
}
