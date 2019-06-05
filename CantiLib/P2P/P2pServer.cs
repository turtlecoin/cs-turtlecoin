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
    /// A standalone P2P server
    /// </summary>
    public sealed class P2pServer
    {
        #region Properties and Fields

        #region Public

        /// <summary>
        /// Invoked when the server is started successfully
        /// </summary>
        /// <param name="sender">A reference to this P2pServer instance</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnStart { get; set; }

        /// <summary>
        /// Invoked when the server is stopped
        /// </summary>
        /// <param name="sender">A reference to this P2pServer instance</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnStop { get; set; }

        /// <summary>
        /// Invoked when a new peer connection is detected
        /// </summary>
        /// <param name="sender">A reference to the associated P2pPeer instance</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnPeerConnected { get; set; }

        /// <summary>
        /// Invoked when a peer's polling attempt comes back unsuccessful, indicating a disconnection
        /// </summary>
        /// <param name="sender">A reference to the associated P2pPeer instance</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnPeerDisconnected { get; set; }

        /// <summary>
        /// Invoked when a connected peer sends data
        /// </summary>
        /// <param name="sender">(P2pPeer Peer, byte[] Data) - A reference to 
        /// the associated P2pPeer instance, as well as the data received</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnDataReceived { get; set; }

        /// <summary>
        /// Invoked when the server sends data to a connected peer
        /// </summary>
        /// <param name="sender">(P2pPeer Peer, byte[] Data) - A reference to 
        /// the associated P2pPeer instance, as well as the data sent</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnDataSent { get; set; }

        /// <summary>
        /// The maximum number of concurrent peer connections this server will allow
        /// </summary>
        public int MaxConnections { get; private set; }

        /// <summary>
        /// The maximum number of concurrent peer connections this server will allow
        /// </summary>
        public int WorkerCount { get; private set; }

        /// <summary>
        /// The amount of time (in milliseconds) to wait for a connection with a peer before timing out
        /// </summary>
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// The port this server is binded to
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Returns a copy of the connected peer list
        /// </summary>
        public List<P2pPeer> PeerList
        {
            get
            {
                // Lock peer list to prevent race conditions
                lock (Peers)
                {
                    // Return a copied list instance
                    return new List<P2pPeer>(Peers);
                }
            }
        }

        /// <summary>
        /// Returns a copy of the pending connection list
        /// </summary>
        public List<TcpClient> PendingConnections
        {
            get
            {
                // Lock connection queue to prevent race conditions
                lock (ConnectionQueue)
                {
                    // Return a copied list instance
                    return new List<TcpClient>(ConnectionQueue);
                }
            }
        }

        #endregion

        #region Private

        // Our listener
        private TcpListener Listener { get; set; }

        // Event that is set when the server is stopped
        private ManualResetEvent StopEvent { get; set; }

        // Event that is set when the server detects a pending connection
        private ManualResetEvent ReadyEvent { get; set; }

        // Queue of incoming connections
        private Queue<TcpClient> ConnectionQueue { get; set; }

        // Connection workers
        private List<Thread> Workers { get; set; }

        // Dictionary that links one peer worker thread to a peer client
        private List<P2pPeer> Peers { get; set; }

        // Handles connection polling
        private Timer PollingTimer { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Starts listening for new connections
        /// </summary>
        /// <param name="Port">The port to listen for incoming connections on</param>
        /// <param name="PollingInterval">How often (in milliseconds) connections should be polled</param>
        public void Start(int Port, int PollingInterval)
        {
            // Store port
            this.Port = Port;

            // Setup and start our TCP listener on our desired port
            var EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            Listener = new TcpListener(EndPoint);

            // Start listening for connections
            Listener.Start();
            Listener.BeginAcceptTcpClient(Listen, null);

            // Assign and start each of our peer worker threads
            foreach (var Worker in Workers)
            {
                Worker.Start();
            }

            // Begin polling connections
            PollingTimer = new Timer(new TimerCallback(PollPeers), null, 0, PollingInterval);

            // Invoke on start event handler, signalling that our threads have begun
            OnStart?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Stops the server and ends all associated threads
        /// </summary>
        public void Stop()
        {
            // Stop polling timer
            PollingTimer.Dispose();

            // Stop our TCP listener
            Listener.Stop();

            // Stop all worker threads
            StopEvent.Set();
            foreach (var Worker in Workers)
            {
                Worker.Join();
            }

            // Remove all peers
            while (Peers.Count > 0)
            {
                RemovePeer(Peers[0]);
            }

            // Invoke on stop event handler, signalling that our threads have exited
            OnStop?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Broadcasts a byte array to all connected peers
        /// </summary>
        /// <param name="Data">The byte array to be sent</param>
        public void Broadcast(byte[] Data)
        {
            // Lock peer list to prevent race conditions
            lock (Peers)
            {
                // Iterate through peers
                foreach (var Peer in Peers)
                {
                    // Send a message to this peer
                    Peer.SendMessage(Data);
                }
            }
        }

        /// <summary>
        /// Adds a peer to the connection queue to be accepted when space is available
        /// </summary>
        /// <param name="Address">The remote peer's host address</param>
        /// <param name="Port">The remote peer's listening port</param>
        /// <returns>True if connection was made</returns>
        public bool AddPeer(string Address, int Port)
        {
            // Wrap this in a try-catch to catch if a connection failed
            try
            {
                // Create a new TcpClient instance
                var Connection = new TcpClient();

                // Begin connecting to the remote address
                var Result = Connection.BeginConnect(Address, Port, null, null);

                // Wait for either a connection or a timeout to elapse
                if (!Result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(ConnectionTimeout)))
                {
                    // Connection timed out
                    Connection.Close();
                    return false;
                }

                // Did not time out, attempt to grab connection
                else
                {
                    // End connection request
                    Connection.EndConnect(Result);

                    // Add peer to list
                    AddPeer(new P2pPeer(this, Connection, PeerDirection.OUT));

                    // Connection was successful
                    return true;
                }
            }
            catch
            {
                // Failed to end connection request, connection failed
                return false;
            }
        }

        /// <summary>
        /// Removes a peer from the peer list
        /// </summary>
        /// <param name="Peer">The peer to remove</param>
        public void RemovePeer(P2pPeer Peer)
        {
            // Lock peer list to prevent race conditions
            lock (Peers)
            {
                // Stop this peer
                Peer.Stop();

                // Remove this peer from our peer list
                Peers.RemoveAll(x => x == Peer);
            }
        }

        #endregion

        #region Private

        // Adds an incoming connection to the queue
        private void Listen(IAsyncResult Result)
        {
            // Check if server is bound
            if (Listener.Server.IsBound)
            {
                try
                {
                    // Get incoming connection
                    var Connection = Listener.EndAcceptTcpClient(Result);

                    // Lock our connection queue to prevent any race conditions
                    lock (ConnectionQueue)
                    {
                        // Add the incoming connection to our connection queue
                        ConnectionQueue.Enqueue(Connection);

                        // Signal that a connection is ready to be accepted
                        ReadyEvent.Set();
                    }

                    // Start listening again
                    Listener.BeginAcceptTcpClient(Listen, null);
                }
                catch { }
            }
        }

        // Accepts incoming connections if we are able
        private void AcceptPeerConnection()
        {
            // Create a wait handle array so we can cancel this thread if need be
            WaitHandle[] Wait = new[] { ReadyEvent, StopEvent };
            while (0 == WaitHandle.WaitAny(Wait))
            {
                // Check if stopped
                if (StopEvent.WaitOne(0)) break;

                // Lock our connection queue to prevent any race conditions
                lock (ConnectionQueue)
                {
                    // Connection queue has entries, accept one
                    if (ConnectionQueue.Count > 0)
                    {
                        // Dequeue the new peer in line
                        var Connection = ConnectionQueue.Dequeue();

                        // Create a peer instance
                        var Peer = new P2pPeer(this, Connection, PeerDirection.IN);

                        // Handle this connection
                        AddPeer(Peer);
                    }

                    // There are no entries in the connection queue
                    else
                    {
                        // No peers in line, reset ready event
                        ReadyEvent.Reset();
                        continue;
                    }
                }
            }
        }

        // Polls peer connections to see if any have disconnected
        private void PollPeers(object _)
        {
            // Lock peer list to prevent race conditions
            lock (Peers)
            {
                // Poll each connected peer
                for (int i = 0; i < Peers.Count; i++)
                {
                    // Remove peer
                    if (!Peers[i].Poll())
                    {
                        RemovePeer(Peers[i]);
                    }
                }
            }
        }

        // Handles a new peer client
        private void AddPeer(P2pPeer Peer)
        {
            // Lock peer list to prevent race conditions
            lock (Peers)
            {
                // Check that we have space for this peer
                if (Peers.Count < MaxConnections)
                {
                    Peer.Start();
                    Peers.Add(Peer);
                }

                // No space available, close connection
                else
                {
                    Peer.Stop();
                }
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new P2P server
        /// </summary>
        /// <param name="MaxConnections">The maximum number of concurrent peer connections this server will allow</param>
        public P2pServer(int WorkerCount, int MaxConnections)
        {
            // Assign variables
            this.WorkerCount = WorkerCount;
            this.MaxConnections = MaxConnections;
            Workers = new List<Thread>();
            Peers = new List<P2pPeer>();

            // Setup threads and events
            ReadyEvent = new ManualResetEvent(false);
            StopEvent = new ManualResetEvent(false);
            ConnectionQueue = new Queue<TcpClient>();
            for (int i = 0; i < WorkerCount; i++)
            {
                Workers.Add(new Thread(AcceptPeerConnection));
            }
        }

        #endregion
    }
}
