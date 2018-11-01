//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Blockchain;
using Canti.Blockchain.Commands;
using Canti.Blockchain.P2P;
using Canti.Data;
using Canti.Utilities;
using System;
using System.Collections.Generic;

namespace Daemon
{
    // Network daemon initializing class
    partial class Daemon
    {
        #region Variables
        // P2p server
        private static Server Server;

        // Protocol handler
        private static IProtocol Protocol;

        // Database handler
        private static IDatabase Database;

        // True if daemon is running
        public bool Running = false;
        #endregion

        #region Event handlers
        // Custom incoming data handling
        private static void DataReceived(object sender, EventArgs e)
        {
            Packet Packet = (Packet)sender;
            //Logger.Log(Level.DEBUG, "Received packet from {0}: {1}", Packet.Peer.Address, Encoding.ByteArrayToHexString(Packet.Data));
        }

        // Custom outgoing data handling
        private static void DataSent(object sender, EventArgs e)
        {
            Packet Packet = (Packet)sender;
            //Logger.Log(Level.DEBUG, "Sent packet to {0}: {1}", Packet.Peer.Address, Encoding.ByteArrayToHexString(Packet.Data));
        }

        // An error was received
        private static void ServerError(object sender, EventArgs e)
        {
            Logger.Log(Level.ERROR, "Server error: {0}", (string)sender);
        }

        // Custom peer connected handling
        private static void PeerConnected(object sender, EventArgs e)
        {
            PeerConnection Peer = (PeerConnection)sender;
            //Logger.Log(Level.DEBUG, "Peer connection formed with {0}", Peer.Address);
        }

        // Custom peer disconnected handling
        private static void PeerDisconnected(object sender, EventArgs e)
        {
            PeerConnection Peer = (PeerConnection)sender;
            //Logger.Log(Level.DEBUG, "Peer connection lost with {0}", Peer.Address);
        }

        // Custom server start handling
        private static void ServerStarted(object sender, EventArgs e)
        {
            Server Server = (Server)sender;
            Logger.Log(Level.INFO, "Server started on port {0}, peer ID of {1}", Server.Port, Server.PeerId);
        }

        // Custom server stopped handling
        private static void ServerStopped(object sender, EventArgs e)
        {
            Server Server = (Server)sender;
            Logger.Log(Level.INFO, "Server stopped", Server.Port, Server.PeerId);
        }
        #endregion

        // Entry point
        public Daemon(int Port = 0)
        {

            // Create server
            Server = new Server();

            // Bind event handlers
            Server.OnStart = ServerStarted;
            Server.OnStop = ServerStopped;
            Server.OnDataReceived += DataReceived;
            Server.OnDataSent += DataSent;
            Server.OnError += ServerError;
            Server.OnPeerConnected += PeerConnected;

            // Subscribe a protocol handler
            Protocol = new LevinProtocol(Server);

            // Start server
            if (Port != 0) Server.Start(Port);
            else Server.Start(P2pPort);

            // Set as running
            Running = true;
        }

        // Run daemon
        public void Start()
        {
            // Check if running
            if (!Running) return;

            /*
             * 
             * This is basically all debugging still
             * 
             */

            // Enter into a loop
            int MenuSelection = 0;
            while (MenuSelection != 4 && Running)
            {
                // Manually connect to a peer
                if (MenuSelection == 1)
                {
                    Logger.Log(Level.INFO, "Enter a URL:");
                    string Url = Console.ReadLine();
                    Logger.Log(Level.INFO, "Enter a port:");
                    int Port = int.Parse(Console.ReadLine());
                    Server.Connect(new Connection(Url, Port, ""));

                }

                // Broadcast a test packet
                else if (MenuSelection == 2)
                {
                    // Create a response
                    Handshake.Request Request = new Handshake.Request
                    {
                        NodeData = new NodeData()
                        {
                            NetworkId = GlobalsConfig.NETWORK_ID,
                            Version = 1,
                            Port = 8090,
                            LocalTime = GeneralUtilities.GetTimestamp(),
                            PeerId = Server.PeerId
                        },
                        PayloadData = new CoreSyncData()
                        {
                            CurrentHeight = Globals.DAEMON_BLOCK_HEIGHT,
                            TopId = Globals.DAEMON_TOP_ID
                        }
                    };

                    // Get body bytes
                    byte[] BodyBytes = Request.Serialize();

                    // Create a header
                    BucketHead2 Header = new BucketHead2
                    {
                        Signature = GlobalsConfig.LEVIN_SIGNATURE,
                        ResponseRequired = false,
                        PayloadSize = (ulong)BodyBytes.Length,
                        CommandCode = (uint)Handshake.Id,
                        ProtocolVersion = GlobalsConfig.LEVIN_VERSION,
                        Flags = LevinProtocol.LEVIN_PACKET_RESPONSE,
                        ReturnCode = LevinProtocol.LEVIN_RETCODE_SUCCESS
                    };

                    Logger.Log(Level.DEBUG, "[OUT] Sending Handshake Request:");
                    Logger.Log(Level.DEBUG, "- Node Data:");
                    Logger.Log(Level.DEBUG, "  - Network ID: {0}", Encoding.StringToHexString(Request.NodeData.NetworkId));
                    Logger.Log(Level.DEBUG, "  - Peer ID: {0}", Request.NodeData.PeerId);
                    Logger.Log(Level.DEBUG, "  - Version: {0}", Request.NodeData.Version);
                    Logger.Log(Level.DEBUG, "  - Local Time: {0}", Request.NodeData.LocalTime);
                    Logger.Log(Level.DEBUG, "  - Port: {0}", Request.NodeData.Port);
                    Logger.Log(Level.DEBUG, "- Core Sync Data:");
                    Logger.Log(Level.DEBUG, "  - Current Height: {0}", Request.PayloadData.CurrentHeight);
                    Logger.Log(Level.DEBUG, "  - Top ID: {0}", Encoding.StringToHexString(Request.PayloadData.TopId));

                    // Send notification
                    Server.Broadcast(Encoding.AppendToByteArray(BodyBytes, Header.Serialize()));
                }

                // Show peer list
                else if (MenuSelection == 3)
                {
                    Server.Prune();
                    string Peers = "";
                    List<PeerConnection> PeerList = Server.GetPeerList();
                    foreach (PeerConnection Peer in PeerList) Peers += Peer.Address + " ";
                    Logger.Log(Level.DEBUG, "Peers:");
                    Logger.Log(Level.DEBUG, Peers);
                }

                // Write menu
                Logger.Log(Level.INFO, "Menu:");
                Logger.Log(Level.INFO, " 1\tConnect to a Server");
                Logger.Log(Level.INFO, " 2\tTest Packet");
                Logger.Log(Level.INFO, " 3\tShow Peer List");
                Logger.Log(Level.INFO, " 4\tExit");
                Logger.Log(Level.INFO, "Enter Selection:");

                // Get menu selection
                MenuSelection = int.Parse(Console.ReadLine());
            }

            // Stop daemon
            Stop();
        }

        public void Stop()
        {
            // Set to not running
            Running = false;

            // Close all connections
            Server?.Close();
        }
    }
}
