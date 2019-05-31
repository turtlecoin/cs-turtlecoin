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
        INT,

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

    // For parsing object types to sql types
    public static class SqlHelper
    {
        // TODO - is this used?
        public static SqlType GetType(dynamic Value)
        {
            // Get object type
            var Type = Value.GetType();

            // Integer type
            if (Type == typeof(byte) || Type == typeof(sbyte)) return SqlType.TINYINT;
            else if (Type == typeof(short) || Type == typeof(ushort)) return SqlType.SMALLINT;
            else if (Type == typeof(int) || Type == typeof(uint)) return SqlType.INT;
            else if (Type == typeof(long) || Type == typeof(ulong)) return SqlType.BIGINT;
            else if (Type == typeof(string)) return SqlType.TEXT;
            else if (Type == typeof(byte[])) return SqlType.VARBINARY;
            else throw new ArgumentOutOfRangeException($"Could not parse SQL serialization type for object {Value}");
        }
    }
}
