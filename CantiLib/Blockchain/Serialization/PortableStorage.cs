//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Canti.Utilities;

// TODO: Utilize a logger to log errors instead of throwing exceptions....
// Or should I throw exceptions, then listen for them wherever it is I use this class?
namespace Canti.Blockchain
{
    // A key-value storage solutions for encoding and decoding objects as raw bytes
    [Serializable]
    public partial class PortableStorage
    {
        // Logger
        private Logger Logger;

        // Dictionary of entries
        private Dictionary<string, object> Entries = new Dictionary<string, object>();

        // Entry point
        public PortableStorage(Logger Logger = null)
        {
            // Set logger
            if (Logger != null) this.Logger = Logger;
        }

        // Add an entry
        public bool AddEntry(string Name, object Value)
        {
            // Verify entry is valid
            if (Value == null) return false;

            // Add to entry table
            Entries.Add(Name, Value);
            return true;
        }

        // Adds a new entry as a raw hexstring representation of the object's raw bytes
        public bool AddEntryAsBinary(string Name, object Value)
        {
            // Verify entry is valid
            if (Value == null) return false;

            // Create an output array
            byte[] Output = new byte[0];
            int Size = 0;
            if (Value.GetType().IsArray)
            {
                Type ArrayType = Value.GetType();
                foreach (object Val in (Value as Array))
                    Size += Encoding.GetSizeOfObject(Val);
            }
            else Size = Encoding.GetSizeOfObject(Value);
            Output = Encoding.AppendToByteArray(SerializeVarInt(Size), Output);

            // Serialize object
            Output = Encoding.AppendToByteArray(SerializeObjectAsBinary(Value), Output);

            // Add as an entry
            AddEntry(Name, Encoding.ByteArrayToString(Output));
            return true;
        }

        // Serializes entry table to a byte array
        public byte[] Serialize(bool IncludeHeader = true)
        {
            // Create an output array
            byte[] Output = new byte[0];

            // Write header
            if (IncludeHeader)
            {
                // Add signatures
                Output = Encoding.AppendToByteArray(Encoding.IntegerToByteArray(GlobalsConfig.STORAGE_SIGNATUREA), Output);
                Output = Encoding.AppendToByteArray(Encoding.IntegerToByteArray(GlobalsConfig.STORAGE_SIGNATUREB), Output);

                // Add version number
                Output = Encoding.AppendToByteArray(GlobalsConfig.STORAGE_FORMAT_VERSION, Output);
            }

            // Add array length
            Output = Encoding.AppendToByteArray(SerializeVarInt(Entries.Count), Output);

            // Iterate over objects
            foreach (KeyValuePair<string, object> Entry in Entries)
            {
                // Serialize object
                byte[] EntryBytes = SerializeEntry(Entry.Key, Entry.Value);

                // Add to output array
                Output = Encoding.AppendToByteArray(EntryBytes, Output);
            }

            // Return output array
            return Output;
        }

        // Deserializes a byte array to a storage object
        public byte[] Deserialize(byte[] Data, bool IncludeHeader = true)
        {
            // Verify header
            int Offset = 0;
            if (IncludeHeader)
            {
                if (Encoding.ByteArrayToInteger<uint>(Data, 0) != GlobalsConfig.STORAGE_SIGNATUREA ||
                    Encoding.ByteArrayToInteger<uint>(Data, 4) != GlobalsConfig.STORAGE_SIGNATUREB)
                    throw new Exception(string.Format("Signature mismatch, expected {0}{1}, got {2}{3}",
                        GlobalsConfig.STORAGE_SIGNATUREA, GlobalsConfig.STORAGE_SIGNATUREB,
                        Encoding.ByteArrayToInteger<uint>(Data, 0), Encoding.ByteArrayToInteger<uint>(Data, 4)));
                else if (Data[8] != GlobalsConfig.STORAGE_FORMAT_VERSION)
                    throw new Exception(string.Format("Version mismatch, expected {0}, got {1}",
                        GlobalsConfig.STORAGE_FORMAT_VERSION, Data[8]));
                Offset = 9;
            }

            // Get entry count
            int Count = DeserializeVarInt<int>(Data, Offset, out Offset);

            // Create a buffer containing just the storage contents
            byte[] Buffer = Encoding.SplitByteArray(Data, Offset, Data.Length - Offset);

            // Loop through data until it's empty
            for (int i = 0; i < Count; i++)
            {
                // Deserialize entry
                Buffer = DeserializeEntry(Buffer);
            }

            // Return successful
            return Buffer;
        }

