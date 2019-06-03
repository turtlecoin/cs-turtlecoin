//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;
using System.Threading;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    // TODO - see which of these are actually used, pulled these values from core source
    internal enum PeerState
    {
        BEFORE_HANDSHAKE,
        SYNCHRONIZING,
        IDLE,
        NORMAL,
        SYNC_REQUIRED,
        POOL_SYNC_REQUIRED,
        SHUTDOWN
    };

    internal sealed class Peer
    {
        #region Properties and Fields

        #region Internal

        // Reference to the raw P2P peer object
        internal P2pPeer P2pPeer { get; set; }

        // Returns the connected address as a string in a similar form to "127.0.0.1:11897"
        internal string Address { get; set; }

        // Returns the underlying P2P peer's connected port
        internal uint Port { get; set; }

        // A unique identifier for this peer
        internal ulong Id { get; set; }

        // Whether or not this peer connection has been validated via handshake
        internal bool Validated { get; set; }

        // The unix timestamp of the last time this peer was heard from
        internal ulong LastSeen { get; set; }

        // The last known height of this peer
        internal uint Height { get; set; }

        // The connection state of this peer
        internal PeerState State { get; set; }

        #endregion

        #region Private

        // Used to determine whether or not we are reading a packet header or body
        private enum PacketReadState
        {
            HEADER = 0,
            BODY = 1
        }

        // Reference to the node owning this peer
        private Node Node { get; set; }

        // The working byte buffer for reading packets
        private byte[] Buffer { get; set; }

        // The current read state for reading packings
        private PacketReadState ReadState { get; set; }

        // The packet header for whichever packet is being read
        private PacketHeader Header { get; set; }

        #region Threading

        private ManualResetEvent StopEvent { get; set; }
        private ManualResetEvent ReadyEvent { get; set; }
        private Thread PacketBufferThread { get; set; }
        private Queue<byte[]> IncomingBufferQueue { get; set; }

        #endregion

        #endregion

        #endregion

        #region Methods

        // Disposes of this peer, stopping any threads
        internal void Dispose()
        {
            // Stop packet buffer thread
            StopEvent.Set();
            PacketBufferThread.Join();
        }

        // Sends a message to our underlying P2P peer object
        internal void SendMessage(Packet Packet)
        {
            P2pPeer.SendMessage(Packet.Serialize());
            Node.OnPacketSent(this, Packet);
        }

        // Adds a byte array to our packet reading buffer
        internal void AddData(byte[] Data)
        {
            // Lock our incoming data queue to prevent race conditions
            lock (IncomingBufferQueue)
            {
                // Add the incoming data to our queue
                IncomingBufferQueue.Enqueue(Data);

                // Signal a ready event
                ReadyEvent.Set();
            }
        }

        // Reads our packet buffer queue if there is any data to be read
        private void ReadPacketBuffer()
        {
            // Create a wait handle so we only process data as it's sent to us
            WaitHandle[] Wait = new[] { ReadyEvent, StopEvent };
            while (0 == WaitHandle.WaitAny(Wait))
            {
                // Lock our incoming data queue to prevent race conditions
                lock (IncomingBufferQueue)
                {
                    // Check if there is data waiting
                    if (IncomingBufferQueue.Count > 0)
                    {
                        // Get data from queue
                        var Data = IncomingBufferQueue.Dequeue();

                        // Process this data
                        ProcessBuffer(Data);
                    }

                    // There is not incoming data
                    else
                    {
                        // No data to be read, reset our ready event
                        ReadyEvent.Reset();
                        continue;
                    }
                }
            }
        }

        // Processes our packet reading buffer to deserialize it into a usable packet
        internal void ProcessBuffer(byte[] Data)
        {
            // Add this data to our working byte buffer
            Buffer = Buffer.AppendBytes(Data);

            // Header deserialization
            if (ReadState == PacketReadState.HEADER)
            {
                // Check if we have enough data for a packet header
                if (Buffer.Length < 33)
                {
                    return;
                }

                // Attempt to deserialize our packet header
                try
                {
                    // Store header data and set our read state to body
                    Header = new PacketHeader(Buffer);
                    ReadState = PacketReadState.BODY;
                }
                catch { }

                // Remove the bytes we read from our buffer
                Buffer = Buffer.SubBytes(33, Buffer.Length - 33);
            }

            // Body deserialization
            if (ReadState == PacketReadState.BODY)
            {
                // Check whether our buffer is the correct size
                if ((ulong)Buffer.LongLength < Header.PayloadSize)
                {
                    return;
                }

                // Attempt to deserialize our packet body
                PortableStorage Body = null;
                try
                {
                    // Get body bytes
                    var BodyBytes = Buffer.SubBytes(0, (int)Header.PayloadSize);

                    // Store deserialized body object
                    Body = new PortableStorage(BodyBytes, out _);
                }
                catch { }

                // Invoke our packet received handler
                if (Body != null)
                {
                    Node.OnPacketReceived(this, new Packet(Header, Body));
                }

                // Remove the bytes we read from our buffer
                Buffer = Buffer.SubBytes((int)Header.PayloadSize, Buffer.Length - (int)Header.PayloadSize);

                // Reset our read state back to header
                ReadState = PacketReadState.HEADER;
            }
        }

        #endregion

        #region Constructors

        internal Peer(Node Node, P2pPeer P2pPeer)
        {
            // Assign variable references
            this.Node = Node;
            this.P2pPeer = P2pPeer;

            // Set peer values
            Validated = false;
            State = PeerState.BEFORE_HANDSHAKE;
            LastSeen = GetTimestamp();
            Address = P2pPeer.Address;
            if (P2pPeer.Direction == PeerDirection.OUT)
            {
                Port = (uint)P2pPeer.Port;
            }

            // Setup packet buffer variables
            Buffer = new byte[0];
            ReadState = PacketReadState.HEADER;
            Header = new PacketHeader();

            // Ready our threading objects
            IncomingBufferQueue = new Queue<byte[]>();
            ReadyEvent = new ManualResetEvent(false);
            StopEvent = new ManualResetEvent(false);
            PacketBufferThread = new Thread(ReadPacketBuffer);

            // Start thread
            PacketBufferThread.Start();
        }

        #endregion
    }
}
