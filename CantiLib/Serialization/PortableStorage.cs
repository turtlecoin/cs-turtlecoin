//
// Copyright (c) 2006-2013, Andrey N. Sabelnikov, www.sabelnikov.net
// Copyright (c) 2012-2017, The CryptoNote developers, The Bytecoin developers
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using static Canti.Utils;

namespace Canti
{
    /// <summary>
    /// A key-value storage solution for encoding and decoding a set of objects as raw bytes
    /// </summary>
    [Serializable]
    public sealed class PortableStorage : IEnumerable
    {
        #region Constants

        // These constants are checked for when verifying a blob of data upon deserialization
        private static readonly byte[] STORAGE_SIGNATURE_A = { 0x01, 0x11, 0x01, 0x01 };
        private static readonly byte[] STORAGE_SIGNATURE_B = { 0x01, 0x01, 0x02, 0x01 };
        private static readonly byte[] STORAGE_VERSION = { 0x01 };

        // Max size values
        private const int MAX_STRING_LENGTH = 1_048_576;

        // Serialization types
        private enum SerializationType
        {
            INVALID = 0,
            LONG = 1,
            INT = 2,
            SHORT = 3,
            SBYTE = 4,
            ULONG = 5,
            UINT = 6,
            USHORT = 7,
            BYTE = 8,
            DOUBLE = 9,
            STRING = 10,
            BOOL = 11,
            OBJECT = 12,
            OBJECTARRAY = 13,
            BYTEARRAY = 14
        }

        #endregion

        #region Properties and Fields

        #region Public

