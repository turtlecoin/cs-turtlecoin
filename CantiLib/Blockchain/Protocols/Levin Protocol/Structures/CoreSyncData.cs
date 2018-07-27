//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.Blockchain
{
    [Serializable]
    public struct CoreSyncData
    {
        public uint CurrentHeight { get; set; }
        public string TopId { get; set; }
        public byte[] Serialize()
        {
            PortableStorage Storage = new PortableStorage();
            Storage.AddEntry("current_height", CurrentHeight);
            Storage.AddEntry("top_id", TopId);
            return Storage.Serialize(false);
        }
        public static CoreSyncData Deserialize(object Value)
        {
            if (Value.GetType() != typeof(PortableStorage)) throw new Exception("Invalid deserialization object");
            PortableStorage Storage = (PortableStorage)Value;
            return new CoreSyncData
            {
                CurrentHeight = (uint)Storage.GetEntry("current_height"),
                TopId = (string)Storage.GetEntry("top_id")
            };
        }
    }
}
