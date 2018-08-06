//
// Copyright (c) The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain.Crypto
{
    // Interface for cryptography implementations
    public interface IHashProvider
    {
        byte[] Hash(byte[] input);
    }
}
