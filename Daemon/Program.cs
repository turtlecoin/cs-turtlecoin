//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Utilities;
using Canti.Data;
using Canti.Blockchain.P2P;
using System;
using System.Collections.Generic;
using Canti.Blockchain;
using Canti.Blockchain.Commands;

namespace Daemon
{
    class Program
    {
        public static int Port = 8091;
        public static Server Server = new Server();
        public static Logger Logger = new Logger();

        static void DataReceived(object sender, EventArgs e)
        {
            // Custom incoming data handling
            Packet Packet = (Packet)sender;
            //Logger.Log(Level.DEBUG, "Received packet from {0}: {1}", Packet.Peer.Address, Encoding.ByteArrayToHexString(Packet.Data));
        }

        static void DataSent(object sender, EventArgs e)
        {
            // Custom outgoing data handling
            Packet Packet = (Packet)sender;
            //Logger.Log(Level.DEBUG, "Sent packet to {0}: {1}", Packet.Peer.Address, Encoding.ByteArrayToHexString(Packet.Data));
        }

        static void ServerError(object sender, EventArgs e)
        {
            // An error was received
            Logger.Log(Level.ERROR, "Server error: {0}", (string)sender);
        }

        static void PeerConnected(object sender, EventArgs e)
        {
            // Custom peer connected handling
            PeerConnection Peer = (PeerConnection)sender;
            //Logger.Log(Level.DEBUG, "Peer connection formed with {0}", Peer.Address);
        }

        static void PeerDisconnected(object sender, EventArgs e)
        {
            // Custom peer disconnected handling
            PeerConnection Peer = (PeerConnection)sender;
            //Logger.Log(Level.DEBUG, "Peer connection lost with {0}", Peer.Address);
        }

        static void ServerStarted(object sender, EventArgs e)
        {
            // Custom server start handling
            Server Server = (Server)sender;
            Logger.Log(Level.INFO, "Server started on port {0}, peer ID of {1}", Server.Port, Server.PeerId);
        }

        static void ServerStopped(object sender, EventArgs e)
        {
            // Custom server stopped handling
            Server Server = (Server)sender;
            Logger.Log(Level.INFO, "Server stopped", Server.Port, Server.PeerId);
        }

        static void Main(string[] args)
        {
            // Parse commandline arguments
            if (args.Length >= 1) Port = int.Parse(args[0]);

            // Set log level and start logger
            Logger.LogLevel = Level.DEBUG;
            Logger.Start();

            // Add logger to server
            Server.Logger = Logger;

            // Bind event handlers
            Server.OnStart = ServerStarted;
            Server.OnStop = ServerStopped;
            Server.OnDataReceived += DataReceived;
            Server.OnDataSent += DataSent;
            Server.OnError += ServerError;
            Server.OnPeerConnected += PeerConnected;

            // Start server
            Server.Start(Port);

            // Enter into a loop
            int MenuSelection = 0;
            while (MenuSelection != 4)
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

                    Logger?.Log(Level.DEBUG, "[OUT] Sending Handshake Request:");
                    Logger?.Log(Level.DEBUG, "- Node Data:");
                    Logger?.Log(Level.DEBUG, "  - Network ID: {0}", Encoding.StringToHexString(Request.NodeData.NetworkId));
                    Logger?.Log(Level.DEBUG, "  - Peer ID: {0}", Request.NodeData.PeerId);
                    Logger?.Log(Level.DEBUG, "  - Version: {0}", Request.NodeData.Version);
                    Logger?.Log(Level.DEBUG, "  - Local Time: {0}", Request.NodeData.LocalTime);
                    Logger?.Log(Level.DEBUG, "  - Port: {0}", Request.NodeData.Port);
                    Logger?.Log(Level.DEBUG, "- Core Sync Data:");
                    Logger?.Log(Level.DEBUG, "  - Current Height: {0}", Request.PayloadData.CurrentHeight);
                    Logger?.Log(Level.DEBUG, "  - Top ID: {0}", Encoding.StringToHexString(Request.PayloadData.TopId));

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

            // Close all connections
            Server.Close();

            // Close logger
            Logger.Stop();
        }
    }
}
