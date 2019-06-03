//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;
using System.Linq;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        #region Properties and Fields

        #region Internal

        // Contains a list of our connected peers
        internal List<Peer> PeerList { get; set; }

        #endregion

        #endregion

        #region Methods

        // Starts peer list operations
        private void StartPeerList()
        {
            // Assign variables
            PeerList = new List<Peer>();
        }

        // Stops peer list operations
        private void StopPeerList()
        {
            // Loop until peer list is empty
            while (PeerList.Count > 0)
            {
                // Dispose of and remove peer
                RemovePeer(PeerList[0]);
            }
        }

        // Adds a new peer to the peer list
        private void AddPeer(P2pPeer P2pPeer)
        {
            // Do not add peer if not running
            if (Stopped)
            {
                RemovePeer(P2pPeer);
                return;
            }

            // Lock the peer list to prevent race conditions
            lock (PeerList)
            {
                // Check if this peer already exists
                if (PeerList.Any(x => x.P2pPeer == P2pPeer)) return;

                // Create a new local peer
                var Peer = new Peer(this, P2pPeer);

                // Add the peer to our peer list
                PeerList.Add(Peer);

                // Peer is outgoing
                if (P2pPeer.Direction == PeerDirection.OUT)
                {
                    // Request handshake
                    RequestHandshake(Peer);
                }
            }
        }

        // Removes a peer from the peer list
        private void RemovePeer(Peer Peer)
        {
            // Lock peer list to prevent race conditions
            lock (PeerList)
            {
                // Check if peer is null, why not
                if (Peer == null) return;

                // Remove peer from the peer list
                if (PeerList.Contains(Peer))
                {
                    PeerList.Remove(Peer);
                    OnPeerDisconnected(Peer);
                }

                // Dispose of peer
                Peer.Dispose();

                // Stop and remove the underlying P2pPeer
                Peer.P2pPeer.Stop();
                P2pServer.RemovePeer(Peer.P2pPeer);
            }
        }

        // Removes a peer from the peer list based on a P2pPeer object
        private void RemovePeer(P2pPeer Peer)
        {
            // Lock peer list to prevent race conditions
            lock (PeerList)
            {
                // Check if this peer exists
                if (PeerList.Any(x => x.P2pPeer == Peer))
                {
                    // Remove peer from the peer list
                    RemovePeer(PeerList.First(x => x.P2pPeer == Peer));
                }
            }
        }

        // Adds a packet of data to a peer's buffer
        private void AddData(P2pPeer P2pPeer, byte[] Buffer)
        {
            // Do nothing if not running
            if (Stopped) return;

            // Lock peer list to prevent race conditions
            lock (PeerList)
            {
                // Check if peer list containers this peer
                if (!PeerList.Any(x => x.P2pPeer == P2pPeer))
                {
                    AddPeer(P2pPeer);
                }

                // Add data to this peer's buffer
                PeerList.First(x => x.P2pPeer == P2pPeer).AddData(Buffer);
            }
        }

        // Returns the number of connected peers
        private int GetPeerCount()
        {
            // Do nothing if not running
            if (Stopped) return 0;

            // Lock peer list to prevent race conditions
            lock (PeerList)
            {
                // Return peer list count
                return PeerList.Count;
            }
        }

        // Serializes our connected peer list as a byte array
        // TODO - Show historic peer list??
        private byte[] SerializePeerList()
        {
            // Lock peer list to prevent race conditions
            lock (PeerList)
            {
                // Create a buffer to work on
                byte[] Buffer = new byte[0];

                // Iterate through all *validated* peers
                foreach (var Peer in PeerList.Where(x => x.Validated))
                {
                    // Serialize peer attributes
                    Buffer = Buffer.AppendInteger(IpAddressToUint(Peer.Address));
                    Buffer = Buffer.AppendInteger(Peer.Port);
                    Buffer = Buffer.AppendInteger(Peer.Id);
                    Buffer = Buffer.AppendInteger(Peer.LastSeen);
                }
                
                // Return completed buffer
                return Buffer;
            }
        }

        #endregion
    }
}
