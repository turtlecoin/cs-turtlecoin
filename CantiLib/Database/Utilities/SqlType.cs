//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti
{
    /// <summary>
    /// SQL equivalent data type
    /// </summary>
    public enum SqlType
    {
        /// <summary>
        /// Fixed length string (requires size, 8,000 characters max)
        /// </summary>
        CHAR,

        /// <summary>
        /// Variable length string
        /// </summary>
        TEXT,

        /// <summary>
        /// Fixed length byte array (requires size, 8,000 bytes max)
        /// </summary>
        BINARY,

        /// <summary>
        /// Variable length byte array
        /// </summary>
        VARBINARY,

        /// <summary>
        /// Byte
        /// </summary>
        TINYINT,

        /// <summary>
        /// Short
        /// </summary>
        SMALLINT,

        /// <summary>
        /// Int
        /// </summary>
        INTEGER,

        /// <summary>
        /// Long
        /// </summary>
        BIGINT,

        /// <summary>
        /// Unix timestamp
        /// </summary>
        TIMESTAMP,

        /// <summary>
        /// Bool
        /// </summary>
        BOOLEAN
    }
}
