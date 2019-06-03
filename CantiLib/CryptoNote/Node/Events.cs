//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    public sealed partial class Node : INode
    {
        #region Methods

        // Assigns our callbacks to our underlying P2P server's corresponding event handlers
        private void AssignCallbacks()
        {
            P2pServer.OnStart += OnStart;
            P2pServer.OnStop += OnStop;
            P2pServer.OnPeerConnected += OnP2pPeerConnected;
            P2pServer.OnPeerDisconnected += OnP2pPeerDisconnected;
            P2pServer.OnDataReceived += OnDataReceived;
            P2pServer.OnDataSent += OnDataSent;
        }

        #endregion

        #region Events Callbacks

        #region Public

        /// <summary>
        /// Invoked when the underlying P2P server is started successfully
        /// </summary>
        /// <param name="sender">A reference to the P2pServer instance</param>
        /// <param name="e">Always empty</param>
        public void OnStart(object sender, EventArgs e) { }

        /// <summary>
        /// Invoked when the underlying P2P server is stopped
        /// </summary>
        /// <param name="sender">A reference to the P2pServer instance</param>
        /// <param name="e">Always empty</param>
        public void OnStop(object sender, EventArgs e) { }

        /// <summary>
        /// Invoked when a new peer connection is detected by the underlying P2P server
        /// </summary>
        /// <param name="sender">A reference to the associated P2pPeer instance</param>
        /// <param name="e">Always empty</param>
        public void OnP2pPeerConnected(object sender, EventArgs e)
        {
            // Get peer data
            var P2pPeer = (P2pPeer)sender;

            // Add this peer to our peer list
            AddPeer(P2pPeer);
        }

        /// <summary>
        /// Invoked when a peer's polling attempt comes back unsuccessful, indicating a disconnection
        /// </summary>
        /// <param name="sender">A reference to the associated P2pPeer instance</param>
        /// <param name="e">Always empty</param>
        public void OnP2pPeerDisconnected(object sender, EventArgs e)
        {
            // Get peer data
            var P2pPeer = (P2pPeer)sender;

            // Remove peer from peer list
            RemovePeer(P2pPeer);
        }

        /// <summary>
        /// Invoked when a connected peer sends the underlying P2P server data
        /// </summary>
        /// <param name="sender">(P2pPeer Peer, byte[] Data) - A reference to 
        /// the associated P2pPeer instance, as well as the data received</param>
        /// <param name="e">Always empty</param>
        public void OnDataReceived(object sender, EventArgs e)
        {
            // Get the incoming data
            var Data = ((P2pPeer Peer, byte[] Buffer))sender;

            // Add data to packet buffer
            AddData(Data.Peer, Data.Buffer);
        }

        /// <summary>
        /// Invoked when the underlying P2P server sends data to a connected peer
        /// </summary>
        /// <param name="sender">(P2pPeer Peer, byte[] Data) - A reference to 
        /// the associated P2pPeer instance, as well as the data sent</param>
        /// <param name="e">Always empty</param>
        public void OnDataSent(object sender, EventArgs e) { }

        #endregion

        #region Internal

        // This is called when we receive a valid packet
        internal void OnPacketReceived(Peer Peer, Packet Packet)
        {
            // Log debug message
            Logger.Debug($"[{Peer.Address}:{Peer.Port} IN] {Packet.Type} {Packet.Flag} (VALIDATED: {Peer.Validated}, " +
                $"RESPONSE REQUIRED: {Packet.Header.ResponseRequired})");
            //Logger.Debug($"  Payload Size: {Packet.Header.PayloadSize}");
            //PrintPacketBody(Packet.Body);

            // Wrap this in a try-catch, in case we received an invalid packet
            try
            {
                // Handle this packet
                switch (Packet.Type)
                {
                    case PacketType.HANDSHAKE:
                        HandleHandshake(Peer, Packet);
                        break;

                    case PacketType.TIMED_SYNC:
                        HandleTimedSync(Peer, Packet);
                        break;

                    case PacketType.NOTIFY_CHAIN_REQUEST:
                        HandleNotifyChain(Peer, Packet);
                        break;

                    case PacketType.NOTIFY_CHAIN_RESPONSE:
                        HandleNotifyChain(Peer, Packet);
                        break;
                }

                // If we reach this far, the packet was valid, reset peer's last seen timestamp
                Peer.LastSeen = GetTimestamp();
            }

            // We use this exception type when a packet contains invalid data
            catch (InvalidOperationException e)
            {
                Logger.Warning($"{e.Message}, killing connection");
                RemovePeer(Peer);
            }
            
            // TODO - uncomment this catch when done debugging to catch any unknown errors
            /*catch (Exception e)
            {
                Logger.Warning($"Invalid {Packet.Type} packet received from {Peer.Address}: {e.Message}, killing connection...");
                RemovePeer(Peer);
            }*/
        }

        // This is called when we send a packet
        internal void OnPacketSent(Peer Peer, Packet Packet)
        {
            // Log debug message
            Logger.Debug($"[{Peer.Address}:{Peer.Port} OUT] {Packet.Type} {Packet.Flag} (VALIDATED: {Peer.Validated}, " +
                $"RESPONSE REQUIRED: {Packet.Header.ResponseRequired})");
        }

        // This is called when a handshake is accepted
        internal void OnPeerConnected(Peer Peer)
        {
            // Log connection message
            Logger.WriteLine($"[{Peer.Address}:{Peer.Port} {Peer.P2pPeer.Direction}] CONNECTION FORMED");

            // TODO - un-comment when syncing is ready to be tested, otherwise we get flooded
            //NotifyChain(Peer);
        }

        // This is called when a peer is removed from our peer list
        internal void OnPeerDisconnected(Peer Peer)
        {
            // Log disconnection message
            if (!Stopped)
            {
                Logger.WriteLine($"[{Peer.Address}:{Peer.Port} {Peer.P2pPeer.Direction}] PEER DISCONNECTED");

                // Warn if we have no more connections
                if (PeerList.Count == 0)
                {
                    Logger.Warning("All peer connections have been dropped");
                }
            }
        }

        #endregion

        #endregion

        // TODO - This is test code to print full packet bodies
        private void PrintPacketBody(dynamic Body, int Depth = 1)
        {
            int Index = 0;
            foreach (KeyValuePair<string, dynamic> Entry in Body)
            {
                Type ValueType = Entry.Value.GetType();
                if (ValueType.IsGenericType && ValueType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    string Output = $"[{Index}] {Entry.Key}:";
                    Output = Output.PadLeft(Output.Length + (Depth * 2));
                    Logger.Debug(Output);
                    PrintPacketBody(Entry.Value, Depth + 1);
                }
                else if (Entry.Key == "block_ids")
                {
                    string Output = $"[{Index}] {Entry.Key}:";
                    Output = Output.PadLeft(Output.Length + (Depth * 2));
                    Logger.Debug(Output);
                    for (var i = 0; i < Entry.Value.Length; i += 64)
                    {
                        string tmp = $"[{i / 64}] {Entry.Value.Substring(i, 64)}";
                        tmp = tmp.PadLeft(tmp.Length + ((Depth + 1) * 2));
                        Logger.Debug(tmp);
                    }
                }
                else
                {
                    string Output = $"[{Index}] {Entry.Key}: {Entry.Value}";
                    Output = Output.PadLeft(Output.Length + (Depth * 2));
                    Logger.Debug(Output);
                }
                Index++;
            }
        }
    }
}