        /// <summary>
        /// Accesses a stored value
        /// </summary>
        /// <param name="Key">The name of the stored value</param>
        /// <returns>If the given key is present, returns the value being stored, otherwise returns null</returns>
        public dynamic this[string Key]
        {
            get
            {
                lock (Entries)
                {
                    if (Entries.ContainsKey(Key))
                    {
                        return Entries[Key];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                lock (Entries)
                {
                    if (Entries.ContainsKey(Key))
                    {
                        Entries[Key] = value;
                    }
                    else
                    {
                        Entries.Add(Key, value);
                    }
                }
            }
        }

        /// <summary>
        /// The amount of value entries stored
        /// </summary>
        public int Count
        {
            get
            {
                return Entries.Count;
            }
        }

        #endregion

        #region Private

        // Dictionary of storage entries
        private Dictionary<string, dynamic> Entries { get; set; }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Converts a Dictionary object to a PortableStorage object
        /// </summary>
        /// <param name="Value">The Dictionary object to convert</param>
        public static implicit operator PortableStorage(Dictionary<string, dynamic>Value)
        {
            PortableStorage Output = new PortableStorage();
            foreach (var Entry in Value)
            {
                Output[Entry.Key] = Entry.Value;
            }
            return Output;
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Serializes this storage object into a byte array
        /// </summary>
        /// <param name="IncludeHeader">Whether or not to include the storage header in the output</param>
        /// <returns>A serialized byte array representation of all stored value entries</returns>
        public byte[] Serialize(bool IncludeHeader = true)
        {
            // Lock our dictionary to prevent race conditions
            lock (Entries)
            {
                // Begin by creating an empty output buffer
                byte[] Output = new byte[0];

                // Write header to output buffer if specified
                if (IncludeHeader)
                {
                    // Add signatures and version
                    Output = Output.AppendBytes(STORAGE_SIGNATURE_A);
                    Output = Output.AppendBytes(STORAGE_SIGNATURE_B);
                    Output = Output.AppendBytes(STORAGE_VERSION);
                }

                // Add the number of entries our entry dictionary contains as a varint
                Output = Output.AppendBytes(PackP2pVarInt(Entries.Count));

                // Iterate over and serialize each entry into our output buffer
                foreach (var Entry in Entries)
                {
                    // Serialize entry object
                    byte[] ObjectBytes = SerializeObject(Entry);

                    // Append to output buffer
                    Output = Output.AppendBytes(ObjectBytes);
                }

                // Return output array
                return Output;
            }
        }

        /// <summary>
        /// Gets the entry enumerator, allowing for foreach statements
        /// </summary>
        /// <returns>An enumerator object</returns>
        public IEnumerator GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        #endregion

        #region Private

        // Serializes an object to a byte array
        private static byte[] SerializeObject(dynamic Entry)
        {
            // Append entry name
            byte[] Output = new[] { (byte)Entry.Key.Length };
            Output = Output.AppendString((string)Entry.Key);

            // Get entry value
            var Value = Entry.Value;

            // Get object's type
            var Type = GetType(Value);
            if (Type != SerializationType.BYTEARRAY)
            {
                Output = Output.AppendBytes(new[] { (byte)Type });
            }
            else Output = Output.AppendBytes(new[] { (byte)SerializationType.STRING });

            // Serialize object based on type
            switch (Type)
            {
                #region Integers

                case SerializationType.LONG:
                    Output = Output.AppendInteger((long)Value);
                    break;

                case SerializationType.INT:
                    Output = Output.AppendInteger((int)Value);
                    break;

                case SerializationType.SHORT:
                    Output = Output.AppendInteger((short)Value);
                    break;

                case SerializationType.SBYTE:
                    Output = Output.AppendInteger((sbyte)Value);
                    break;

                case SerializationType.ULONG:
                    Output = Output.AppendInteger((ulong)Value);
                    break;

                case SerializationType.UINT:
                    Output = Output.AppendInteger((uint)Value);
                    break;

                case SerializationType.USHORT:
                    Output = Output.AppendInteger((ushort)Value);
                    break;

                case SerializationType.BYTE:
                    Output = Output.AppendInteger((byte)Value);
                    break;

                case SerializationType.DOUBLE:
                    Output = Output.AppendInteger((double)Value);
                    break;

                case SerializationType.BOOL:
                    Output = Output.AppendInteger((bool)Value ? (byte)0x01 : (byte)0x00);
                    break;

                #endregion

                #region Others

                case SerializationType.STRING:
                    // String size exceeds maximum length, default to nothing
                    if (((string)Value).Length > MAX_STRING_LENGTH)
                    {
                        throw new ArgumentOutOfRangeException("Entry string was too long to serialize");
                    }

                    // Append string length as a varint
                    Output = Output.AppendBytes(PackP2pVarInt(((string)Value).Length));

                    // Append string bytes
                    Output = Output.AppendString((string)Value);
                    break;

                case SerializationType.BYTEARRAY:
                    // Byte size exceeds maximum length, default to nothing
                    if (((byte[])Value).Length > MAX_STRING_LENGTH)
                    {
                        throw new ArgumentOutOfRangeException("Entry byte array was too long to serialize");
                    }

                    // Append string length as a varint
                    Output = Output.AppendBytes(PackP2pVarInt(((byte[])Value).Length));

                    // Append string bytes
                    Output = Output.AppendBytes((byte[])Value);
                    break;
                
                #endregion

                #region Not Implemented

                case SerializationType.OBJECT:
                    // Object is a dictionary
                    Type ValueType = Value.GetType();
                    if (ValueType.IsGenericType && ValueType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        // Create a portable storage from dictionary
                        PortableStorage Storage = (Dictionary<string, dynamic>)Value;

                        // Append bytes
                        Output = Output.AppendBytes(Storage.Serialize(false));
                        break;
                    }
                    else throw new NotImplementedException();

                case SerializationType.OBJECTARRAY:
                    throw new NotImplementedException();

                default:
                    return new byte[0];

                #endregion
            }

            // Return resulting output buffer
            return Output;
        }

        // Gets an object's serialization type dynamically
        private static SerializationType GetType(object Value)
        {
            // Check for null input
            if (Value == null) return SerializationType.INVALID;

            // Get object type
            Type Type = Value.GetType();

            // Convert type
            if (Type == typeof(long)) return SerializationType.LONG;
            else if (Type == typeof(int)) return SerializationType.INT;
            else if (Type == typeof(short)) return SerializationType.SHORT;
            else if (Type == typeof(sbyte)) return SerializationType.SBYTE;
            else if (Type == typeof(ulong)) return SerializationType.ULONG;
            else if (Type == typeof(uint)) return SerializationType.UINT;
            else if (Type == typeof(ushort)) return SerializationType.USHORT;
            else if (Type == typeof(byte)) return SerializationType.BYTE;
            else if (Type == typeof(double)) return SerializationType.DOUBLE;
            else if (Type == typeof(string)) return SerializationType.STRING;
            else if (Type == typeof(bool)) return SerializationType.BOOL;
            else if (Type == typeof(byte[])) return SerializationType.BYTEARRAY;
            else if (Type.IsArray) return SerializationType.OBJECTARRAY;
            else return SerializationType.OBJECT;
        }

        // Deserialized a storage entry from a byte buffer
        private byte[] DeserializeEntry(byte[] Data)
        {
            // Buffer is empty
            if (Data.Length == 0)
            {
                return Data;
            }

            // Get entry name
            int NameLength = ByteArrayToInteger<byte>(Data, 0);
            if (NameLength < 1 || NameLength > MAX_STRING_LENGTH)
            {
                throw new Exception("Name size exceeds allowed string bounds");
            }
            string Name = System.Text.Encoding.UTF8.GetString(Data, 1, NameLength);
            Data = Data.SubBytes(NameLength + 1, Data.Length - NameLength - 1);

            // Get object type
            int ValueType = Data.SubBytes(0, 1)[0];

            // Type is a serializable type
            if (ValueType > 0 || ValueType < 14)
            {
                // Get serialization type
                SerializationType Type = (SerializationType)ValueType;
                Data = Data.SubBytes(1, Data.Length - 1);

                // Deserialize object based on type
                switch (Type)
                {
                    #region Integers

                    case SerializationType.LONG:
                        Entries.Add(Name, ByteArrayToInteger<long>(Data));
                        return Data.SubBytes(8, Data.Length - 8);

                    case SerializationType.INT:
                        Entries.Add(Name, ByteArrayToInteger<int>(Data));
                        return Data.SubBytes(4, Data.Length - 4);

                    case SerializationType.SHORT:
                        Entries.Add(Name, ByteArrayToInteger<short>(Data));
                        return Data.SubBytes(2, Data.Length - 2);

                    case SerializationType.SBYTE:
                        Entries.Add(Name, ByteArrayToInteger<sbyte>(Data));
                        return Data.SubBytes(1, Data.Length - 1);

                    case SerializationType.ULONG:
                        Entries.Add(Name, ByteArrayToInteger<ulong>(Data));
                        return Data.SubBytes(8, Data.Length - 8);

                    case SerializationType.UINT:
                        Entries.Add(Name, ByteArrayToInteger<uint>(Data));
                        return Data.SubBytes(4, Data.Length - 4);

                    case SerializationType.USHORT:
                        Entries.Add(Name, ByteArrayToInteger<ushort>(Data));
                        return Data.SubBytes(2, Data.Length - 2);

                    case SerializationType.BYTE:
                        Entries.Add(Name, ByteArrayToInteger<byte>(Data));
                        return Data.SubBytes(1, Data.Length - 1);

                    case SerializationType.DOUBLE:
                        Entries.Add(Name, ByteArrayToInteger<double>(Data));
                        return Data.SubBytes(8, Data.Length - 8);

                    case SerializationType.BOOL:
                        Entries.Add(Name, Convert.ToBoolean(ByteArrayToInteger<byte>(Data)));
                        return Data.SubBytes(1, Data.Length - 1);

                    #endregion

                    #region Miscellaneous

                    case SerializationType.STRING:
                        int Length = UnpackP2pVarInt<int>(Data, 0, out int Offset);
                        Entries.Add(Name, ByteArrayToHexString(Data.SubBytes(Offset, Length)));
                        return Data.SubBytes(Offset + Length, Data.Length - Offset - Length);

                    #endregion

                    #region Not Implemented

                    case SerializationType.OBJECT:
                        PortableStorage Storage = new PortableStorage(Data, out Data, false);
                        Entries.Add(Name, Storage.Entries);
                        return Data;

                    case SerializationType.OBJECTARRAY:
                        return new byte[0];

                    default:
                        return new byte[0];

                        #endregion
                }
            }

            // Non-serializable type, treat as a single long hex string
            else
            {
                // TODO - This isn't final, I still need to explore how other things are serialized
                Entries.Add(Name, ByteArrayToHexString(Data));
                return new byte[0];
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new storage object
        /// </summary>
        public PortableStorage()
        {
            // Initialize our entry dictionary
            Entries = new Dictionary<string, dynamic>();
        }

        /// <summary>
        /// Deserializes a byte array into a storage object
        /// </summary>
        /// <param name="Data">The byte array to be deserialized from</param>
        /// <param name="Buffer">The remaining contents of the data byte array</param>
        /// <param name="IncludeHeader">Whether or not the input byte array includes a storage header</param>
        public PortableStorage(byte[] Data, out byte[] Buffer, bool IncludeHeader = true)
        {
            // Initialize our entry dictionary
            Entries = new Dictionary<string, dynamic>();

            // Verify header information matches our defined constants
            Buffer = Data;
            if (IncludeHeader)
            {
                // Check signatures and version number
                if (!STORAGE_SIGNATURE_A.Matches(Data.SubBytes(0, 4)))
                {
                    throw new ArgumentException("Invalid storage signature (A)");
                }
                else if (!STORAGE_SIGNATURE_B.Matches(Data.SubBytes(4, 4)))
                {
                    throw new ArgumentException("Invalid storage signature (B)");
                }
                else if (!STORAGE_VERSION.Matches(Data.SubBytes(8, 1)))
                {
                    throw new ArgumentException("Invalid storage version");
                }

                // Remove header section
                Buffer = Data.SubBytes(9, Data.Length - 9);
            }

            // Get entry count
            int Count = UnpackP2pVarInt<int>(Buffer, 0, out int Offset);
            Buffer = Buffer.SubBytes(Offset, Buffer.Length - Offset);

            // Loop through data buffer until it's empty
            for (int i = 0; i < Count; i++)
            {
                // Deserialize entry
                Buffer = DeserializeEntry(Buffer);
            }
        }

        #endregion
    }
}