        // Deserializes an entry from a byte array and adds it to storage
        private byte[] DeserializeEntry(byte[] Buffer)
        {
            // Get entry name length
            int NameLength = Encoding.ByteArrayToInteger<byte>(Buffer, 0, 1);//DeserializeVarInt<int>(Buffer, 0, out int Offset);
            int Offset = 1;
            if (NameLength < 1 || NameLength > MAX_STRING_LENGTH) throw new Exception("Name size exceeds allowed string bounds");

            // Get entry name
            string Name = Encoding.ByteArrayToString(Buffer, Offset, NameLength);
            Offset += NameLength;
            Buffer = Encoding.SplitByteArray(Buffer, Offset, Buffer.Length - Offset);

            // Get object type
            SerializationType Type = GetType(Buffer);
            if ((int)Type < 1 || (int)Type > 14) throw new Exception("Invalid serialization type caught: " +
                Encoding.ByteArrayToHexString(Encoding.SplitByteArray(Buffer, 1, Buffer.Length - 1)));
            Buffer = Encoding.SplitByteArray(Buffer, 1, Buffer.Length - 1);

            // Create an entry object
            var Output = new object();

            // Object is a string
            if (Type == SerializationType.STRING)
            {
                // Deserialize string length
                int Length = DeserializeVarInt<int>(Buffer, 0, out Offset);

                // Deserialize string
                Output = Encoding.ByteArrayToString(Encoding.SplitByteArray(Buffer, Offset, Length));

                // Resize buffer
                Offset = 1 + (Output as string).Length;
                if (Buffer.Length - Offset > 0) Buffer = Encoding.SplitByteArray(Buffer, Offset, Buffer.Length - Offset);
            }

            // Object is an integer
            else if ((int)Type >= 1 && (int)Type <= 9)
            {
                // Create a generic method wrapper to access deserialization
                MethodInfo MethodInfo = typeof(Encoding).GetMethod("ByteArrayToInteger", new[] { typeof(byte[]), typeof(int) });
                Type[] Args = new Type[] { ConvertSerializationType(Type) };
                MethodInfo Method = MethodInfo.MakeGenericMethod(Args);

                // Deserialize integer
                Output = Method.Invoke(null, new object[] { Buffer, 0 });

                // Resize buffer
                Offset = Encoding.GetSizeOfObject(Output);
                if (Buffer.Length - Offset > 0) Buffer = Encoding.SplitByteArray(Buffer, Offset, Buffer.Length - Offset);
            }

            // Object is an object
            else if (Type == SerializationType.OBJECT)
            {
                // Create a new storage object
                PortableStorage Storage = new PortableStorage();

                // Deserialize entry
                Buffer = Storage.Deserialize(Buffer, false);
                Output = Storage;
            }

            // Add to entries
            Entries.Add(Name, Output);

            // Return buffer output
            return Buffer;
        }

