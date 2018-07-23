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

        /* Thanks to - https://stackoverflow.com/a/24087164/8737306 */
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize) 
        {
            return source
                  .Select((x, i) => new { Index = i, Value = x })
                  .GroupBy(x => x.Index / chunkSize)
                  .Select(x => x.Select(v => v.Value).ToList())
                  .ToList();
        }
    }
}
