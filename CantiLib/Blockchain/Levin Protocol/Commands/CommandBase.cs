//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

// Contains command base structs that can later be inherited
namespace Canti.Blockchain
{
    // Contains serialization requirement for command request structures
    public interface ICommandRequestBase
    {
        // Serializes command request to byte array
        byte[] Serialize();
    }

    // Contains deserialization requirement for command response structures
    public interface ICommandResponseBase<T>
    {
        // Deseriaizes a byte array to a command response
        //T Deserialize(byte[] Data);
    }
}