        // Gets an object's serialization type
        private SerializationType GetType(object Value)
        {
            // Get serialization type
            SerializationType Type;
            if (Value == null) Type = SerializationType.INVALID;
            else if (Value.GetType() == typeof(long)) Type = SerializationType.LONG;
            else if (Value.GetType() == typeof(int)) Type = SerializationType.INT;
            else if (Value.GetType() == typeof(short)) Type = SerializationType.SHORT;
            else if (Value.GetType() == typeof(sbyte)) Type = SerializationType.SBYTE;
            else if (Value.GetType() == typeof(ulong)) Type = SerializationType.ULONG;
            else if (Value.GetType() == typeof(uint)) Type = SerializationType.UINT;
            else if (Value.GetType() == typeof(ushort)) Type = SerializationType.USHORT;
            else if (Value.GetType() == typeof(byte)) Type = SerializationType.BYTE;
            else if (Value.GetType() == typeof(double)) Type = SerializationType.DOUBLE;
            else if (Value.GetType() == typeof(string)) Type = SerializationType.STRING;
            else if (Value.GetType() == typeof(bool)) Type = SerializationType.BOOL;
            else if (Value.GetType().IsArray) Type = SerializationType.ARRAY;
            else Type = SerializationType.OBJECT;
            return Type;
        }
        private SerializationType GetType(byte[] Value)
        {
            // Get serialization type
            byte Type = Encoding.ByteArrayToInteger<byte>(Value, 0);
            return (SerializationType)Type;
        }
        private Type ConvertSerializationType(SerializationType Type)
        {
            if (Type == SerializationType.ULONG) return typeof(ulong);
            else if (Type == SerializationType.LONG) return typeof(long);
            else if (Type == SerializationType.UINT) return typeof(uint);
            else if (Type == SerializationType.INT) return typeof(int);
            else if (Type == SerializationType.USHORT) return typeof(ushort);
            else if (Type == SerializationType.SHORT) return typeof(short);
            else if (Type == SerializationType.BYTE) return typeof(byte);
            else if (Type == SerializationType.SBYTE) return typeof(sbyte);
            else if (Type == SerializationType.DOUBLE) return typeof(double);
            else if (Type == SerializationType.BOOL) return typeof(bool);
            else if (Type == SerializationType.STRING) return typeof(string);
            else if (Type == SerializationType.ARRAY) return typeof(Array);
            else if (Type == SerializationType.OBJECT) return typeof(object);
            else return null;
        }

        // Serializes an object to a byte array
        private byte[] SerializeObject(object Value, bool IncludeType = true)
        {
            // Create an output array
            byte[] Output = new byte[0];

            // Add object type
            SerializationType Type = GetType(Value);
            if (IncludeType) Output = new byte[] { (byte)Type };

            // Create entry bytes buffer
            byte[] EntryBytes = new byte[0];

            // Type is 64 bit
            if (Value.GetType() == typeof(long) || Value.GetType() == typeof(ulong))
            {
                // Encode bytes
                ulong Input = Convert.ToUInt64(Value);
                EntryBytes = Encoding.IntegerToByteArray(Input);
            }

            // Type is 32 bit
            else if (Value.GetType() == typeof(int) || Value.GetType() == typeof(uint))
            {
                // Encode bytes
                uint Input = Convert.ToUInt32(Value);
                EntryBytes = Encoding.IntegerToByteArray(Input);
            }

            // Type is 16 bit
            else if (Value.GetType() == typeof(short) || Value.GetType() == typeof(ushort))
            {
                // Encode bytes
                ushort Input = Convert.ToUInt16(Value);
                EntryBytes = Encoding.IntegerToByteArray(Input);
            }

            // Type is 8 bit
            else if (Value.GetType() == typeof(byte) || Value.GetType() == typeof(sbyte))
            {
                // Encode bytes
                byte Input = Convert.ToByte(Value);
                EntryBytes = Encoding.IntegerToByteArray(Input);
            }

            // Type is double
            else if (Value.GetType() == typeof(double))
            {
                // Encode bytes
                double Input = Convert.ToDouble(Value);
                EntryBytes = Encoding.IntegerToByteArray(Input);
            }

            // Type is string
            else if (Value.GetType() == typeof(string))
            {
                // Check string length
                if (((string)Value).Length > MAX_STRING_LENGTH) EntryBytes = new byte[0];

                // Encode bytes
                EntryBytes = Encoding.StringToByteArray((string)Value);

                // Add string length
                EntryBytes = Encoding.AppendToByteArray(EntryBytes, SerializeVarInt(((string)Value).Length));
            }

            // Type is bool
            else if (Value.GetType() == typeof(bool))
            {
                // Encode bytes
                EntryBytes = new byte[1] { (bool)Value ? (byte)1 : (byte)0 };
            }

            // Type is array
            else if (Value.GetType().IsArray)
            {
                // Encode bytes
                EntryBytes = SerializeArray((Array)Value);
            }

            // Type is object
            else
            {
                // Check if object has serialization method
                Type ObjectType = Value.GetType();
                MethodInfo Method = ObjectType.GetMethod("Serialize");
                if (Method == null) throw new Exception("Could not serialize object: No Serialize() method found in object type " + ObjectType.Name);

                // Encode bytes
                EntryBytes = (byte[])Method.Invoke(Value, null);
            }

            // Return result
            if (EntryBytes.Length > 0) return Encoding.AppendToByteArray(EntryBytes, Output);
            else return new byte[0];
        }

