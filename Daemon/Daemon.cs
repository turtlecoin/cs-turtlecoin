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
            ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "Server error: " + sender, LogLevel.ERROR);
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
            ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "Server started on port " + Server.Port + ", peer ID of " + Server.PeerId + "\n", LogLevel.INFO);
        }

        // Custom server stopped handling
        private static void ServerStopped(object sender, EventArgs e)
        {
            Server Server = (Server)sender;
            ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "Server stopped", LogLevel.INFO);
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

            Logger.Log(LogLevel.DEBUG, "Daemon.Construct", "Daemon started. Running? " + Running);
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
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "Enter a URL:", LogLevel.INFO);
                    string Url = Console.ReadLine();
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "Enter a port:", LogLevel.INFO);
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

                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "[OUT] Sending Handshake Request:", LogLevel.DEBUG);
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Node Data:", LogLevel.DEBUG);
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Network ID: " + Encoding.StringToHexString(Request.NodeData.NetworkId), LogLevel.DEBUG);
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Peer ID: " + Request.NodeData.PeerId, LogLevel.DEBUG);
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Version: " + Request.NodeData.Version, LogLevel.DEBUG);
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Local Time: " + Request.NodeData.LocalTime, LogLevel.DEBUG);
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Port: " + Request.NodeData.Port, LogLevel.DEBUG);
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "- Core Sync Data:", LogLevel.DEBUG);
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Current Height: " + Request.PayloadData.CurrentHeight, LogLevel.DEBUG);
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "  - Top ID: " + Encoding.StringToHexString(Request.PayloadData.TopId), LogLevel.DEBUG);

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
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "Peers:", LogLevel.DEBUG);
                    ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, Peers, LogLevel.DEBUG);
                }

                // Write menu
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "Menu:", LogLevel.INFO);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, " 1\tConnect to a Server", LogLevel.INFO);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, " 2\tTest Packet", LogLevel.INFO);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, " 3\tShow Peer List", LogLevel.INFO);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, " 4\tExit\n", LogLevel.INFO);
                ConsoleMessage.WriteLine(ConsoleMessage.DefaultColor, "Enter Selection:", LogLevel.INFO);

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
