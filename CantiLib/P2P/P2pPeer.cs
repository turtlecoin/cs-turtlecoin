//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Canti
{
    /// <summary>
    /// Enumerates whether a peer connection is incoming or outgoing
    /// </summary>
    public enum PeerDirection
    {
        IN,
        OUT
    }

    /// <summary>
    /// A P2P peer belonging to a P2pServer
    /// </summary>
    public sealed class P2pPeer
    {
        #region Properties and Fields

        #region Internal

        // Returns the remote address of the peer
        internal string Address { get; private set; }

        // Returns the port of the underlying socket connection
        internal int Port { get; private set; }

        // The raw TCP client connection
        internal TcpClient Client { get; private set; }

        // Specifies the direction this peer connection is coming from
        internal PeerDirection Direction { get; set; }

        #endregion

        #region Private

        // The P2P server this peer is associated with
        private P2pServer Server { get; set; }

        // Thread that handles reading incoming data
        private Thread ReadThread { get; set; }

        // Thread that handles sending outgoing data from the queue
        private Thread WriteThread { get; set; }

        // Event that is set when peer is told to stop
        private ManualResetEvent StopEvent { get; set; }

        // Event that is set when data is waiting to be written
        private ManualResetEvent ReadyEvent { get; set; }

        // Keeps track of outgoing data arrays
        private Queue<byte[]> OutgoingMessageQueue { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Internal

        // Starts this peer's associated threads
        internal void Start()
        {
            // Begin reading
            Read();

            // Begin writing
            WriteThread.Start();

            // Invoke peer connected callback
            Server.OnPeerConnected?.Invoke(this, EventArgs.Empty);
        }

        // Stops this peer's associated threads
        internal void Stop()
        {
            // Invoke peer disconnected callback
            Server.OnPeerDisconnected?.Invoke(this, EventArgs.Empty);

            // Set stop event
            StopEvent.Set();

            // Stop write thread
            if (WriteThread.IsAlive)
            {
                WriteThread.Join();
            }

            // Stop client
            Client.Close();
        }

        // Adds an outgoing message to our queue
        internal void SendMessage(byte[] Data)
        {
            // Lock message queue to prevent race conditions
            lock (OutgoingMessageQueue)
            {
                // Add data to outgoing message queue
                OutgoingMessageQueue.Enqueue(Data);
                ReadyEvent.Set();
            }
        }

        // Polls this connection and returns true if it's active
        internal bool Poll()
        {
            try
            {
                if (Client != null && Client.Client != null && Client.Client.Connected)
                {
                    if (Client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        if (Client.Client.Receive(new byte[1], SocketFlags.Peek) == 0) return false;
                        else return true;
                    }
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        #endregion

        #region Private

        // Reads incoming data from the client's network stream
        private void Read()
        {
            // Get client stream
            // TODO - this can fail if internet drops, needs to be debugged and checked for
            var Stream = Client.GetStream();

            // Create a byte buffer
            int BytesRead = Client.Available;
            byte[] Buffer = new byte[BytesRead];

            // Create a wait handle array so we can cancel this thread if need be
            Stream.BeginRead(Buffer, 0, BytesRead, (IAsyncResult Result) =>
            {
                try
                {
                    // Check if peer has been stopped
                    if (StopEvent.WaitOne(0)) return;

                    // End reading
                    BytesRead = Stream.EndRead(Result);

                    // Check if any data was read
                    if (BytesRead > 0)
                    {
                        // Invoke data received handler
                        Server.OnDataReceived?.Invoke((this, Buffer), EventArgs.Empty);
                    }
                }
                catch { }

                // Start reading again
                Read();
            }, null);
        }

        // Sends outgoing data from the outgoing data queue
        private void Write()
        {
            // Get client stream
            var Stream = Client.GetStream();

            // Create a wait handle array so we can cancel this thread if need be
            WaitHandle[] Wait = new[] { ReadyEvent, StopEvent };
            while (0 == WaitHandle.WaitAny(Wait))
            {
                // Check if peer has been stopped
                if (StopEvent.WaitOne(0)) break;

                // Lock our outgoing message queue to prevent race conditions
                lock (OutgoingMessageQueue)
                {
                    // Check if we have data to send
                    if (OutgoingMessageQueue.Count > 0)
                    {
                        // Get the next packet in line
                        var Data = OutgoingMessageQueue.Dequeue();

                        // Send data to our associated peer
                        Stream.Write(Data, 0, Data.Length);

                        // Invoke data sent handler
                        Server.OnDataSent?.Invoke((this, Data), EventArgs.Empty);
                    }

                    // No outgoing messages in the queue
                    else
                    {
                        // Reset ready event to wait again
                        ReadyEvent.Reset();
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Constructors

        // Initializes a new P2P peer
        internal P2pPeer(P2pServer Server, TcpClient Client, PeerDirection Direction)
        {
            // Assign local variables
            this.Server = Server;
            this.Client = Client;
            this.Direction = Direction;
            // TODO - Should we use ipv6 or v4?
            Address = ((IPEndPoint)Client.Client.RemoteEndPoint).Address.MapToIPv4().ToString();
            Port = ((IPEndPoint)Client.Client.RemoteEndPoint).Port;

            // Threading
            StopEvent = new ManualResetEvent(false);
            ReadyEvent = new ManualResetEvent(false);
            WriteThread = new Thread(Write);
            OutgoingMessageQueue = new Queue<byte[]>();
        }

        #endregion
    }
}