        // Serializes a variable int to a byte array
        private byte[] SerializeVarInt<T>(T Value) where T : IConvertible
        {
            // Create an output buffer
            byte[] Output = new byte[0];

            // Check varint size
            ulong Size = Convert.ToUInt64(Value);

            // Value is 8 bit
            if (Size <= 63)
            {
                // Encode varint
                byte Converted = Convert.ToByte(Value);
                byte Input = (byte)(Converted << 2);
                Input |= PORTABLE_RAW_SIZE_MARK_BYTE;

                // Encode bytes to buffer
                Output = Encoding.IntegerToByteArray(Input);
            }

            // Value is 16 bit
            else if (Size <= 16383)
            {
                // Encode varint
                ushort Converted = Convert.ToUInt16(Value);
                ushort Input = (ushort)(Converted << 2);
                Input |= PORTABLE_RAW_SIZE_MARK_WORD;

                // Encode bytes to buffer
                Output = Encoding.IntegerToByteArray(Input);
            }

            // Value is 32 bit
            else if (Size <= 1073741823)
            {
                // Encode varint
                uint Converted = Convert.ToUInt32(Value);
                uint Input = Converted << 2;
                Input |= PORTABLE_RAW_SIZE_MARK_DWORD;

                // Encode bytes to buffer
                Output = Encoding.IntegerToByteArray(Input);
            }

            // Value is 64 bit
            else if (Size <= 4611686018427387903)
            {
                // Encode varint
                ulong Converted = Convert.ToUInt64(Value);
                ulong Input = Converted << 2;
                Input |= PORTABLE_RAW_SIZE_MARK_INT64;

                // Encode bytes to buffer
                Output = Encoding.IntegerToByteArray(Input);
            }

            // Return encoded varint buffer
            return Output;
        }

        // Deserializes a variable int from a byte array, and returns a new offset value
        private T DeserializeVarInt<T>(byte[] Data, int Offset, out int NewOffset) where T : IConvertible
        {
            // Get byte size
            int SizeMask = Data[Offset] & PORTABLE_RAW_SIZE_MARK_MASK;
            int BytesLeft = 0;

            // Type is 8 bit
            if (SizeMask == PORTABLE_RAW_SIZE_MARK_BYTE) BytesLeft = 0;

            // Type is 16 bit
            else if (SizeMask == PORTABLE_RAW_SIZE_MARK_WORD) BytesLeft = 1;

            // Type is 32 bit
            else if (SizeMask == PORTABLE_RAW_SIZE_MARK_DWORD) BytesLeft = 3;

            // Type is 64 bit
            else if (SizeMask == PORTABLE_RAW_SIZE_MARK_INT64) BytesLeft = 7;

            // Calculate output number
            int Output = Data[Offset];
            for (NewOffset = Offset; NewOffset <= Offset + BytesLeft; NewOffset++)
                Output |= Data[NewOffset] << ((NewOffset - Offset) * 8);
            Output >>= 2;

            // Return result
            return (T)Convert.ChangeType(Output, typeof(T));
        }

        // Serializes an array to a byte array
        private byte[] SerializeArray(Array Value)
        {
            // Create an output array
            byte[] Output = new byte[0];

            // Add array size
            Output = SerializeVarInt(Value.LongLength);

            // Loop through each array object
            for (int i = 0; i < Value.Length; i++)
            {
                // Read object
                object Child = Value.GetValue(i);

                // Add bytes
                Output = Encoding.AppendToByteArray(SerializeObject(Child), Output);
            }

            // Return output array
            return Output;
        }

