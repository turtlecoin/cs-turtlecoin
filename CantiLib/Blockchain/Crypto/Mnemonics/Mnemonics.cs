//
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Linq;
using System.Collections.Generic;

using Canti.Data;

namespace Canti.Blockchain.Crypto.Mnemonics
{
    public static class Mnemonics
    {
        public static PrivateKey MnemonicToPrivateKey(string words)
        {
            return MnemonicToPrivateKey(words.Split(' '));
        }

        /* This function assumes you have called IsValidMnemonicFirst. If
           you haven't, it is liable to blow up. */
        public static PrivateKey MnemonicToPrivateKey(string[] words)
        {
            /* The private key will go here */
            List<byte> data = new List<byte>();

            /* For each word in the input, get the index of that word in the
               word list */
            var wordIndexes = words.Select(
                word => Array.FindIndex(WordList.English, x => x == word)
            ).ToArray();

            /* We process 3 words at a time */
            for (int i = 0; i < words.Length - 1; i += 3)
            {
                /* Take the indexes of these three words in the word list */
                uint w1 = (uint)(wordIndexes[i]);
                uint w2 = (uint)(wordIndexes[i + 1]);
                uint w3 = (uint)(wordIndexes[i + 2]);

                /* Word list length */
                uint wlLen = (uint)(WordList.English.Length);

                uint val = w1 + wlLen * (((wlLen - w1) + w2) % wlLen) + wlLen 
                                      * wlLen * (((wlLen - w2) + w3) % wlLen);

                if (!(val % wlLen == w1))
                {
                    throw new ArgumentException("Invalid mnemonic");
                }

                /* Convert uint to byte array and append to output private
                   key */
                data.AddRange(Encoding.IntegerToByteArray<uint>(val));
            }

            /* And return our new private key */
            return new PrivateKey(data.ToArray());
        }

        public static string PrivateKeyToMnemonic(PrivateKey privateKey)
        {
            /* Where we'll put the output words into */
            List<string> words = new List<string>();

            /* For less typing */
            byte[] data = privateKey.data;

            /* Take chunks 4 at a time */
            for (int i = 0; i < privateKey.data.Length - 1; i += 4)
            {
                /* Read 4 bytes from the byte[] as an integer */
                uint val = Encoding.ByteArrayToInteger<uint>(data, i, 4);

                uint wlLen = (uint)(WordList.English.Length);

                uint w1 = val % wlLen;
                uint w2 = ((val / wlLen) + w1) % wlLen;
                uint w3 = (((val / wlLen) / wlLen) + w2) % wlLen;

                words.Add(WordList.English[w1]);
                words.Add(WordList.English[w2]);
                words.Add(WordList.English[w3]);
            }

            /* Add the checksum */
            words.Add(GetChecksumWord(words.ToArray()));

            /* Convert into combined string */
            return string.Join(" ", words.ToArray());
        }

        /* Get the checksum word given the 24 input words */
        private static string GetChecksumWord(string[] words)
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

        public static (bool valid, string reason) IsValidMnemonic(string words)
        {
            return IsValidMnemonic(words.Split(' '));
        }

        public static (bool valid, string reason) IsValidMnemonic(string[] words)
        {
            string err = "";

            /* Mnemonics must be 25 words long */
            if (words.Length != 25)
            {
                err = "Mnemonic seed is wrong length - It should be 25 words " +
                     $"long, but it is {words.Length} words long!";

                return (false, err);
            }
            
            /* All words must be present in the word list */
            foreach (string word in words)
            {
                if (!WordList.English.Contains(word))
                {
                    err = $"Mnemonic seed has invalid word - {word} is not " +
                           "in the English word list!";

                    return (false, err);
                }
            }

            /* The checksum must be correct */
            if (!HasValidChecksum(words))
            {
                err = "Mnemonic seed has incorrect checksum!";

                return (false, err);
            }

            try
            {
                MnemonicToPrivateKey(words);
            }
            catch (ArgumentException)
            {
                err = "Mnenonic seed is invalid!";

                return (false, err);
            }

            return (true, err);
        }
    }
}
