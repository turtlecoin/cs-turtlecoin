//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Blockchain.Crypto;
using Canti.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// TODO:
// - Prune old peers periodically
// - Fix Close not actually closing everything... Find where it's hanging up
namespace Canti.Blockchain.P2P
{
    public class Server
    {
        // Private variables
        private TcpListener Listener;
        private List<PeerConnection> Peers = new List<PeerConnection>();
        private Queue<Packet> IncomingRequests = new Queue<Packet>();
        private Queue<Packet> OutgoingRequests = new Queue<Packet>();
        private Thread IncomingRequestThread;
        private Thread OutgoingRequestThread;
        private Thread PeerConnectionThread;

        // public variables
        public Logger Logger;
        public bool Running = false;
        public int Port = 0;
        public ulong PeerId = 0;

        // Event handlers
        public EventHandler OnStart;
        public EventHandler OnStop;
        public EventHandler OnError;
        public EventHandler OnDataReceived;
        public EventHandler OnDataSent;
        public EventHandler OnPeerConnected;
        public EventHandler OnPeerDisconnected;

        // Start server on specified port
        public void Start(int Port = GlobalsConfig.P2P_DEFAULT_PORT)
        {
            // Create a new TCP listener and start listening
            try
            {
                // Set publics
                this.Port = Port;
                PeerId = SecureRandom.Integer<ulong>();

                // Start listener
                Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Port);
                Listener.Start();
                Running = true;

                // Raise server start event
                OnStart?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                // Failed to start the server for some reason or another
                Logger.Log(Level.FATAL, "Failed to start P2P server on port {0}, port may be in use", Port);
                OnError?.Invoke("Failed to start P2P server", EventArgs.Empty);
            }

            // Start request handling thread
            IncomingRequestThread = new Thread(ProcessIncomingRequests);
            IncomingRequestThread.Start();

            // Start request handling thread
            OutgoingRequestThread = new Thread(ProcessOutgoingRequests);
            OutgoingRequestThread.Start();

            // Start connection handling thread
            PeerConnectionThread = new Thread(PeerConnection);
            PeerConnectionThread.Start();
        }

        // Handles all incoming connections
        private void PeerConnection()
        {
            // Begin update loop
            while (Running)
            {
                // Loop as long as there are pending connections
                while (Listener.Pending())
                {
                    // Accept client connection
                    TcpClient Client = Listener.AcceptTcpClient();

                    // Add to peer list
                    PeerConnection Peer = new PeerConnection(this, Client);
                    Peers.Add(Peer);

                    // Log connection to console
                    Logger?.Log(Level.DEBUG, "Peer connection formed with {0}", Peer.Address);

                    // Invoke connection event handler
                    OnPeerConnected?.Invoke(Peer, EventArgs.Empty);
                }

                // Let the thread sleep
                Thread.Sleep(200);
            }
        }

        // Handles all incoming requests in order of appearance
        private void ProcessIncomingRequests()
        {
            while (Running)
            {
                // Wait for requests
                while (IncomingRequests.Count > 0)
                {
                    OnDataReceived?.Invoke(IncomingRequests.Dequeue(), EventArgs.Empty);
                }

                // Let the thread sleep
                Thread.Sleep(200);
            }
        }

        // Handles all outgoing requests in order of appearance
        private void ProcessOutgoingRequests()
        {
            while (Running)
            {
                // Wait for requests
                while (OutgoingRequests.Count > 0)
                {
                    // Get request data from queue
                    Packet Request = OutgoingRequests.Dequeue();

                    // Broadcast data if no client is specified
                    if (Request.Peer == null) Broadcast(Request.Data);

                    // Send to specified peer
                    else SendMessage(Request.Peer, Request.Data);

                    // Invoke data sent event
                    OnDataSent?.Invoke(Request, EventArgs.Empty);
                }

                // Let the thread sleep
                Thread.Sleep(200);
            }
        }

        // Connect to a specified URL
        public void Connect(Connection Connection)
        {
            // Try to connect to peer
            try
            {
                // Create a connection
                TcpClient Client = new TcpClient(Connection.Host, Connection.Port);

                // Add to peer list
                PeerConnection Peer = new PeerConnection(this, Client);
                Peers.Add(Peer);

                // Log connection to console
                Logger?.Log(Level.DEBUG, "Peer connection formed with {0}", Peer.Address);

                // Invoke connection event handler
                OnPeerConnected?.Invoke(Peer, EventArgs.Empty);
            }

            // Unable to connect to peer
            catch
            {
                // Log error to console
                Logger?.Log(Level.ERROR, "Peer connection could not be formed with {0}:{1}", Connection.Host, Connection.Port);

                // Raise error event
                OnError?.Invoke("Unable to connect to peer " + Connection.Host + ":" + Connection.Port, EventArgs.Empty);
            }
        }

        // Broadcast data to all peers
        public void Broadcast(byte[] Data)
        {
            // Send data to all peers
            foreach (PeerConnection Peer in Peers) Peer.SendMessage(Data);
        }

        // Send data to a specified peer
        public bool SendMessage(PeerConnection Peer, byte[] Data)
        {
            // Check if connection is alive
            if (!Peer.Connected)
            {
                // Remove peer from peer list
                Peers.Remove(Peer);

                // Raise peer disconnected event
                OnPeerDisconnected?.Invoke(Peer, EventArgs.Empty);
                return false;
            }

            // Send data to peer
            return Peer.SendMessage(Data);
        }

        // Prunes disconnected peers from the peer list
        public void Prune()
        {
            // Create a new peer list
            List<PeerConnection> PeerList = new List<PeerConnection>();

            // Iterate through connected peers and add them to the list
            foreach (PeerConnection Peer in Peers) PeerList.Add(Peer);

            // Iterate through newly created list and remove disconnected peers from connected peer list
            foreach (PeerConnection Peer in PeerList) if (!Peer.Connected) Peers.Remove(Peer);
        }

        // Returns a list of connected peers
        public List<PeerConnection> GetPeerList()
        {
            // Create a new peer list
            List<PeerConnection> PeerList = new List<PeerConnection>();

            // Iterate through connected peers and add them to the list
            foreach (PeerConnection Peer in Peers) PeerList.Add(Peer);

            // Return newly created list
            return PeerList;
        }

        // Closes all connections
        public void Close()
        {
            // Reset publics
            PeerId = 0;
            Port = 0;
            Running = false;

            // Stop the listener
            Listener?.Stop();

            // Clear all collections
            IncomingRequests.Clear();
            OutgoingRequests.Clear();
            Peers.Clear();

            // Raise stop event handler
            OnStop?.Invoke(this, EventArgs.Empty);
        }
    }
}