        // Serializes an entry to a byte array
        private byte[] SerializeEntry(string Name, object Value)
        {
            // Serialize name
            byte[] NameLength = new byte[] { (byte)Name.Length };
            byte[] NameBytes = Encoding.StringToByteArray(Name);
            NameBytes = Encoding.AppendToByteArray(NameBytes, NameLength);

            // Serialize object
            byte[] ObjectBytes = SerializeObject(Value);

            // Return output array
            return Encoding.AppendToByteArray(ObjectBytes, NameBytes);
        }

        // Serializes an object to a byte array
        private byte[] SerializeObjectAsBinary(object Value)
        {
            // Create an output array
            byte[] Output = new byte[0];

            // Object is an integer
            if (Value.GetType().GetInterfaces().Contains(typeof(IConvertible)))
            {
                // Integer is 8 bit
                if (Value.GetType() == typeof(byte) || Value.GetType() == typeof(sbyte))
                    Output = Encoding.AppendToByteArray(Encoding.IntegerToByteArray(Convert.ToByte(Value)), Output);

                // Integer is 16 bit
                else if (Value.GetType() == typeof(ushort) || Value.GetType() == typeof(short))
                    Output = Encoding.AppendToByteArray(Encoding.IntegerToByteArray(Convert.ToUInt16(Value)), Output);

                // Integer is 32 bit
                else if (Value.GetType() == typeof(uint) || Value.GetType() == typeof(int))
                    Output = Encoding.AppendToByteArray(Encoding.IntegerToByteArray(Convert.ToUInt32(Value)), Output);

                // Integer is 64 bit
                else if (Value.GetType() == typeof(ulong) || Value.GetType() == typeof(long))
                    Output = Encoding.AppendToByteArray(Encoding.IntegerToByteArray(Convert.ToUInt64(Value)), Output);
            }

            // Object is a string
            else if (Value.GetType() == typeof(string))
            {
                Output = Encoding.AppendToByteArray(SerializeVarInt(((string)Value).Length), Output);
                Output = Encoding.AppendToByteArray(Encoding.StringToByteArray((string)Value), Output);
            }

            // Object is an array
            else if (Value.GetType().IsArray)
            {
                Output = Encoding.AppendToByteArray(SerializeArrayAsBinary((Array)Value), Output);
            }

            // Property is an object
            else
            {
                // Get property list of type
                var Properties = Value.GetType().GetProperties();
                foreach (var Property in Properties)
                    Output = Encoding.AppendToByteArray(SerializeObjectAsBinary(Property.GetValue(Value)), Output);
            }

            // Return output array
            return Output;
        }

        // Serializes an array to a byte array
        private byte[] SerializeArrayAsBinary(Array Value)
        {
            // Verify array is valid
            if (Value == null) return new byte[0];
            else if (!Value.GetType().IsArray) return new byte[0];

            // Create an output array
            byte[] Output = new byte[0];

            // Loop through all array entries
            for (int i = 0; i < Value.Length; i++)
            {
                // Encode object
                byte[] Buffer = SerializeObjectAsBinary(Value.GetValue(i));

                // Append to output array
                Output = Encoding.AppendToByteArray(Buffer, Output);
            }

            // Return output array
            return Output;
        }

