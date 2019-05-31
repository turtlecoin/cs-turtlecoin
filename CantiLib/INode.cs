//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti
{
    /// <summary>
    /// Interface for a network node, allowing for future protocol extensibility
    /// </summary>
    public interface INode
    {
        #region Methods

        /// <summary>
        /// Starts this node and any associated threads
        /// </summary>
        /// <returns>True if started successfully</returns>
        bool Start();

        /// <summary>
        /// Stops this node and waits for all associated threads to exit
        /// </summary>
        void Stop();

        /// <summary>
        /// Adds a peer to the connection queue to be accepted when space is available
        /// </summary>
        /// <param name="Address">The remote peer's host address</param>
        /// <param name="Port">The remote peer's listening port</param>
        /// <returns>True if connection was made</returns>
        bool AddPeer(string Address, int Port);

        #endregion

        #region Event Callbacks

        /// <summary>
        /// Invoked when the underlying P2P server is started successfully
        /// </summary>
        /// <param name="sender">A reference to the P2pServer instance</param>
        /// <param name="e">Always empty</param>
        void OnStart(object sender, EventArgs e);

        /// <summary>
        /// Invoked when the underlying P2P server is stopped
        /// </summary>
        /// <param name="sender">A reference to the P2pServer instance</param>
        /// <param name="e">Always empty</param>
        void OnStop(object sender, EventArgs e);

        /// <summary>
        /// Invoked when a new peer connection is detected by the underlying P2P server
        /// </summary>
        /// <param name="sender">A reference to the associated P2pPeer instance</param>
        /// <param name="e">Always empty</param>
        void OnP2pPeerConnected(object sender, EventArgs e);

        /// <summary>
        /// Invoked when a peer's polling attempt comes back unsuccessful, indicating a disconnection
        /// </summary>
        /// <param name="sender">A reference to the associated P2pPeer instance</param>
        /// <param name="e">Always empty</param>
        void OnP2pPeerDisconnected(object sender, EventArgs e);

        /// <summary>
        /// Invoked when a connected peer sends the underlying P2P server data
        /// </summary>
        /// <param name="sender">(P2pPeer Peer, byte[] Data) - A reference to 
        /// the associated P2pPeer instance, as well as the data received</param>
        /// <param name="e">Always empty</param>
        void OnDataReceived(object sender, EventArgs e);

        /// <summary>
        /// Invoked when the underlying P2P server sends data to a connected peer
        /// </summary>
        /// <param name="sender">(P2pPeer Peer, byte[] Data) - A reference to 
        /// the associated P2pPeer instance, as well as the data sent</param>
        /// <param name="e">Always empty</param>
        void OnDataSent(object sender, EventArgs e);

        #endregion
    }
}
