//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti
{
    /// <summary>
    /// Provides a context that an API server can invoke methods from
    /// </summary>
    public interface IMethodContext
    {
        /// <summary>
        /// Checks that a received request's version is valid
        /// </summary>
        /// <param name="Version">The request's version</param>
        /// <returns>True if version is valid</returns>
        bool CheckVersion(int Version);
    }
}
