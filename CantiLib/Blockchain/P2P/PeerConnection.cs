//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net.Sockets;
using System.Threading;

namespace Canti.Blockchain.P2P
{
    public class PeerConnection
    {
        // Private variables
        private Thread RunningThread;
        private TcpClient Client;
        private Server Server;

        // Public variables
        public EventHandler OnDataReceived;

        // Entry point
        public PeerConnection(Server Server, TcpClient Client)
        {
            // Set variables
            this.Server = Server;
            this.Client = Client;

            // Bind data received handler
            OnDataReceived += delegate (object sender, EventArgs e)
            {
                // Raise event for server handler
                this.Server.OnDataReceived?.Invoke(sender, e);
            };

            // Begin running in new thread
            RunningThread = new Thread(Process);
            RunningThread.Start();
        }

        // Sends a message to the associated client
        public bool SendMessage(byte[] Data)
        {
            // Send data to client stream
            try { Client.GetStream().Write(Data, 0, Data.Length); }

            // Unable to send (connection was closed)
            catch { return false; }

            // Raise data sent event
            Server.OnDataSent?.Invoke(new Packet(this, Data), EventArgs.Empty);
            return true;
        }

        // Main logic thread
        private void Process()
        {
            // Get network stream from client
            NetworkStream Stream = Client.GetStream();

            // Begin update loop
            while (Server.Running)
            {
                // Check if data is available
                while (Stream.DataAvailable)
                {
                    // Create a byte buffer then read incoming data
                    Byte[] Data = new Byte[Client.Available];
                    Stream.Read(Data, 0, Data.Length);

                    // Queue incoming request
                    OnDataReceived?.Invoke(new Packet(this, Data), EventArgs.Empty);
                }

                // Let the thread sleep if no data is available
                Thread.Sleep(200);
            }

            // Close connection
            Client.Close();
        }

        // Checks if the peer is still connected
        public bool Connected
        {
            get
            {
                return Client.Connected;
            }
        }

        // Returns connection information as a string
        public string Address
        {
            get
            {
                return Client.Client.RemoteEndPoint.ToString();
            }
        }
    }
}
