//
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using Canti.Cryptography.Native.SkeinFish;

/* This is post NIST skein, with a 512 bit state and a 256 bit output */

/* You can find test vectors at page 81 of this PDF:
   http://www.skein-hash.info/sites/default/files/skein1.3.pdf - however,
   these are only for 256-256, 512-512, and 1024-1024 - They can still be used
   to verify the implementation if you're providing a comprehensive skein API
*/
   
/* There are some correct test vectors for 512-256 on the wikipedia page for
   skein:
   https://en.wikipedia.org/wiki/Skein_(hash_function)#Examples_of_Skein_hashes
*/

namespace Canti.Cryptography.Native
{
    public class Skein : IHashProvider
    {
        public byte[] Hash(byte[] input)
        {
            Skein512_256 s = new Skein512_256();
            return s.ComputeHash(input);
        }
    }
}