        // Decodes an object packed into a byte array
        private T DeserializeObjectFromBinary<T>(byte[] Data)
        {
            // Create an output object
            T Output = default(T);

            // Object is an integer
            if (typeof(T).GetInterfaces().Contains(typeof(IConvertible)))
            {
                // Create a generic method wrapper to access deserialization
                MethodInfo MethodInfo = typeof(Encoding).GetMethod("ByteArrayToInteger", new[] { typeof(byte[]), typeof(int) });
                Type[] Args = new Type[] { typeof(T) };
                MethodInfo Method = MethodInfo.MakeGenericMethod(Args);

                // Deserialize integer
                Output = (T)Method.Invoke(null, new object[] { Data, 0 });
            }

            // Object is a string
            else if (typeof(T) == typeof(string))
            {
                // Deserialize string length
                int Length = DeserializeVarInt<int>(Data, 0, out int Offset);

                // Deserialize string
                Output = (T)Convert.ChangeType(Encoding.ByteArrayToString(Encoding.SplitByteArray(Data, Offset, Length)), typeof(T));
            }

            // Object is an array
            else if (typeof(T).IsArray)
            {
                // Create a generic method wrapper to access deserialization
                MethodInfo MethodInfo = typeof(PortableStorage).GetMethod("DeserializeArrayFromBinary");
                Type[] Args = new Type[] { typeof(T) };
                MethodInfo Method = MethodInfo.MakeGenericMethod(Args);

                // Deserialize integer
                Output = (T)Method.Invoke(null, new object[] { Data });
            }

            // Property is an object
            else
            {
                // Get property list of type
                var Properties = typeof(T).GetProperties();

                // Create a buffer
                byte[] Buffer = Encoding.AppendToByteArray(Data, new byte[0]);

                // Loop through object properties
                foreach (PropertyInfo Property in Properties)
                {
                    // Create a generic method wrapper to access deserialization
                    MethodInfo MethodInfo = typeof(PortableStorage).GetMethod("DeserializeObjectFromBinary");
                    Type[] Args = new Type[] { Property.PropertyType };
                    MethodInfo Method = MethodInfo.MakeGenericMethod(Args);

                    // Set object parameter value
                    var Param = Method.Invoke(null, new object[] { Buffer });
                    object Temp = Output;
                    Property.SetValue(Temp, Convert.ChangeType(Param, Property.PropertyType));
                    Output = (T)Convert.ChangeType(Temp, typeof(T));

                    // Resize buffer
                    Buffer = Encoding.SplitByteArray(Buffer, Encoding.GetSizeOfObject(Property.GetValue(Output)), Buffer.Length - Encoding.GetSizeOfObject(Property.GetValue(Output)));
                }
            }

            // Return output
            return Output;
        }

        // Deserializes an array packed into a byte array
        public T[] DeserializeArrayFromBinary<T>(byte[] Data)
        {
            // Create a list of objects
            List<T> Output = new List<T>();

            // Get default object size
            int Size = Encoding.GetSizeOfObject(default(T)); // May not work

            // Get object count
            int Count = Data.Length / Size;

            // Loop through buffer
            for (int i = 0; i < Count; i++)
            {
                // Split buffer into just what is needed
                int offset = i * Size;
                byte[] Buffer = Encoding.SplitByteArray(Data, offset, 24);

                // Create a generic method wrapper to access deserialization
                MethodInfo MethodInfo = typeof(PortableStorage).GetMethod("DeserializeObjectFromBinary");
                Type[] Args = new Type[] { typeof(T) };
                MethodInfo Method = MethodInfo.MakeGenericMethod(Args);

                // Deserialize object
                T Param = (T)Method.Invoke(null, new object[] { Buffer });

                // Add to list
                Output.Add(Param);
            }

            // Return output
            return Output.ToArray();
        }
        public T[] DeserializeArrayFromBinary<T>(string Name)
        {
            // Get string
            string Value = (string)Entries[Name];

            // Create a generic method wrapper to access deserialization
            MethodInfo MethodInfo = typeof(PortableStorage).GetMethod("DeserializeArrayFromBinary", new[] { typeof(byte[]) });
            Type[] Args = new Type[] { typeof(T) };
            MethodInfo Method = MethodInfo.MakeGenericMethod(Args);

            // Deserialize integer
            return (T[])Method.Invoke(null, new object[] { Encoding.StringToByteArray(Value) });
        }

        // Gets an entry from the entry dictionary
        public object GetEntry(string Name)
        {
            if (Entries.ContainsKey(Name)) return Entries[Name];
            else return null;
        }

        // Gets all entries from the entry dictionary as an object array
        public object[] GetEntries()
        {
            List<object> Output = new List<object>();
            foreach (KeyValuePair<string, object> Entry in Entries)
                Output.Add(Entry.Value);
            return Output.ToArray();
        }
    }
}
