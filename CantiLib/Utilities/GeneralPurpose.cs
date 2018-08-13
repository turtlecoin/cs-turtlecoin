//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Linq;
using System.Collections.Generic;

namespace Canti.Utilities
{
    public static class GeneralUtilities
    {
        // Returns the current unix timestamp
        public static ulong GetTimestamp()
        {
            return (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        /* Thanks to - https://stackoverflow.com/a/24087164/8737306

           Splits a list of type <T> into a List of Lists of type <T>, with
           chunks of size chunkSize

           E.g. you can use this to split a list of bytes into lists of bytes
           where each list is 8 bytes long */
        public static List<List<T>> ChunkBy<T>(this List<T> source,
                                               int chunkSize) 
        {
            return source
                  .Select((x, i) => new { Index = i, Value = x })
                  .GroupBy(x => x.Index / chunkSize)
                  .Select(x => x.Select(v => v.Value).ToList())
                  .ToList();
        }

        /* Thanks to - https://stackoverflow.com/a/38596841/8737306

           Takes the last N elements from an IEnumerable/List */
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source,
                                                 int numElements)
        {
            return source.Skip(Math.Max(0, source.Count() - numElements));
        }

        /* Thanks to - https://stackoverflow.com/a/50552122/8737306
        
           Deconstructs a dictionary into a tuple as you go, which is helpful
           for nicer syntax when iterating over a dictionary, rather than
           having to call entry.Key, entry.Value */
        public static IEnumerable<(TKey, TValue)> Tuples<TKey, TValue>(
            this IDictionary<TKey, TValue> dict)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in dict)
            {
                yield return (kvp.Key, kvp.Value);
            }
        }

        public static bool IsHex(string str)
        {
            return str.All(c => IsHex(c));
        }

        /* Must be 0..9, or A..F, or a..f - order of chars goes like so -
           0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`
           abcdefghijklmnopqrstuvwxyz */
        /* TODO: Do this in a better way */
        public static bool IsHex(char c)
        {
            if (c < '0')
            {
                return false;
            }

            if (c > '9' && c < 'A')
            {
                return false;
            }

            if (c > 'F' && c < 'a')
            {
                return false;
            }

            if (c > 'f')
            {
                return false;
            }

            return true;
        }
    }
}
